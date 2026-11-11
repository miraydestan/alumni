# Alumni Tracking System

## About the Project

The **Alumni Tracking System** is a web application developed for a university Web Programming course. It provides user management, public announcements, announcement administration, job postings and applications, a REST API, structured request/error logging, and custom 404/5xx error handling. PostgreSQL is used for persistent storage through Entity Framework Core.

## Technology Stack

- **Framework:** ASP.NET Core 9 (`net9.0`)
- **Architecture:** MVC with Razor Views, Web API controllers, and custom middleware
- **Database:** PostgreSQL 17 (`postgres:17-alpine`)
- **ORM and provider:** Entity Framework Core and Npgsql
- **API documentation:** Swagger / OpenAPI
- **Logging:** Built-in `ILogger` Console logging, `RequestLoggingMiddleware`, and `FileErrorLoggerProvider` (`logs/error.logs`)
- **UI:** Razor, HTML, CSS, Bootstrap 5
- **Infrastructure:** Docker Compose (`db` PostgreSQL service and `app` container service)
- **Version control:** Git and GitHub

## Architecture and Data Flow

Incoming requests pass through the ASP.NET Core middleware pipeline before reaching MVC or Web API controllers and PostgreSQL via `AlumniDbContext`.

```text
Browser / API Client
        |
        v
Swagger / Static Files / HTTPS Redirection
        |
        v
RequestLoggingMiddleware        --> Logs "time method route restype status duration" to Console
        |
        v
ServerErrorGuardianMiddleware   --> Catches unhandled exceptions / 5xx, logs error & returns safe JSON 500
        |
        v
NotFoundGuardianMiddleware      --> Returns JSON 404 with reason for API / JSON requests
        |
        v
MVC 404 Re-Execution (non-API)  --> Re-executes /Home/NotFound (Views/Shared/NotFound.cshtml) on 404
        |
        v
ASP.NET Core Routing & Controllers
        |
        v
AlumniDbContext (EF Core + Npgsql) --> PostgreSQL Database

Server Errors (ILogger LogLevel >= Error) --> Console + logs/error.logs (FileErrorLoggerProvider)
```

- **MVC requests:** Controllers return Razor Views for browser pages. Invalid non-API URLs or MVC `NotFound()` results render the custom `"404 – Page Not Found"` page (`Views/Shared/NotFound.cshtml`) with HTTP status `404`.
- **API requests:** `[ApiController]` endpoints return JSON responses. Unmatched `/api/*` endpoints (or requests with `Accept: application/json`) receive structured JSON `404` responses from `NotFoundGuardianMiddleware`, while matched `[ApiController]` actions returning `NotFound()` continue returning standard `ProblemDetails` JSON.

## Current Features

- **User management:** MVC CRUD pages (`/User`, `/users`) and a REST API (`/api/users`).
- **Announcements:** Public announcement list and details pages (`/Announcement`, `/Announcement/Details/{id}`).
- **Announcement administration:** Create, view, edit, and delete announcements (`/AnnouncementAdmin`).
- **Job postings and applications:** MVC views (`/JobPosting`, `/jobs`) and REST API endpoints (`/api/users/{id}/jobs`, `/api/jobs/{id}/applications`, `/api/applications`), with fail-closed `401 Unauthorized` protection on application creation endpoints until authentication is implemented.
- **Health and utility endpoints:** `/api/health`, `/hello`, `/hello/miray`, and `/sum/{number1}/{number2}`.
- **Swagger UI:** `/api/swagger` (with `/swagger` redirecting to `/api/swagger`).
- **Request Logger Middleware (`Middleware/RequestLoggingMiddleware.cs`):** Measures request duration with `Stopwatch` and logs every completed MVC and Web API request once to the console in the format `HH:mm:ss METHOD /route restype STATUS DURATIONms`.
- **5xx Guardian Middleware (`Middleware/ServerErrorGuardianMiddleware.cs`):** Catches unhandled exceptions and unstarted `5xx` responses, logs full error details server-side, and returns a safe JSON `500` response (`statusCode`, `title`, `reason`, `path`) without exposing stack traces or credentials.
- **404 Guardian Middleware (`Middleware/NotFoundGuardianMiddleware.cs`):** Intercepts unstarted `404` responses on API requests (or `Accept: application/json`) and returns a structured JSON error payload (`statusCode`, `title`, `reason`, `path`).
- **Generic MVC 404 Page (`Views/Shared/NotFound.cshtml` via `HomeController.PageNotFound`):** Displays a Bootstrap-styled `"404 – Page Not Found"` view with a **Back to Home** button (`href="/"`) for invalid browser/MVC requests while preserving JSON `404` responses for API clients.
- **File Error Logging (`Logging/FileErrorLoggerProvider.cs`):** Hooks into `ILogger` (`ILoggerProvider`) to automatically create `logs/` and persist server-side error logs (`LogLevel.Error` / `LogLevel.Critical`) into `logs/error.logs` with thread-safe appends and credential sanitization, alongside existing console logging.
- **Database migrations and seeding:** EF Core migrations (`InitialCreate`, `AddJobPostings`, `AddApplications`) in `Migrations/`, and `DatabaseSeeder` inserting initial users, announcements, and job postings when tables are empty.

