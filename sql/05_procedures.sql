/*
    FinanceTracker — stored procedures
*/

CREATE PROCEDURE dbo.AddFinancialRecord
    @UserId       INT,
    @CategoryId   INT,
    @RecordTypeId INT,
    @Title        VARCHAR(100),
    @Amount       DECIMAL(18,2),
    @Date         DATETIME,
    @Description  VARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Amount <= 0
    BEGIN
        RAISERROR('Amount should be greater than zero', 16, 1);
        RETURN;   -- RAISERROR only reports; RETURN is what stops execution
    END

    INSERT INTO FinancialRecords
        (UserId, CategoryId, RecordTypeId, Title, Amount, Date, Description)
    VALUES
        (@UserId, @CategoryId, @RecordTypeId, @Title, @Amount, @Date, @Description);
END;
GO

CREATE PROCEDURE dbo.DeleteRecordById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM FinancialRecords WHERE FinancialRecordId = @Id)
    BEGIN
        RAISERROR('Record not found', 16, 1);
        RETURN;
    END

    DELETE FROM FinancialRecords
    WHERE FinancialRecordId = @Id;
END;
GO

/*
    Usage:

    EXEC dbo.AddFinancialRecord
        @UserId = 1, @CategoryId = 5, @RecordTypeId = 2,
        @Title = 'Test food', @Amount = 750, @Date = '2026-08-20';

    -- fails on validation
    EXEC dbo.AddFinancialRecord
        @UserId = 1, @CategoryId = 5, @RecordTypeId = 2,
        @Title = 'Bad', @Amount = -100, @Date = '2026-08-20';

    -- fails on the composite foreign key: the procedure does not bypass
    -- database constraints
    EXEC dbo.AddFinancialRecord
        @UserId = 1, @CategoryId = 7, @RecordTypeId = 1,
        @Title = 'Wrong pair', @Amount = 500, @Date = '2026-08-20';

    EXEC dbo.DeleteRecordById @Id = 999;   -- 'Record not found'
*/
