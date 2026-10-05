/*
    FinanceTracker — audit log and triggers

    Deliberately no foreign key to FinancialRecords: the log has to keep
    traces of rows that no longer exist.
*/

CREATE TABLE RecordsAudit (
    AuditId           INT IDENTITY(1,1) NOT NULL,
    FinancialRecordId INT               NOT NULL,
    Operation         VARCHAR(10)       NOT NULL,
    Amount            DECIMAL(18,2)     NULL,
    ChangedAt         DATETIME          NOT NULL,
    CONSTRAINT PK_RecordsAudit PRIMARY KEY (AuditId)
);
GO

/*
    A trigger fires once per statement, not once per row — inserting 33 rows
    in one INSERT fires it a single time with all 33 rows in `inserted`.
    Hence INSERT ... SELECT rather than reading a variable, which would
    silently keep only one arbitrary row.
*/
CREATE TRIGGER trg_AuditInsert
ON FinancialRecords
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO RecordsAudit (FinancialRecordId, Operation, Amount, ChangedAt)
    SELECT FinancialRecordId, 'INSERT', Amount, GETDATE()
    FROM inserted;
END;
GO

CREATE TRIGGER trg_AuditUpdate
ON FinancialRecords
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- `inserted` holds the new values, `deleted` the previous ones
    INSERT INTO RecordsAudit (FinancialRecordId, Operation, Amount, ChangedAt)
    SELECT FinancialRecordId, 'UPDATE', Amount, GETDATE()
    FROM inserted;
END;
GO

CREATE TRIGGER trg_AuditDelete
ON FinancialRecords
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO RecordsAudit (FinancialRecordId, Operation, Amount, ChangedAt)
    SELECT FinancialRecordId, 'DELETE', Amount, GETDATE()
    FROM deleted;
END;
GO

/*
    Verification.

    1) all three operations -> three audit rows

    INSERT INTO FinancialRecords (UserId, CategoryId, RecordTypeId, Title, Amount, Date, Description)
    VALUES (1, 5, 2, 'Test', 500, '2026-08-20', 'Trigger test');

    UPDATE FinancialRecords SET Amount = 600 WHERE Title = 'Test';
    DELETE FROM FinancialRecords WHERE Title = 'Test';

    SELECT * FROM RecordsAudit;

    2) multi-row behaviour -> one statement, two audit rows

    INSERT INTO FinancialRecords (UserId, CategoryId, RecordTypeId, Title, Amount, Date, Description)
    VALUES
    (1, 5, 2, 'Multi 1', 100, '2026-08-21', 'Test'),
    (1, 6, 2, 'Multi 2', 200, '2026-08-21', 'Test');

    SELECT * FROM RecordsAudit WHERE Operation = 'INSERT';

    Triggers can be switched off for bulk loads, otherwise the audit table
    grows by one row per loaded row:

    DISABLE TRIGGER trg_AuditInsert ON FinancialRecords;
    ENABLE  TRIGGER trg_AuditInsert ON FinancialRecords;
*/