The active CRUD controllers use dependency-injected `AlumniDbContext` and persist changes to PostgreSQL. `Models/UserStore.cs` and `Models/AnnouncementStore.cs`, if still present in the repository, should not be described as the active storage mechanism for these controllers.

## Database and Persistence

Docker Compose defines the `db` (`alumni-postgres`) PostgreSQL service and the `app` (`alumni-app`) application service. The named volume `alumni_postgres_data` stores PostgreSQL data outside the container, so removing and recreating the container with `docker compose down` followed by `docker compose up -d` preserves the data.

> **Important:** Do not use `docker compose down -v` unless you intentionally want to remove the named volume and delete the database data.

### Environment Configuration

The application reads database settings from environment variables (including loading a local `.env` file in `Program.cs`). Expected variables:

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

`Program.cs` also runs `await db.Database.MigrateAsync();` on startup before seeding data. If the EF command is unavailable, check that the project's EF Core tooling is installed and configured before proceeding. Do not delete migrations to resolve a tooling issue.

### Persistence Test

A persistence test was performed by creating a record, stopping and recreating the database container with `docker compose down` and `docker compose up -d`, then checking the record again. The record remained available, confirming that the named volume persisted the data across container recreation.

## Prerequisites

- .NET 9 SDK
- Docker Desktop with Docker Compose
- EF Core command-line tooling for manual migration commands

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

4. Apply migrations (optional if letting startup `MigrateAsync()` apply them):

   ```powershell
   dotnet ef database update
   ```

5. Run the ASP.NET Core application:

   ```powershell
   dotnet run --launch-profile http
   ```

   Alternatively, build and run the full Docker Compose stack (PostgreSQL on port `5432` and the app container on port `8080`):

   ```powershell
   docker compose up -d --build
   ```

Use the URL printed by `dotnet run` or the configured launch profile. The local `http` launch profile runs at `http://localhost:5067` (and `https` listens on `https://localhost:7160` and `http://localhost:5067`); confirm the actual URL in the terminal or `Properties/launchSettings.json`.

## Main Routes

Assuming the application is running locally at `http://localhost:5067`:

### MVC Routes (Browser)

| Route | Purpose |
|---|---|
| `/` | Home page (`HomeController.Index`) |
| `/about` | About page (`HomeController.About`) |
| `/Home/NotFound` | Custom `"404 – Page Not Found"` page (`HomeController.PageNotFound`) |
| `/User` (and `/users`) | User management pages |
| `/Announcement` | Public announcements |
| `/Announcement/Details/{id}` | Announcement details |
| `/AnnouncementAdmin` | Announcement administration |
| `/JobPosting` (and `/jobs`) | Job postings list, details, and applications |

### Web API & Utility Routes

| Route | Methods | Purpose |
|---|---|---|
| `/api/users` | `GET`, `POST` | List or create users |
| `/api/users/{id}` | `GET`, `PUT`, `PATCH`, `DELETE` | Read, update, patch, or delete a user |
| `/api/users/{id}/jobs` | `GET`, `POST` | List or create job postings for a user |
| `/api/users/{id}/applications` | `GET` | List applications submitted by a user |
| `/api/jobs/{id}/applications` | `GET`, `POST` | List or submit applications for a job posting (`POST` returns `401` without auth) |
| `/api/jobs/{id}/applications/{applicationId}` | `GET` | Read a specific application for a job posting |
| `/api/applications` | `GET`, `POST` | List or submit applications (`POST` returns `401` without auth) |
| `/api/applications/{id}` | `GET` | Read a specific application |
| `/api/health` | `GET`, `POST` | Health check (`{"status":"ok"}`) |
| `/api/swagger` | `GET` | Swagger UI (open `/swagger` to redirect here) |
| `/hello`, `/hello/miray`, `/sum/{number1}/{number2}` | `GET` | Utility endpoints in `AlumniController` |

## Error Handling, 404 Behavior & Testing

### 1. API 404 Responses (`NotFoundGuardianMiddleware`)

Unmatched `/api/*` requests (or requests with `Accept: application/json`) return JSON from `NotFoundGuardianMiddleware`:

```json
{
  "statusCode": 404,
  "title": "Not Found",
  "reason": "The requested resource or endpoint could not be found.",
  "path": "/api/invalid-endpoint"
}
```

Matched `[ApiController]` endpoints returning `NotFound()` (e.g., `GET /api/users/99999`) preserve ASP.NET Core's `ProblemDetails` JSON (`application/problem+json`).

