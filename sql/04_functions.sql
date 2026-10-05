/*
    FinanceTracker — user-defined functions
*/

/*
    Scalar function: total expenses of one user in a given month.
    ISNULL guards the empty case — SUM over no rows returns NULL, and the
    function must return a number.
*/
CREATE FUNCTION dbo.GetMonthlyExpenses (@UserId INT, @Year INT, @Month INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @Total DECIMAL(18,2);

    SELECT @Total = SUM(FR.Amount)
    FROM FinancialRecords FR
    WHERE FR.UserId = @UserId
      AND YEAR(FR.Date)  = @Year
      AND MONTH(FR.Date) = @Month
      AND FR.RecordTypeId = 2;

    RETURN ISNULL(@Total, 0);
END;
GO

/*
    Inline table-valued function — a parameterised view.
    The result can be filtered further, like a table:

        SELECT * FROM dbo.GetRecordsByCategory(7) WHERE Amount > 12000;
*/
CREATE FUNCTION dbo.GetRecordsByCategory (@CategoryId INT)
RETURNS TABLE
AS
RETURN
(
    SELECT
        FinancialRecordId,
        UserId,
        CategoryId,
        RecordTypeId,
        Title,
        Amount,
        Date,
        Description
    FROM FinancialRecords
    WHERE CategoryId = @CategoryId
);
GO

/*
    Usage:

    SELECT dbo.GetMonthlyExpenses(1, 2026, 7);   -- 20700.00
    SELECT dbo.GetMonthlyExpenses(1, 2020, 1);   -- 0.00, not NULL
    SELECT * FROM dbo.GetRecordsByCategory(7);
*/
