# Alumni Tracking System

## About the Project

The **Alumni Tracking System** is a web application developed for a university Web Programming course. It currently provides user management, public announcements, announcement administration, and a REST API. PostgreSQL is used for persistent storage through Entity Framework Core.

## Technology Stack

- **Framework:** ASP.NET Core 9 (`net9.0`)
- **Architecture:** MVC with Razor Views and Web API controllers
- **Database:** PostgreSQL 17 (`postgres:17-alpine`)
- **ORM and provider:** Entity Framework Core and Npgsql
- **API documentation:** Swagger / OpenAPI
- **UI:** Razor, HTML, CSS, Bootstrap
- **Infrastructure:** Docker Compose (database service only)
- **Version control:** Git and GitHub

## Architecture and Data Flow

For MVC pages, the browser sends a request to an MVC controller, which uses `AlumniDbContext` to query or update the database. The controller then returns a Razor View. For API requests, the API controller processes the request and returns an HTTP response, commonly JSON.

```text
Browser / API Client
        |
        v
ASP.NET Core Routing
        |
        v
MVC / API Controller
        |
        v
AlumniDbContext (EF Core)
        |
        v
Npgsql Provider
        |
        v
PostgreSQL Database
```

## Current Features

- **User management:** MVC CRUD pages and a REST API for users.
- **Announcements:** Public announcement list and details pages.
- **Announcement administration:** Create, view, edit, and delete announcements.
- **Health endpoint:** `/api/health`.
- **Swagger UI:** `/api/swagger`.
- **Database migrations:** EF Core migration files are maintained in `Migrations/`.
- **Seed data:** `DatabaseSeeder` inserts initial users and announcements when their respective tables are empty.

The active CRUD controllers use dependency-injected `AlumniDbContext` and persist changes to PostgreSQL. `Models/UserStore.cs` and `Models/AnnouncementStore.cs`, if still present in the repository, should not be described as the active storage mechanism for these controllers.

## Database and Persistence

Docker Compose starts the PostgreSQL database service. The named volume `alumni_postgres_data` stores PostgreSQL data outside the container, so removing and recreating the container with `docker compose down` followed by `docker compose up -d` preserves the data.

> **Important:** Do not use `docker compose down -v` unless you intentionally want to remove the named volume and delete the database data.

### Environment Configuration

The application reads database settings from environment variables. A local `.env` file can provide values such as:

- `POSTGRES_HOST`
- `POSTGRES_PORT`
- `POSTGRES_DB`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`

The `.env` file contains local configuration and must not be committed. `.env.example` documents the expected variable names and can be used as a template. Replace example credentials with your own local values. Never publish real passwords or secrets.

### Apply Migrations

After configuring the environment and starting PostgreSQL, apply the EF Core migrations:

```powershell
dotnet ef database update
```

If the EF command is unavailable, check that the project's EF Core tooling is installed and configured before proceeding. Do not delete migrations to resolve a tooling issue.

### Persistence Test

A persistence test was performed by creating a record, stopping and recreating the database container with `docker compose down` and `docker compose up -d`, then checking the record again. The record remained available, confirming that the named volume persisted the data across container recreation.

## Prerequisites

- .NET 9 SDK
- Docker Desktop with Docker Compose
- EF Core command-line tooling for migration commands

## Run Locally

1. Clone the repository and enter its directory:

   ```powershell
   git clone https://github.com/miraydestan/alumni.git
   cd alumni
   ```

2. Configure your local environment. Create `.env` from `.env.example` and set local database credentials. Do not commit `.env`.

3. Start PostgreSQL:

   ```powershell
   docker compose up -d db
   docker compose ps
   ```

4. Apply migrations:

   ```powershell
   dotnet ef database update
   ```

5. Run the ASP.NET Core application:

   ```powershell
   dotnet run
   ```

Use the URL printed by `dotnet run` or the configured launch profile. The development URL has commonly been `http://localhost:5067` in this project; confirm the actual URL in the terminal or `Properties/launchSettings.json`.

## Main Routes

Assuming the application is running at `http://localhost:5067`:

| Route | Purpose |
|---|---|
| `/` | Home page |
| `/User` | User management pages |
| `/Announcement` | Public announcements |
| `/Announcement/Details/{id}` | Announcement details |
| `/AnnouncementAdmin` | Announcement administration |
| `/api/users` | Users REST API |
| `/api/health` | Health endpoint |
| `/api/swagger` | Swagger UI |

The `ApiUserController` provides the user API operations implemented in the current code, including `GET`, `POST`, `PUT`, `PATCH`, and `DELETE`. Check Swagger for the current request schemas and responses.

## Project Structure

```text
alumni/
├── Controllers/       # MVC and Web API controllers
├── Data/
│   ├── AlumniDbContext.cs
│   ├── Factories/     # Factory methods for seed entities
│   └── Seeders/       # Database seed logic
├── Migrations/        # EF Core migration files and model snapshot
├── Models/            # User and Announcement entities (and any legacy files)
├── Views/             # Razor Views
├── wwwroot/           # Static assets
├── Alumni.csproj      # .NET project and package references
├── Program.cs         # Dependency injection, configuration, middleware and routing
├── docker-compose.yml # PostgreSQL service and named volume
├── .env.example       # Example environment variable names and values
├── ddd.md             # Data Design Documentation
└── README.md
```

## Implemented and Planned Scope

**Implemented (current project):** ASP.NET Core MVC and Web API, user and announcement entities, PostgreSQL connection, EF Core migrations, database seeding, user and announcement CRUD flows, Docker Compose database service, and Swagger UI.

**Planned / not yet confirmed as implemented:** alumni profiles, departments, graduation details, job experience, authentication and authorization, and additional search/filtering or dashboard features. These should be marked implemented only after their models, routes, and persistence are present in the codebase.

## Related Documentation

See [ddd.md](ddd.md) for the current data model, database design, persistence configuration, and planned domain entities.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
