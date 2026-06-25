# SampleAPI

## Build and Run Instructions

### Prerequisites

* .NET 8 SDK
* SQL Server (local or remote instance)
* Visual Studio or another .NET-compatible IDE

### Setup

#### 1. Configure the Connection String

Open `SampleAPI/appsettings.json` and update the database connection string:

```json
{
  "ConnectionStrings": {
    "CustomerOrder": "Server=localhost;Database=CustomerOrderDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

#### 2. Create and Apply Database Migrations

Run the following commands from the Package Manager Console:

```powershell
# Create the initial migration
Add-Migration InitialMigration -Project SampleAPI.Database -StartupProject SampleAPI

# Apply the migration to the database
Update-Database -Project SampleAPI.Database -StartupProject SampleAPI
```

---

## Assumptions

* A primary key was created for the database entities.
* No other fields were assumed to be unique unless explicitly specified.
* The `GET /orders` endpoint was assumed to return all order properties.

---

## Design Decisions

### Architecture

* Entity Framework Core Code-First approach was used to generate the database schema.
* Entity Framework Core was used as the data access layer:

  * `DbContext` acts as the repository.
  * `DbSet<T>` provides access to entity collections.
* The solution is split into four projects:

  * API project
  * Database project
  * API test project
  * Database test project

This separation helps maintain clear boundaries between application layers and improves maintainability.

### Data Integrity

* Duplicate order prevention is enforced at the database level through unique constraints.
* An application-level validation check was considered but rejected due to the potential for race conditions.
* Database-level enforcement provides stronger consistency guarantees.

### Error Handling

* Global exception handling was not implemented due to time constraints.
* Exceptions are currently handled at the service/controller level where required.

---

## Future Improvements

### Reliability & Maintainability

* Implement global exception handling middleware.
* Increase automated test coverage.
* Refactor `CustomerOrderService` to improve efficiency and reduce complexity.

### Scalability

* Add pagination to `GET /orders` to prevent performance issues as data volume grows.
* Review and add database indexes where appropriate.

### Operational Readiness

* Add API versioning.
* Implement health checks.
* Add metrics, monitoring, and observability.
* Introduce structured logging and tracing.
