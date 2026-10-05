/*
    FinanceTracker — database schema
    SQL Server (T-SQL)

    Run order: 01_schema -> 02_seed_data -> 03_views -> 04_functions
               -> 05_procedures -> 06_triggers -> 07_indexes
*/

CREATE TABLE Users (
    UserId    INT IDENTITY(1,1) NOT NULL,
    FirstName VARCHAR(50)       NOT NULL,
    LastName  VARCHAR(50)       NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY (UserId)
);
GO

CREATE TABLE RecordTypes (
    RecordTypeId   INT IDENTITY(1,1) NOT NULL,
    RecordTypeName VARCHAR(10)       NOT NULL,
    CONSTRAINT PK_RecordTypes PRIMARY KEY (RecordTypeId),
    CONSTRAINT UQ_RecordTypes_Name UNIQUE (RecordTypeName)
);
GO

/*
    A category belongs to exactly one record type:
    'Rent' is always an expense, 'Salary' is always an income.

    UQ_Categories_CategoryId_RecordTypeId looks redundant (CategoryId is
    already the PK), but SQL Server requires an explicitly declared unique
    key before another table can reference that pair of columns.
*/
CREATE TABLE Categories (
    CategoryId   INT IDENTITY(1,1) NOT NULL,
    CategoryName VARCHAR(100)      NOT NULL,
    RecordTypeId INT               NOT NULL,
    CONSTRAINT PK_Categories PRIMARY KEY (CategoryId),
    CONSTRAINT UQ_Categories_CategoryId_RecordTypeId UNIQUE (CategoryId, RecordTypeId),
    CONSTRAINT FK_Categories_RecordTypes
        FOREIGN KEY (RecordTypeId) REFERENCES RecordTypes (RecordTypeId)
);
GO

CREATE TABLE FinancialRecords (
    FinancialRecordId INT IDENTITY(1,1) NOT NULL,
    UserId            INT               NOT NULL,
    CategoryId        INT               NOT NULL,
    RecordTypeId      INT               NOT NULL,
    Title             VARCHAR(100)      NOT NULL,
    Amount            DECIMAL(18,2)     NOT NULL,
    Date              DATETIME          NOT NULL,
    Description       VARCHAR(150)      NULL,

    CONSTRAINT PK_FinancialRecords PRIMARY KEY (FinancialRecordId),

    CONSTRAINT CK_FinancialRecords_Amount CHECK (Amount > 0),
    CONSTRAINT CK_FinancialRecords_Date
        CHECK (Date <= GETDATE() AND YEAR(Date) > 1900),

    CONSTRAINT FK_FinancialRecords_Users
        FOREIGN KEY (UserId) REFERENCES Users (UserId),

    CONSTRAINT FK_FinancialRecords_RecordTypes
        FOREIGN KEY (RecordTypeId) REFERENCES RecordTypes (RecordTypeId),

    /*
        Composite foreign key — the point of the whole design.

        Two separate FKs (one on CategoryId, one on RecordTypeId) would each
        be valid on their own and would still allow an Income record pointing
        at the 'Rent' category. Referencing the pair as a unit makes that
        combination impossible at the database level.
    */
    CONSTRAINT FK_FinancialRecords_Categories_Composite
        FOREIGN KEY (CategoryId, RecordTypeId)
        REFERENCES Categories (CategoryId, RecordTypeId)
);
GO
