/*
    FinanceTracker — seed data
    Insert order follows the foreign keys:
    RecordTypes -> Categories -> Users -> FinancialRecords
*/

INSERT INTO RecordTypes (RecordTypeName)
VALUES
    ('Income'),
    ('Expense');
GO

INSERT INTO Categories (CategoryName, RecordTypeId)
VALUES
    ('Salary',        1),
    ('Gift',          1),
    ('Freelance',     1),
    ('Bonus',         1),
    ('Food',          2),
    ('Transport',     2),
    ('Rent',          2),
    ('Health',        2),
    ('Entertainment', 2),
    ('Education',     2);
GO

INSERT INTO Users (FirstName, LastName)
VALUES
    ('Anna',  'Kovalenko'),
    ('Ihor',  'Melnyk'),
    ('Olena', 'Shevchuk');
GO

INSERT INTO FinancialRecords (UserId, CategoryId, RecordTypeId, Title, Amount, Date, Description)
VALUES
    -- Anna — income
    (1,  1, 1, 'Salary',        32000.00, '2026-06-05', 'Monthly salary'),
    (1,  1, 1, 'Salary',        32000.00, '2026-07-05', 'Monthly salary'),
    (1,  1, 1, 'Salary',        34000.00, '2026-08-05', 'Salary after raise'),
    (1,  3, 1, 'Freelance',      8500.00, '2026-06-18', 'Landing page project'),
    (1,  3, 1, 'Freelance',     12000.00, '2026-07-22', 'Small CRM module'),
    (1,  4, 1, 'Bonus',         15000.00, '2026-07-30', 'Quarterly bonus'),
    (1,  2, 1, 'Gift',           3000.00, '2026-08-10', 'Birthday gift'),
    -- Anna — expenses
    (1,  7, 2, 'Rent',          12000.00, '2026-06-01', 'Monthly apartment rent'),
    (1,  7, 2, 'Rent',          12000.00, '2026-07-01', 'Monthly apartment rent'),
    (1,  7, 2, 'Rent',          12500.00, '2026-08-01', 'Rent increased'),
    (1,  5, 2, 'Food',           2400.00, '2026-06-08', 'Groceries'),
    (1,  5, 2, 'Food',           3150.00, '2026-07-09', 'Groceries and delivery'),
    (1,  5, 2, 'Food',           2890.00, '2026-08-12', 'Groceries'),
    (1,  6, 2, 'Transport',       900.00, '2026-06-20', 'Taxi and public transport'),
    (1,  6, 2, 'Transport',      1250.00, '2026-07-15', 'Fuel'),
    (1,  8, 2, 'Health',         4300.00, '2026-07-03', 'Dentist appointment'),
    (1,  9, 2, 'Entertainment',  1800.00, '2026-08-14', 'Cinema and concert'),
    (1, 10, 2, 'Education',     11000.00, '2026-06-25', 'Online course'),
    -- Ihor
    (2,  1, 1, 'Salary',        45000.00, '2026-07-10', 'Monthly salary'),
    (2,  1, 1, 'Salary',        45000.00, '2026-08-10', 'Monthly salary'),
    (2,  4, 1, 'Bonus',         20000.00, '2026-08-15', 'Project delivery bonus'),
    (2,  7, 2, 'Rent',          18000.00, '2026-07-02', 'House rent'),
    (2,  7, 2, 'Rent',          18000.00, '2026-08-02', 'House rent'),
    (2,  5, 2, 'Food',           5600.00, '2026-07-19', 'Groceries for family'),
    (2,  6, 2, 'Transport',      3400.00, '2026-08-06', 'Car maintenance'),
    (2,  9, 2, 'Entertainment', 12000.00, '2026-08-18', 'Weekend trip'),
    -- Olena
    (3,  3, 1, 'Freelance',     22000.00, '2026-06-28', 'Design contract'),
    (3,  3, 1, 'Freelance',     19500.00, '2026-08-03', 'Branding project'),
    (3,  2, 1, 'Gift',           5000.00, '2026-07-07', 'Gift from parents'),
    (3,  5, 2, 'Food',           3200.00, '2026-08-11', 'Groceries'),
    (3,  8, 2, 'Health',        25000.00, '2026-07-25', 'Medical procedure'),
    (3, 10, 2, 'Education',      7500.00, '2026-08-08', 'English lessons'),
    (3,  6, 2, 'Transport',       780.00, '2026-08-16', 'Public transport');
GO

/*
    Constraint check — this row must fail.
    RecordTypeId = 1 (Income) with CategoryId = 7 (Rent, an expense category):
    both foreign keys are individually valid, the composite one is not.

    INSERT INTO FinancialRecords (UserId, CategoryId, RecordTypeId, Title, Amount, Date, Description)
    VALUES (1, 7, 1, 'Wrong pair', 500, '2026-08-20', 'Must violate FK_FinancialRecords_Categories_Composite');
*/
