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

The database design for the `job_postings` table and its one-to-many relationship with `users` is specified in detail in the section below.

## Database Design: Job Postings Table (`job_postings`)

This section documents the database schema design and API specification for the `job_postings` table. As distinguishing implemented features from planned improvements is a primary objective of this document, note that this section defines the database design specification for upcoming implementation and is not yet implemented in the active C# entity models, DbContext, migrations, or controllers.

### 1. Table Definition

Table name: `job_postings`

Columns:

| Column Name | Data Type | Nullable | Key / Constraint | Description |
|---|---|---|---|---|
| `id` | `integer` | No (`NOT NULL`) | Primary Key | Unique identifier for each job posting |
| `user_id` | `integer` | No (`NOT NULL`) | Foreign Key (`users.id`) | References the user who created/owns the posting; required |
| `title` | `text` | No (`NOT NULL`) | None | Required text field specifying the job title |
| `company` | `text` | No (`NOT NULL`) | None | Required text field specifying the company name |

SQL Schema Definition:

```sql
CREATE TABLE job_postings (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INT NOT NULL,
    title TEXT NOT NULL,
    company TEXT NOT NULL,
    CONSTRAINT fk_job_postings_users
        FOREIGN KEY (user_id)
        REFERENCES "Users" ("Id")
        ON DELETE RESTRICT
);
```

### 2. One-to-Many Relationship

The `job_postings` table establishes a one-to-many (1:N) relationship between users and job postings:
- **One user can create many job postings:** A single user can create and own multiple job postings.
- **Each job posting belongs to exactly one user:** Every job posting record is strictly associated with a single user owner.
- **Ownership identification:** The `user_id` column identifies the owner of the posting.

### 3. Foreign Key and Deletion Rule

- **Foreign Key:** `job_postings.user_id` references `users.id` (mapped to `"Users"("Id")` in PostgreSQL / EF Core).
- **Deletion Rule:** `RESTRICT` (or `NO ACTION`).
- **Explanation and Rationale:**
  - The deletion rule is configured as `RESTRICT` / `NO ACTION` so that a user with existing job postings cannot be deleted accidentally.
  - If a DELETE operation is executed against a record in `users` while that user still has one or more associated postings in `job_postings`, the database rejects the deletion and raises a foreign key violation error.
  - This rule is chosen over alternatives for critical data integrity reasons:
    - **Prevents accidental cascading data loss:** Unlike `CASCADE`, which would silently remove all job postings when a user is deleted, `RESTRICT` prevents accidental loss of job postings that may still be active or needed for historical tracking.
    - **Prevents orphaned records:** Unlike `SET NULL`, which is prohibited because `user_id` is a required (`NOT NULL`) field, `RESTRICT` ensures that every job posting always has a valid owner.
    - **Intentional account lifecycle:** To delete a user, an administrator or client must first explicitly reassign or delete their associated job postings, ensuring safe and deliberate data management.

### 4. API Endpoint: `POST /api/users/{id}/jobs`

The API provides an endpoint to create a job posting for a specified user:

- **HTTP Method and Route:** `POST /api/users/{id}/jobs`
- **Route Parameter:** `{id}` is the user ID from the URL, indicating the owner of the posting.
- **Request Body:** JSON payload containing `title` and `company`:
  ```json
  {
    "title": "Junior Developer",
    "company": "Acme"
  }
  ```
- **Ownership Determination:**
  - The server must determine ownership from the URL (`{id}`), not from a `user_id` supplied in the request body.
  - If a client supplies a `user_id` in the JSON request body, the server ignores or rejects it. The posting is always assigned the owner ID specified by `{id}` in the URL. This prevents clients from spoofing ownership or associating postings with unintended users.
- **Expected HTTP Responses:**
  - `404 Not Found`: If the user with ID `{id}` does not exist in the database. The server verifies that the user exists in `users` before creating the posting; if no matching user is found, it returns `404 Not Found`.
  - `201 Created`: If creation succeeds. The server persists the new record in `job_postings` and returns `201 Created` with the newly created posting, including its `id` and `user_id` in the response body.
  - `400 Bad Request`: If the request payload is malformed or missing required fields (`title`, `company`).

### 5. Example Request

`POST /api/users/1/jobs`

```json
{
  "title": "Junior Developer",
  "company": "Acme"
}
```

**Explanation:**
The resulting posting belongs to user `1` because the server extracts the owner ID directly from the URL route parameter (`{id} = 1`) rather than accepting a `user_id` from the request body.

**Example Response (`201 Created`):**

```json
{
  "id": 1,
  "user_id": 1,
  "title": "Junior Developer",
  "company": "Acme"
}
```

## Database Design: Applications Table (`applications`)

This section documents the database schema design and API specification for the `applications` table, which serves as the join table implementing the many-to-many relationship between users (applicants) and job postings.

### 1. Table Definition

Table name: `applications`

Columns:

