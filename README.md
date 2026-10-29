# FinanceTracker

REST API for tracking personal income and expenses, built with ASP.NET Core and SQL Server.

> Backend is complete. A React frontend is next on the roadmap.

## Tech stack

- C# / .NET 10
- ASP.NET Core Web API (controllers)
- Entity Framework Core 10, code-first migrations
- SQL Server
- xUnit v3 — unit and integration tests

## Features

- Create, read, update and delete income and expense records
- Search by category and by date range
- Income, expense and balance totals
- Top expenses report
- Categories endpoint for populating frontend dropdowns
- Validation with consistent `ProblemDetails` error responses

## Architecture

```
HTTP request
    │
    ▼
Controllers ── DTOs (request / response) and mapping
    │
    ▼
IFinanceService
    │
    ▼
EfFinanceService ── entity ↔ domain model mapping
    │
    ▼
EF Core ──▶ SQL Server
```

Controllers depend only on the `IFinanceService` interface. The original in-memory implementation was replaced with the database one by changing a single line of DI registration, without touching the controllers.

Each layer has its own model: **DTOs** for the HTTP contract, **domain models** (`IncomeRecord`, `ExpenseRecord`) for application logic, and **entities** for the database schema.

## Engineering highlights

**Database integrity.** A composite foreign key on `(CategoryId, RecordTypeId)` makes it impossible at the database level for an income record to reference an expense category. The service also validates this first, so the client gets a clear `400` instead of a constraint violation.

**Queries run in the database.** Totals use `SUM` with `CASE WHEN` generated from LINQ; nothing is loaded into memory to be counted. Date searches use ranges instead of wrapping the column in a function, so the queries stay index-friendly.

**Error handling.** A global `IExceptionHandler` turns exceptions into `ProblemDetails` responses: `400` for invalid input, `500` for unexpected failures. Internal exception details are logged but never returned to the client. Controllers contain no `try/catch`.

**Request cancellation.** `CancellationToken` flows from the controllers through the service to every EF Core call, so the database query stops when the client disconnects.

**Structured logging.** Data changes are logged with message templates (`{RecordId}`, `{Amount}`), and unhandled errors are logged with the same `traceId` the client receives.

**Configuration.** Settings are bound with the Options pattern and validated on startup — invalid configuration stops the application immediately instead of failing later at runtime.

**Integration tests.** Tests run the real API in memory against a dedicated SQL Server database that is recreated from migrations on each run. A guard refuses to reset any database whose name does not end with `_Tests`.

## API

### Records

| Method | Endpoint | Description |
|---|---|---|
| GET | `/records` | All records |
| GET | `/records/{id}` | One record |
| GET | `/records/by-category?category=Food` | Records in a category |
| GET | `/records/by-date?from=2026-10-01&to=2026-10-31` | Records in a date range |
| GET | `/records/top-expenses?count=3` | Largest expenses |
| GET | `/records/summary` | Income, expense and balance totals |
| POST | `/records` | Create a record → `201` |
| PUT | `/records/{id}` | Update a record → `204` |
| DELETE | `/records/{id}` | Delete a record → `204` |

### Categories

| Method | Endpoint | Description |
|---|---|---|
| GET | `/categories?recordType=Income` | Categories, optionally filtered by type |
| GET | `/categories/{id}` | One category |

Example request:

```http
POST /records
Content-Type: application/json

{
  "title": "Groceries",
  "amount": 1500,
  "date": "2026-10-01",
  "category": "Food",
  "recordType": "Expense"
}
```

Ready-to-run requests are in `FinanceTracker.Api/FinanceTracker.Api.http`.

## Getting started

**Prerequisites:** .NET 10 SDK, a local SQL Server instance.

1. Set the connection string with User Secrets — it is never stored in the repository:

   ```bash
   cd FinanceTracker.Api
   dotnet user-secrets set "ConnectionStrings:FinanceDb" "Server=localhost;Database=FinanceTrackerEf;Trusted_Connection=True;TrustServerCertificate=True"
   ```

2. Create the database. Migrations also seed record types and categories:

   ```bash
   dotnet ef database update
   ```

3. Run the API:

   ```bash
   dotnet run
   ```

### Configuration

`appsettings.json`:

```json
"Finance": {
  "LargeExpenseLimit": 10000,
  "DemoUserId": 1
}
```

Expenses at or above `LargeExpenseLimit` are logged as warnings.

### Tests

```bash
dotnet test
```

Integration tests need a local SQL Server and use a separate database, `FinanceTrackerEf_Tests`. Your development database is not touched.

## Project structure

```
FinanceTracker/            Domain models, IFinanceService, validation, in-memory service
FinanceTracker.Api/        Web API: controllers, DTOs, EF Core, migrations
FinanceTracker.Tests/      Unit tests for the in-memory service
FinanceTracker.Api.Tests/  Integration tests for the API
sql/                       Hand-written T-SQL: schema, views, functions, procedures, triggers
```

The `sql/` folder predates the EF Core schema and documents the original database design — see [sql/README.md](sql/README.md).

## Roadmap

- [x] Console application with unit tests
- [x] SQL Server schema in T-SQL
- [x] ASP.NET Core Web API
- [x] Entity Framework Core with migrations
- [x] Error handling, logging, configuration
- [x] Integration tests
- [ ] React frontend
- [ ] Authentication
- [ ] Docker and cloud deployment
