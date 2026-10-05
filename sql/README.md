# SQL

T-SQL scripts for the FinanceTracker database (SQL Server), plus practice
queries written against the Northwind sample database.

## FinanceTracker

Run the scripts in order — the numbering is the dependency order.

| File | Contents |
|---|---|
| `01_schema.sql` | Tables, constraints, composite foreign key |
| `02_seed_data.sql` | Record types, categories, users, 33 financial records |
| `03_views.sql` | `vw_RecordsFull`, `vw_Expenses`, `vw_CategoryTotals`, `vw_MonthlyBalance` |
| `04_functions.sql` | Scalar and inline table-valued functions |
| `05_procedures.sql` | Add and delete procedures with validation |
| `06_triggers.sql` | Audit table and INSERT/UPDATE/DELETE triggers |
| `07_indexes.sql` | 50k-row generator, index experiment, execution plan results |

### Schema

```
Users ─────┐
           ├──< FinancialRecords >── RecordTypes
Categories ┘
```

A category belongs to exactly one record type: `Rent` is always an expense,
`Salary` is always an income. `FinancialRecords` keeps its own `RecordTypeId`
because the income/expense distinction is a first-class concept in the
application, not a side effect of the category.

Two independent foreign keys would each be satisfied by an income record
pointing at `Rent`. A composite foreign key on `(CategoryId, RecordTypeId)`
referencing a matching unique key on `Categories` makes that combination
impossible at the database level.

## practice/

Exercises against the Northwind sample database, ordered by topic: filtering,
aggregation, joins, self-joins, `UNION`, subqueries. Kept separate — they do
not belong to the FinanceTracker schema.