| Column Name | Data Type | Nullable | Key / Constraint | Description |
|---|---|---|---|---|
| `id` | `integer` | No (`NOT NULL`) | Primary Key | Unique identifier for each application record |
| `user_id` | `integer` | No (`NOT NULL`) | Foreign Key (`users.id`) | References the user who submitted the application; required |
| `job_posting_id` | `integer` | No (`NOT NULL`) | Foreign Key (`job_postings.id`) | References the job posting being applied for; required |
| `status` | `text` | No (`NOT NULL`) | None | Application status (e.g. `'applied'`, `'interview'`); required |

Unique Constraint:
- `UNIQUE (user_id, job_posting_id)`: A user cannot apply to the same job posting more than once. The database enforces this constraint, and duplicate submission attempts return HTTP `409 Conflict`.

SQL Schema Definition:

```sql
CREATE TABLE applications (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INT NOT NULL,
    job_posting_id INT NOT NULL,
    status TEXT NOT NULL DEFAULT 'applied',
    CONSTRAINT fk_applications_users
        FOREIGN KEY (user_id)
        REFERENCES "Users" ("Id")
        ON DELETE RESTRICT,
    CONSTRAINT fk_applications_job_postings
        FOREIGN KEY (job_posting_id)
        REFERENCES job_postings (id)
        ON DELETE RESTRICT,
    CONSTRAINT uq_applications_user_job
        UNIQUE (user_id, job_posting_id)
);

CREATE INDEX idx_applications_user_id ON applications (user_id);
CREATE INDEX idx_applications_job_posting_id ON applications (job_posting_id);
```

### 2. Many-to-Many Relationship

The `applications` table acts as a join table linking `users` and `job_postings`:
- **Many users can apply to many job postings ($M:N$):** Multiple users can apply to the same job posting, and a single user can submit applications to multiple job postings.
- **One row per (user, posting):** Each application row represents a single application submitted by one user to one job posting.
- **Navigation Properties:**
  - `User.Applications` collection navigation property.
  - `JobPosting.Applications` collection navigation property.
  - `Application.User` and `Application.JobPosting` reference navigation properties.

### 3. Foreign Keys and Deletion Rule

- **Foreign Keys:**
  - `applications.user_id` references `users.id` (`"Users"("Id")`).
  - `applications.job_posting_id` references `job_postings.id`.
- **Deletion Rule:** `RESTRICT` (or `NO ACTION`).
- **Explanation:**
  - Both foreign keys use `RESTRICT` to prevent accidental deletion of user accounts or job postings that have active application history.
  - Deleting a user or job posting with associated applications is rejected by the database until those applications are explicitly handled.

### 4. API Endpoints

#### POST `/api/jobs/{id}/applications`
Submits an application to the specified job posting.
- **Route Parameter:** `{id}` is the job posting ID from the URL (`job_posting_id`).
- **Request Body:** JSON payload containing `user_id`:
  ```json
  {
    "user_id": 1
  }
  ```
- **Validation and Behavior:**
  - If the job posting with ID `{id}` does not exist: returns `404 Not Found`.
  - If the user with ID `user_id` does not exist: returns `404 Not Found`.
  - If the user has already applied to this job posting: returns `409 Conflict`.
  - If valid: persists the new application with `job_posting_id = {id}` and `status = 'applied'`, returning `201 Created` with the created application record.

#### GET `/api/jobs/{id}/applications`
Lists all applications submitted for a specific job posting.
- **Route Parameter:** `{id}` is the job posting ID.
- **Validation:** If the job posting with ID `{id}` does not exist, returns `404 Not Found`.
- **Response:** `200 OK` with an array of applications belonging exclusively to job posting `{id}`.

#### GET `/api/users/{id}/applications`
Lists all applications submitted by a specific user.
- **Route Parameter:** `{id}` is the user ID.
- **Validation:** If the user with ID `{id}` does not exist, returns `404 Not Found`.
- **Response:** `200 OK` with an array of applications belonging exclusively to user `{id}`.

### 5. Example Request

`POST /api/jobs/1/applications`

```json
{
  "user_id": 2
}
```

**Explanation:**
User `2` submits an application for job posting `1`. The server takes `job_posting_id = 1` from the URL route and `user_id = 2` from the payload.

**Example Response (`201 Created`):**

```json
{
  "id": 1,
  "userId": 2,
  "jobPostingId": 1,
  "status": "applied"
}
```

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
├── Controllers/             # MVC and Web API controllers (Users, Announcements, Jobs, Applications)
├── Data/
│   ├── AlumniDbContext.cs    # EF Core database context
│   ├── Factories/            # Initial data factories
│   └── Seeders/              # Database seed logic
├── Migrations/               # EF Core migrations and snapshot
├── Models/                   # User, Announcement, JobPosting, and Application entities (and request DTOs)
├── Views/                    # Razor Views
├── wwwroot/                  # Static assets
├── Program.cs                # Application configuration and DI
├── Alumni.csproj             # .NET dependencies
├── docker-compose.yml        # PostgreSQL service, application container, and volume
├── Dockerfile                # Multi-stage container build for application service
├── .dockerignore             # Docker build exclusions
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
