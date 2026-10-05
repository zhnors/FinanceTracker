/*
    FinanceTracker — indexes and execution plan experiment

    Measured on a 50k-row dataset, Ctrl+M in SSMS for the actual plan.
*/

-- The audit triggers would log every generated row; switch them off first.
DISABLE TRIGGER trg_AuditInsert ON FinancialRecords;
GO

/*
    Test data generator.

    CROSS JOIN over two system tables produces far more combinations than
    needed; TOP (50000) cuts it down. CategoryId and RecordTypeId come from
    the same Categories row, so the composite foreign key always holds.
*/
INSERT INTO FinancialRecords (UserId, CategoryId, RecordTypeId, Title, Amount, Date, Description)
SELECT TOP (50000)
    (ABS(CHECKSUM(NEWID())) % 3) + 1,
    C.CategoryId,
    C.RecordTypeId,
    C.CategoryName,
    CAST((ABS(CHECKSUM(NEWID())) % 30000) + 100 AS DECIMAL(18,2)),
    DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 730), GETDATE()),
    'Generated test record'
FROM sys.all_objects A
CROSS JOIN sys.all_objects B
CROSS JOIN Categories C;
GO

ENABLE TRIGGER trg_AuditInsert ON FinancialRecords;
GO

SET STATISTICS IO ON;
SET STATISTICS TIME ON;
GO

/*
    Baseline, no index on Amount:
    Clustered Index Scan, 470 logical reads — the whole table.
*/
SELECT * FROM FinancialRecords WHERE Amount > 25000;
GO

CREATE NONCLUSTERED INDEX IX_FinancialRecords_Amount
ON FinancialRecords (Amount);
GO

/*
    Measured results after creating the index above:

    SELECT *            WHERE Amount > 25000  ->  Clustered Index Scan, 470 reads
    SELECT id, Amount   WHERE Amount > 29500  ->  Index Seek,             5 reads
    SELECT id, Amount   WHERE Amount > 25000  ->  Index Seek,            23 reads
    SELECT *            WHERE Amount > 29500  ->  Clustered Index Scan, 470 reads

    What the numbers show: the deciding factor is whether the index covers
    the query, not how many rows come back. A nonclustered index stores the
    key plus the clustered key, so `SELECT FinancialRecordId, Amount` is
    answered from the index alone. `SELECT *` needs every other column, so
    each matching row would require a Key Lookup into the table — past a few
    hundred rows the optimizer decides scanning is cheaper and drops the
    index entirely.
*/

SELECT FinancialRecordId, Amount FROM FinancialRecords WHERE Amount > 29500;
SELECT FinancialRecordId, Amount FROM FinancialRecords WHERE Amount > 25000;
SELECT * FROM FinancialRecords WHERE Amount > 29500;
GO

/*
    Key columns vs INCLUDE:
      key     — searchable and sortable, widens the B-tree, costs writes
      INCLUDE — readable only, stored on the leaf pages, keeps the tree narrow

    Rule: filter and sort columns go in the key, columns that are merely
    returned go in INCLUDE.
*/
CREATE NONCLUSTERED INDEX IX_FinancialRecords_User_Type
ON FinancialRecords (UserId, RecordTypeId)
INCLUDE (Title, Amount, Date);
GO

-- Index Seek, no Key Lookup: 3 logical reads
SELECT Title, Amount, Date
FROM FinancialRecords
WHERE UserId = 1 AND RecordTypeId = 2;
GO

/*
    Notes worth keeping:

    - An index is useless when the column is wrapped in a function.
      WHERE YEAR(Date) = 2026                                -> no seek
      WHERE Date >= '2026-01-01' AND Date < '2027-01-01'     -> seek
      (the searchable form is called "SARGable")

    - Every index slows down INSERT/UPDATE/DELETE and takes disk space,
      so they are added for specific slow queries, not just in case.

    - Low selectivity kills the benefit: an index on RecordTypeId, which has
      two distinct values, will not be used.

    List the indexes on a table:

    SELECT name, type_desc, is_unique
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('FinancialRecords');
*/
