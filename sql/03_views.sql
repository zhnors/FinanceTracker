/*
    FinanceTracker — views
*/

/*
    Readable form of a financial record: ids replaced with names.
    RecordTypeName is reached through Categories; thanks to the composite
    foreign key this always agrees with FinancialRecords.RecordTypeId.
*/
CREATE VIEW vw_RecordsFull
AS
SELECT
    FR.FinancialRecordId,
    U.FirstName,
    U.LastName,
    RT.RecordTypeName,
    C.CategoryName,
    FR.Title,
    FR.Amount,
    FR.Date,
    FR.Description
FROM FinancialRecords FR
JOIN Users       U  ON FR.UserId       = U.UserId
JOIN Categories  C  ON FR.CategoryId   = C.CategoryId
JOIN RecordTypes RT ON C.RecordTypeId  = RT.RecordTypeId;
GO

CREATE VIEW vw_Expenses
AS
SELECT
    FinancialRecordId,
    FirstName,
    LastName,
    RecordTypeName,
    CategoryName,
    Title,
    Amount,
    Date,
    Description
FROM vw_RecordsFull
WHERE RecordTypeName = 'Expense';
GO

CREATE VIEW vw_CategoryTotals
AS
SELECT
    C.CategoryName,
    RT.RecordTypeName,
    COUNT(*)         AS RecordsCount,
    SUM(FR.Amount)   AS TotalAmount
FROM Categories C
JOIN RecordTypes      RT ON C.RecordTypeId = RT.RecordTypeId
JOIN FinancialRecords FR ON C.CategoryId   = FR.CategoryId
GROUP BY C.CategoryName, RT.RecordTypeName;
GO

/*
    Monthly balance per user.

    Conditional aggregation: CASE is evaluated per row before SUM, which
    turns one set of rows into separate income / expense columns in a
    single pass over the data.
*/
CREATE VIEW vw_MonthlyBalance
AS
SELECT
    CONCAT(FirstName, ' ', LastName) AS FullName,
    YEAR(Date)                       AS RecordYear,
    MONTH(Date)                      AS RecordMonth,
    SUM(CASE WHEN RecordTypeName = 'Income'  THEN Amount  ELSE 0 END) AS TotalIncome,
    SUM(CASE WHEN RecordTypeName = 'Expense' THEN Amount  ELSE 0 END) AS TotalExpense,
    SUM(CASE
            WHEN RecordTypeName = 'Income'  THEN  Amount
            WHEN RecordTypeName = 'Expense' THEN -Amount
            ELSE 0
        END)                                                         AS Balance
FROM vw_RecordsFull
GROUP BY CONCAT(FirstName, ' ', LastName), YEAR(Date), MONTH(Date);
GO

/*
    A view returns a set, not an ordered list — ORDER BY belongs in the
    query that reads it:

    SELECT * FROM vw_MonthlyBalance ORDER BY FullName, RecordYear, RecordMonth;
*/
