# Data Design Documentation (DDD)

## 1. Purpose and Scope

This document describes the current data architecture of the Alumni Tracking System, developed for a university Web Programming course.

The application currently uses ASP.NET Core MVC and Web API controllers with PostgreSQL persistence through Entity Framework Core (EF Core). This document distinguishes implemented features from proposed future improvements.

## 2. Current Architecture

The application uses the following data flow:

```text
Browser / API Client
        |
        v
ASP.NET Core MVC / Web API Controllers
        |
        v
AlumniDbContext (Entity Framework Core)
        |
        v
Npgsql PostgreSQL Provider
        |
        v
PostgreSQL Database

MVC controllers return Razor Views for browser requests. API controllers expose HTTP endpoints and return API responses. Both access persistent data through the dependency-injected AlumniDbContext.

3. Implemented Entities

3.1 User

Source file: Models/User.cs

The User entity represents a system user.

Property

C# Type

Description

Id

int

User identifier

Name

string

User's name

Email

string

User's email address

3.2 Announcement

Source file: Models/Announcement.cs

The Announcement entity represents a public announcement.

Property

C# Type

Description

Id

int

Announcement identifier

Title

string

Announcement title

Content

string

Announcement content

CreatedDate

DateTime

Date and time associated with the announcement

The entity initializes CreatedDate with DateTime.Now by default.

4. Database Context and Relationships

Source file: Data/AlumniDbContext.cs

The context exposes the following entity sets:

Users

Announcements

EF Core maps these entities to database tables. The current model does not define a direct foreign-key relationship between users and announcements. Announcement authorship is not represented by a UserId or author navigation property in the current entity model.

Alumni profiles, departments, graduation records, and job experiences are potential future domain entities. They should not be considered implemented until their models and database mappings are added.

5. Data Access and CRUD Operations

The MVC and API controllers use dependency injection to access AlumniDbContext. Database changes are persisted using EF Core operations, including SaveChangesAsync().

Implemented data flows include:

User management through MVC pages and the users REST API.

Public announcement listing and details.

Announcement administration, including create, edit, and delete operations.

Database-backed reads and writes through EF Core.

The API documentation is available at /api/swagger while the application is running.

6. Database Configuration

PostgreSQL is configured through Docker Compose.

Current database infrastructure:

Setting

Value

Compose service

db

Container name

alumni-postgres

Image

postgres:17-alpine

Database port

5432

Named volume

alumni_postgres_data

The Compose configuration starts the database service; the ASP.NET Core application runs separately on the host during local development.

Environment Variables

Database configuration is supplied through environment variables, including:

POSTGRES_HOST

POSTGRES_PORT

POSTGRES_DB

POSTGRES_USER

POSTGRES_PASSWORD

The local .env file is ignored by Git because it may contain credentials. .env.example documents the expected configuration variables. Real passwords and secrets must not be committed.

7. Migrations and Database Initialization

EF Core migration files are stored in the Migrations/ directory. The project contains an initial migration named InitialCreate.

The migration history is tracked by EF Core in __EFMigrationsHistory.

To apply pending migrations after starting PostgreSQL and configuring the environment, run:

dotnet ef database update

Seed Data

Source files:

Data/Factories/UserFactory.cs

Data/Factories/AnnouncementFactory.cs

Data/Seeders/DatabaseSeeder.cs

The factories provide initial user and announcement data. The database seeder checks whether each corresponding table contains records before inserting its seed data. This prevents the initial records from being inserted again every time the application starts.

8. Persistence and Forgetting Test

A database persistence test was performed during development:

Create a test record in PostgreSQL.

Stop the Compose services using docker compose down.

Recreate the services using docker compose up -d.

Query the record again.

Verify that the record still exists.

The test record remained available after container recreation. This demonstrates that the PostgreSQL named volume preserves database files independently of the container lifecycle.

Important: docker compose down does not normally remove named volumes. Avoid docker compose down -v unless you intentionally want to remove the volume and its stored database data.

The manually created forgetting_test table was used for this persistence test; it is not one of the application's two EF Core entity sets.

9. Project Structure

alumni/
├── Controllers/             # MVC and Web API controllers
├── Data/
│   ├── AlumniDbContext.cs    # EF Core database context
│   ├── Factories/            # Initial data factories
│   └── Seeders/              # Database seed logic
├── Migrations/               # EF Core migrations and snapshot
├── Models/                   # User and Announcement entities
├── Views/                    # Razor Views
├── wwwroot/                  # Static assets
├── Program.cs                # Application configuration and DI
├── Alumni.csproj             # .NET dependencies
├── docker-compose.yml        # PostgreSQL service and volume
├── .env.example              # Example environment configuration
├── README.md
└── ddd.md

10. Current Limitations and Future Work

The following items are not represented as implemented in the current data model unless subsequently added:

Alumni profile management.

Department and graduation records.

Job experience tracking.

Authentication, authorization, and role management.

Foreign-key relationships between users and announcements.

Additional indexes, constraints, and advanced search features.

These features may be added in future development phases after their requirements and data relationships are defined.

11. Related Documentation

See README.md for project setup, technology stack, routes, and local execution instructions.
