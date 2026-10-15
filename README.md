# FinanceTracker

Personal finance tracker for managing income, expenses and monthly balance, built with C# and SQL Server.

> 🚧 Work in progress — ASP.NET Core Web API with in-memory storage; moving to SQL Server with Entity Framework Core.

## Tech stack

- C# / .NET
- SQL Server (T-SQL)
- xUnit
- ASP.NET Core Web API

## Features

- Income and expense records with categories
- Search by category and date range
- Totals and balance calculation
- Top expenses report
- Large expense notifications via events
- JSON persistence

## Tests

Unit tests are in `FinanceTracker.Tests`. Run them with:

```bash
dotnet test
```

## Database

The `sql/` folder contains the SQL Server schema and scripts — see [sql/README.md](sql/README.md).

Highlights:
- Composite foreign key that prevents an income record from referencing an expense category
- Audit log implemented with triggers
- Index experiments with measured execution plans on 50k rows

## Project structure

```
FinanceTracker/         Console application
FinanceTracker.Tests/   Unit tests
sql/                    Database schema, views, functions, procedures, triggers
```

## Roadmap

- [x] ASP.NET Core Web API
- [ ] Entity Framework Core with migrations
- [ ] React frontend
- [ ] Docker and cloud deployment