**Example Postman tests (`404` JSON):**
- `GET http://localhost:5067/api/invalid-endpoint` → `404 Not Found` (`NotFoundGuardianMiddleware` JSON)
- `GET http://localhost:5067/api/jobs` → `404 Not Found` (`NotFoundGuardianMiddleware` JSON)
- `GET http://localhost:5067/api/users/99999` → `404 Not Found` (`ProblemDetails` JSON)
- `GET http://localhost:5067/nonexistent-page` with header `Accept: application/json` → `404 Not Found` (`NotFoundGuardianMiddleware` JSON)

### 2. Custom MVC 404 Page (`Views/Shared/NotFound.cshtml`)

When a browser navigates to a non-existing non-API URL or an MVC controller returns `NotFound()`, `UseStatusCodePagesWithReExecute("/Home/NotFound")` renders `Views/Shared/NotFound.cshtml` (`404 – Page Not Found` with a **Back to Home** link to `/`) while keeping HTTP status `404`.

**Example Browser tests (`404` HTML):**
- `http://localhost:5067/nonexistent-page`
- `http://localhost:5067/profile/settings`
- `http://localhost:5067/User/Details/99999`
- `http://localhost:5067/Announcement/Details/99999`

### 3. 5xx Guardian Middleware & File Error Logging (`logs/error.logs`)

Unhandled exceptions return a safe JSON `500` payload (`statusCode`, `title`, `reason`, `path`) without leaking exception messages, stack traces, or database credentials, while the full exception is logged to both the Console and `logs/error.logs`.

**Example Postman test (`500` JSON + `logs/error.logs`):**
- `POST http://localhost:5067/api/users` with `Content-Type: application/json` and an already-existing seeded user `"id"` (for example, `2`):
  ```json
  {
    "id": 2,
    "name": "Duplicate Test",
    "email": "dup@example.com"
  }
  ```
- Inspect the persisted log file in PowerShell:
  ```powershell
  Get-Content .\logs\error.logs
  ```

### 4. Request Console Logging & Normal Endpoints

Each request logs one line to the terminal via `RequestLoggingMiddleware` (`time method route restype status duration`), for example:
`10:25:44 GET /api/users user 200 36ms`

**Example normal checks (`200 OK`):**
- Browser: `http://localhost:5067/`, `http://localhost:5067/about`, `http://localhost:5067/User`
- Postman: `GET http://localhost:5067/api/health`, `GET http://localhost:5067/api/users`

## Project Structure

```text
alumni/
├── Controllers/
│   ├── AlumniController.cs
│   ├── AnnouncementAdminController.cs
│   ├── AnnouncementController.cs
│   ├── ApiApplicationController.cs
│   ├── ApiJobController.cs
│   ├── ApiUserController.cs
│   ├── HealthController.cs
│   ├── HomeController.cs
│   ├── JobPostingController.cs
│   └── UserController.cs
├── Data/
│   ├── AlumniDbContext.cs
│   ├── Factories/           # Seed data factories (Users, Announcements, JobPostings)
│   └── Seeders/             # Database seed logic
├── Logging/
│   └── FileErrorLoggerProvider.cs
├── Middleware/
│   ├── NotFoundGuardianMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └── ServerErrorGuardianMiddleware.cs
├── Migrations/              # EF Core migration files and model snapshot
├── Models/                  # User, Announcement, JobPosting, Application, and request DTOs
├── Views/
│   └── Shared/
│       ├── _Layout.cshtml
│       └── NotFound.cshtml  # Generic MVC 404 page
├── wwwroot/                 # Static assets
├── logs/                    # Runtime-created directory containing error.logs
├── Alumni.csproj            # .NET project and package references
├── Alumni.http              # Sample HTTP requests
├── Program.cs               # DI, logging, middleware pipeline, and routing
├── docker-compose.yml       # PostgreSQL and app services
├── Dockerfile               # Multi-stage container build for application service
├── .env.example             # Example environment variable names and values
├── ddd.md                   # Data Design Documentation
└── README.md
```

## Implemented and Planned Scope

**Implemented (current project):** ASP.NET Core MVC and Web API, `User`, `Announcement`, `JobPosting`, and `Application` entities, PostgreSQL connection, EF Core migrations and database seeding, MVC and REST API flows (with fail-closed `401` protection on application creation endpoints), `RequestLoggingMiddleware`, `ServerErrorGuardianMiddleware`, `NotFoundGuardianMiddleware`, generic MVC 404 page (`Views/Shared/NotFound.cshtml`), file-based error logging (`logs/error.logs`), Docker Compose (`db` and `app`), and Swagger UI.

**Planned / not yet implemented:** alumni profiles, departments, graduation details, job experience, full authentication/authorization (`AddAuthentication` / user login & claims), and additional search/filtering or dashboard features. These should be marked implemented only after their models, routes, and persistence are present in the codebase.

## Related Documentation

See [ddd.md](ddd.md) for the current data model, database design, `JobPosting` business rules, persistence configuration, and planned domain entities.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
