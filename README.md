# 🎓 Alumni Tracking System

## 📖 About the Project

The **Alumni Tracking System** is a web-based application designed to help universities manage and track information about their alumni in an organized and centralized way.

The system will allow users to manage alumni records together with related information such as departments, graduation details, and professional experiences. It will also provide search and filtering capabilities for easier access to alumni information.

The application is being developed incrementally as part of a university **Web Programming** course project, with new features and architectural improvements introduced throughout the development process.

---

## 🏛️ MVC Architecture Overview

The application is built on the **Model-View-Controller (MVC)** architectural pattern using **ASP.NET Core**. The MVC pattern separates application concerns into three interconnected components:

```text
       +------------------------+
       |    Client / Browser    |
       +-----------+------------+
                   | HTTP Request
                   v
       +------------------------+
       |       CONTROLLER       |
       | (Handles HTTP Request) |
       +-----+------------^-----+
             |            |
  Reads /    |            | Returns Model Data /
  Updates    v            | Renders View
       +-----------+ +----+------+
       |   MODEL   | |   VIEW    |
       |  (Data /  | | (UI /     |
       |  State)   | |  Razor)   |
       +-----------+ +-----------+
                   |
                   v HTTP Response
       +------------------------+
       |    Client / Browser    |
       +------------------------+
```

### 1. MODEL (Application Data & Business Entities)

* **Responsibility:** Represents the shape of the data, business entities, and state within the application.
* **Current Implementation:**
  * [Models/User.cs](Models/User.cs): Defines the `User` class with properties:
    * `int Id`: Unique identifier.
    * `string Name`: User's full name.
    * `string Email`: User's email address.
  * [Models/UserStore.cs](Models/UserStore.cs): Shared static in-memory data store (`public static List<User> Users { get; } = new();`) shared by both MVC and Web API controllers. Initialized empty.
  * [Models/Announcement.cs](Models/Announcement.cs): Defines the `Announcement` class with properties:
    * `int Id`: Unique identifier.
    * `string Title`: Headline or title of the announcement.
    * `string Content`: Detailed body content.
    * `DateTime CreatedDate`: Publication timestamp (defaults to `DateTime.Now`).
  * [Models/AnnouncementStore.cs](Models/AnnouncementStore.cs): Shared static in-memory data store (`public static List<Announcement> Announcements { get; } = new() { ... };`) pre-seeded with 3 mock announcements, shared between `AnnouncementController` and `AnnouncementAdminController`.
* **Data Storage Status (In-Memory):**
  * Data persistence is currently handled **in-memory** using static generic collections in `UserStore.cs` and `AnnouncementStore.cs`.
  * **No active database connection is running at this stage.** Relational database storage (**PostgreSQL 17**) and Object-Relational Mapping (**Entity Framework Core 9.0**) are **planned for subsequent phases** and are **not yet active in runtime**. When the application stops or restarts, runtime changes in memory reset.
  * For comprehensive entity details, storage mechanics, limitations, and future database architecture, see [ddd.md](ddd.md) (**Data Design Documentation**).

---

### 2. CONTROLLERS (Request Processing & Coordination)

The project organizes request handling across specialized MVC and Web API controllers:

1. [Controllers/UserController.cs](Controllers/UserController.cs) - MVC Controller:
   * Inherits from `Controller` (standard ASP.NET Core MVC controller).
   * Represents the user-facing web interface for User management.
   * Interacts with the shared `UserStore.Users` collection.
   * Actions:
     * `Index`: Displays the list of all users in a responsive table ([Views/User/Index.cshtml](Views/User/Index.cshtml)).
     * `Details/{id}`: Displays single user information ([Views/User/Details.cshtml](Views/User/Details.cshtml)).
     * `Create` (GET & POST): Renders create form and adds new user to the shared store ([Views/User/Create.cshtml](Views/User/Create.cshtml)).
     * `Edit/{id}` (GET & POST): Renders edit form and updates existing user data ([Views/User/Edit.cshtml](Views/User/Edit.cshtml)).
     * `Delete/{id}` (GET & POST): Renders delete confirmation and removes user from the shared store ([Views/User/Delete.cshtml](Views/User/Delete.cshtml)).

2. [Controllers/ApiUserController.cs](Controllers/ApiUserController.cs) - Web API Controller:
   * Inherits from `ControllerBase`, marked with `[ApiController]` and route `[Route("api/users")]`.
   * Represents the RESTful Web API for User management.
   * Interacts with the shared `UserStore.Users` collection and returns JSON responses with HTTP status codes.
   * Endpoints:
     * `GET /api/users`: Returns all users (`200 OK`).
     * `GET /api/users/{id}`: Returns user by ID (`200 OK` or `404 Not Found`).
     * `POST /api/users`: Creates a new user (`201 Created` with `Location` header). Auto-assigns next ID if ID is 0 or omitted.
     * `PUT /api/users/{id}`: Full update of user (Name and Email; `200 OK` or `404 Not Found`).
     * `PATCH /api/users/{id}`: Partial update of user (`200 OK` or `404 Not Found`).
     * `DELETE /api/users/{id}`: Deletes user (`204 NoContent` or `404 Not Found`).

3. [Controllers/AnnouncementController.cs](Controllers/AnnouncementController.cs) - MVC Controller (Public):
   * Inherits from `Controller`.
   * Serves public, read-only announcement cards and detailed view pages for visitors and alumni.
   * Interacts with `AnnouncementStore.Announcements` ordered by `CreatedDate` descending.
   * Actions:
     * `Index`: Renders public announcements card grid ([Views/Announcement/Index.cshtml](Views/Announcement/Index.cshtml)).
     * `Details/{id}`: Displays single announcement body text and metadata ([Views/Announcement/Details.cshtml](Views/Announcement/Details.cshtml)).

4. [Controllers/AnnouncementAdminController.cs](Controllers/AnnouncementAdminController.cs) - MVC Controller (Admin):
   * Inherits from `Controller`.
   * Provides administrative CRUD interface to manage campus and alumni announcements.
   * Actions:
     * `Index`: Displays admin table with Manage / Create / Edit / Delete controls ([Views/AnnouncementAdmin/Index.cshtml](Views/AnnouncementAdmin/Index.cshtml)).
     * `Create` (GET & POST): Form to create and persist a new announcement to `AnnouncementStore`.
     * `Edit/{id}` (GET & POST): Form to update announcement title and content in `AnnouncementStore`.
     * `Delete/{id}` (GET & POST): Deletes announcement from `AnnouncementStore`.

5. [Controllers/HealthController.cs](Controllers/HealthController.cs):
   * Inherits from `ControllerBase`, route `api/[controller]`.
   * Provides API health check endpoints (`GET` and `POST` at `/api/health`) returning `{ status = "ok" }`.

6. [Controllers/HomeController.cs](Controllers/HomeController.cs):
   * Inherits from `Controller`.
   * Serves landing and platform information Razor Views (`Index` and `About`).

7. [Controllers/AlumniController.cs](Controllers/AlumniController.cs):
   * Inherits from `ControllerBase`, route `/`.
   * Foundational test/demo routes (`/hello`, `/hello/miray`, `/sum/{number1}/{number2}`).

---

### 3. VIEW (User Interface & Presentation)

* **Responsibility:** Renders the user-facing interface presented to clients in web browsers.
* **Current Implementation:**
  * **User Management Views ([Views/User/](Views/User/)):**
    * [Views/User/Index.cshtml](Views/User/Index.cshtml): User list table with action buttons (Details, Edit, Delete), and empty state card when no users exist.
    * [Views/User/Details.cshtml](Views/User/Details.cshtml): Single user details card.
    * [Views/User/Create.cshtml](Views/User/Create.cshtml): Form for adding new users with CSRF validation.
    * [Views/User/Edit.cshtml](Views/User/Edit.cshtml): Form for updating user name and email.
    * [Views/User/Delete.cshtml](Views/User/Delete.cshtml): Confirmation prompt for deleting a user.
  * **Announcement Public Views ([Views/Announcement/](Views/Announcement/)):**
    * [Views/Announcement/Index.cshtml](Views/Announcement/Index.cshtml): Responsive card grid displaying active announcements with creation date badges and detail links.
    * [Views/Announcement/Details.cshtml](Views/Announcement/Details.cshtml): Detailed single announcement view with date, authoring info, and back navigation.
  * **Announcement Admin Management Views ([Views/AnnouncementAdmin/](Views/AnnouncementAdmin/)):**
    * [Views/AnnouncementAdmin/Index.cshtml](Views/AnnouncementAdmin/Index.cshtml): Administrative management table listing announcements with action buttons.
    * [Views/AnnouncementAdmin/Create.cshtml](Views/AnnouncementAdmin/Create.cshtml): Form to create and publish a new announcement.
    * [Views/AnnouncementAdmin/Edit.cshtml](Views/AnnouncementAdmin/Edit.cshtml): Form to edit announcement headline and content.
    * [Views/AnnouncementAdmin/Delete.cshtml](Views/AnnouncementAdmin/Delete.cshtml): Confirmation prompt to remove an announcement.
  * **Home Views ([Views/Home/](Views/Home/)):**
    * [Views/Home/Index.cshtml](Views/Home/Index.cshtml): Main landing page with hero banner and feature cards.
    * [Views/Home/About.cshtml](Views/Home/About.cshtml): Platform info, tracked categories, and goals.
  * **Shared Layout & Assets:**
    * [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml): Master layout with responsive navbar (Home, About, Users, Announcements, Admin links), footer, Bootstrap 5.3, and icons.
    * [Views/_ViewStart.cshtml](Views/_ViewStart.cshtml): Configures `_Layout.cshtml` as default layout.
    * [Views/_ViewImports.cshtml](Views/_ViewImports.cshtml): Global namespaces and Tag Helpers.
    * [wwwroot/css/site.css](wwwroot/css/site.css): Custom CSS styles for UI cards, badges, buttons, and navigation.

---

## 🔄 Request Lifecycle & Flow

### 1. Conceptual Flow (Lecture MVC Model)

```text
Client / Browser
      |
      v
  Controller
      |
      v
 Model / Data
      |
      v
  Controller
      |
      v
HTTP Response
```

### 2. Actual Code Implementation Flows

#### A. Web API Request Flow (ApiUserController - e.g., POST /api/users)

```text
1. Client (Swagger UI / HTTP Client) sends HTTP Request:
   POST /api/users  { "name": "Miray", "email": "miray@example.com" }
      |
      v
2. ASP.NET Core Routing Pipeline
   Routes the request to ApiUserController.CreateUser(User user)
      |
      v
3. Controller Processes Request & Model:
   - Deserializes JSON payload into Alumni.Models.User instance
   - Auto-assigns ID if omitted (user.Id = UserStore.Users.Max() + 1)
   - Appends user to shared in-memory store (UserStore.Users.Add(user))
      |
      v
4. Controller Prepares HTTP Response:
   - Returns HTTP 201 Created with Location header "/api/users/1" and user JSON
      |
      v
5. Client receives JSON Response with status code 201 Created
```

#### B. MVC User Web UI Flow (UserController - e.g., GET /User)

```text
1. User navigates to http://localhost:5067/User in Web Browser
      |
      v
2. Routing invokes UserController.Index()
      |
      v
3. Controller reads UserStore.Users and passes collection to View():
   - Razor View Engine executes Views/User/Index.cshtml
   - Layout is applied from Views/Shared/_Layout.cshtml
   - Displays table of users or empty-state card
      |
      v
4. Client receives rendered HTML/CSS page with HTTP 200 OK
```

#### C. Web UI Home Request Flow (HomeController - e.g., GET /)

```text
1. User navigates to http://localhost:5067/
      |
      v
2. Middleware in Program.cs rewrites root request to /Home/Index
      |
      v
3. Routing invokes HomeController.Index() -> returns View()
      |
      v
4. Client receives rendered HTML page with HTTP 200 OK
```

---

## 📂 Project Directory & File Structure

The current repository structure and the responsibility of each file and directory:

```text
alumni/
|
+-- Controllers/                         # Request handling controllers (MVC and Web API)
|   +-- AlumniController.cs              # Introductory test endpoints (/hello, /sum) & fallback pages
|   +-- AnnouncementController.cs       # MVC controller for public announcement cards & details (/Announcement)
|   +-- AnnouncementAdminController.cs  # MVC controller for administrative announcement CRUD (/AnnouncementAdmin)
|   +-- ApiUserController.cs             # RESTful CRUD Web API endpoints (/api/users)
|   +-- HealthController.cs              # API health verification endpoints (/api/health)
|   +-- HomeController.cs                # MVC controller returning landing Razor Views (/Home/Index, /Home/About)
|   `-- UserController.cs                # MVC controller for User CRUD web views (/User, /User/Create, etc.)
|
+-- Models/                              # Data models and shared state
|   +-- Announcement.cs                  # Announcement business entity (Id, Title, Content, CreatedDate)
|   +-- AnnouncementStore.cs             # Shared in-memory store pre-seeded with 3 mock announcements
|   +-- User.cs                          # User data entity model (Id, Name, Email)
|   `-- UserStore.cs                     # Shared in-memory data store (List<User>)
|
+-- Views/                               # Razor Views (UI presentation layer)
|   +-- Announcement/                    # Public announcement presentation views
|   |   +-- Details.cshtml               # Single announcement detailed view
|   |   `-- Index.cshtml                 # Responsive public announcements card grid
|   +-- AnnouncementAdmin/               # Administrative announcement management views
|   |   +-- Create.cshtml                # Form for publishing new announcement
|   |   +-- Delete.cshtml                # Confirmation prompt for deleting an announcement
|   |   +-- Edit.cshtml                  # Form for editing announcement headline and content
|   |   `-- Index.cshtml                 # Admin table listing announcements with action buttons
|   +-- Home/                            # Views corresponding to HomeController
|   |   +-- About.cshtml                 # Platform purpose, tracked entities, and goals page
|   |   `-- Index.cshtml                 # Main landing page with hero banner and feature cards
|   +-- Shared/                          # Shared templates across views
|   |   `-- _Layout.cshtml               # Master HTML layout, navigation navbar, footer, Bootstrap CDN
|   +-- User/                            # Views corresponding to UserController (MVC CRUD UI)
|   |   +-- Create.cshtml                # Form for creating a new user
|   |   +-- Delete.cshtml                # Confirmation page for deleting a user
|   |   +-- Details.cshtml               # Details view showing user information
|   |   +-- Edit.cshtml                  # Form for editing an existing user
|   |   `-- Index.cshtml                 # User listing table with action links
|   +-- _ViewImports.cshtml              # Global Razor directives and MVC Tag Helpers
|   `-- _ViewStart.cshtml                # Configures the default layout for views
|
+-- Properties/                          # Project launch settings
|   `-- launchSettings.json              # Development profiles, ports (http://localhost:5067), environment variables
|
+-- wwwroot/                             # Static web assets served directly to the client
|   `-- css/
|       `-- site.css                     # Custom styles for Razor views (cards, navbar, badges, buttons)
|
+-- Alumni.csproj                        # .NET 9 project file, SDK configuration, and NuGet dependencies
+-- Alumni.http                          # HTTP test requests file for Visual Studio and VS Code REST Client
+-- appsettings.json                     # General application configuration settings
+-- appsettings.Development.json         # Development environment-specific configuration settings
+-- ddd.md                               # Data Design Documentation (current & planned database design)
+-- docker-compose.yml                   # Docker Compose configuration for PostgreSQL 17 dev database service
+-- Program.cs                           # Application entry point: service registration, middleware, and routing
+-- README.md                            # Comprehensive project documentation
`-- LICENSE                              # Project license (MIT)
```

### Detailed File Responsibilities

| File / Folder | Type | Responsibility |
| ------------- | ---- | -------------- |
| [Program.cs](Program.cs) | Configuration | Configures dependency injection (`AddControllersWithViews`, `AddSwaggerGen`), middleware pipeline, URL rewrites, and endpoint routing. |
| [Alumni.csproj](Alumni.csproj) | Project File | Defines target framework (`net9.0`) and active NuGet dependencies (`Swashbuckle.AspNetCore`, `Microsoft.AspNetCore.OpenApi`). |
| [Alumni.http](Alumni.http) | Testing | Pre-configured HTTP requests for testing API endpoints (`/api/health`, `/api/users`, `POST`, `PUT`, `PATCH`, `DELETE`). |
| [ddd.md](ddd.md) | Documentation | Comprehensive Data Design Document covering POCO models, in-memory stores, limitations, ERD, and planned PostgreSQL schema. |
| [docker-compose.yml](docker-compose.yml) | Infrastructure | Docker Compose definition running PostgreSQL 17 Alpine database service (`alumni_db`, port 5432) for development. |
| [Models/User.cs](Models/User.cs) | Model | Defines the `User` class schema (`Id`, `Name`, `Email`). |
| [Models/UserStore.cs](Models/UserStore.cs) | Storage | Shared static in-memory collection (`List<User> Users`) shared between `UserController` and `ApiUserController`. |
| [Models/Announcement.cs](Models/Announcement.cs) | Model | Defines the `Announcement` class schema (`Id`, `Title`, `Content`, `CreatedDate`). |
| [Models/AnnouncementStore.cs](Models/AnnouncementStore.cs) | Storage | Shared static in-memory collection (`List<Announcement> Announcements`) pre-seeded with 3 items. |
| [Controllers/UserController.cs](Controllers/UserController.cs) | Controller (MVC) | Handles web user requests, performs CRUD on `UserStore.Users`, and returns Razor views (`Index`, `Details`, `Create`, `Edit`, `Delete`). |
| [Controllers/ApiUserController.cs](Controllers/ApiUserController.cs) | Controller (API) | RESTful API controller exposing `/api/users` endpoints (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`) with JSON responses. |
| [Controllers/AnnouncementController.cs](Controllers/AnnouncementController.cs) | Controller (MVC) | Public read-only controller rendering announcement feed and detail pages. |
| [Controllers/AnnouncementAdminController.cs](Controllers/AnnouncementAdminController.cs) | Controller (MVC) | Administrative controller handling CRUD operations on announcements with validation. |
| [Controllers/HealthController.cs](Controllers/HealthController.cs) | Controller (API) | Provides GET and POST health endpoints returning `{ status = "ok" }`. |
| [Controllers/HomeController.cs](Controllers/HomeController.cs) | Controller (MVC) | MVC Controller returning Razor views for `Index` and `About` pages. |
| [Controllers/AlumniController.cs](Controllers/AlumniController.cs) | Controller (API) | API Controller containing test routes (`/hello`, `/sum`) and fallback responses. |
| [Views/User/](Views/User/) | Views (MVC) | Razor views for User CRUD operations (`Index`, `Details`, `Create`, `Edit`, `Delete`). |
| [Views/Announcement/](Views/Announcement/) | Views (MVC) | Public Razor views for viewing announcements (`Index`, `Details`). |
| [Views/AnnouncementAdmin/](Views/AnnouncementAdmin/) | Views (MVC) | Administrative Razor views for managing announcements (`Index`, `Create`, `Edit`, `Delete`). |
| [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml) | View | Master layout defining the standard page skeleton, navigation bar with Users and Announcements links, and footer. |
| [Views/Home/Index.cshtml](Views/Home/Index.cshtml) | View | Homepage presentation content displaying project intro and feature highlights. |
| [Views/Home/About.cshtml](Views/Home/About.cshtml) | View | Informational presentation content explaining the scope of the alumni system. |
| [wwwroot/css/site.css](wwwroot/css/site.css) | Static Asset | Custom stylesheet providing modern UI styling for Razor views. |

---

## 🔌 Current API Endpoints

The following API endpoints are currently implemented and active in the application:

### 1. Health Endpoints

| Method | Endpoint | Description | Response Status |
| ------ | -------- | ----------- | --------------- |
| `GET` | `/api/health` | Returns health status JSON `{ status = "ok" }` | `200 OK` |
| `POST` | `/api/health` | Returns health status JSON `{ status = "ok" }` | `200 OK` |

### 2. Users REST API Endpoints (`ApiUserController`)

> **Note on Data Storage:** The Users API uses an **in-memory list** (`UserStore.Users`). Data is shared with `UserController` and resets upon server restart.

| Method | Endpoint | Description | Request Body | Response Status |
| ------ | -------- | ----------- | ------------ | --------------- |
| `GET` | `/api/users` | Returns list of all users | *None* | `200 OK` |
| `GET` | `/api/users/{id}` | Returns a single user by ID | *None* | `200 OK` or `404 Not Found` |
| `POST` | `/api/users` | Creates a new user (auto-assigns ID if 0 or omitted) | JSON `User` object | `201 Created` (with `Location` header) |
| `PUT` | `/api/users/{id}` | **Full update:** Replaces both `Name` and `Email` | JSON `User` object | `200 OK` or `404 Not Found` |
| `PATCH` | `/api/users/{id}` | **Partial update:** Updates `Name` and/or `Email` only if provided | JSON `User` object | `200 OK` or `404 Not Found` |
| `DELETE` | `/api/users/{id}` | Deletes user by ID | *None* | `204 NoContent` or `404 Not Found` |

### 3. Users MVC Web Routes (`UserController`)

| Method | Route | Description | Result |
| ------ | ----- | ----------- | ------ |
| `GET` | `/User` or `/User/Index` | Displays all users in a table with action buttons | Renders [Views/User/Index.cshtml](Views/User/Index.cshtml) |
| `GET` | `/User/Details/{id}` | Displays single user details card | Renders [Views/User/Details.cshtml](Views/User/Details.cshtml) |
| `GET` | `/User/Create` | Displays form for creating a new user | Renders [Views/User/Create.cshtml](Views/User/Create.cshtml) |
| `POST` | `/User/Create` | Processes create form, adds user to `UserStore` | Redirects to `/User` |
| `GET` | `/User/Edit/{id}` | Displays form for editing user | Renders [Views/User/Edit.cshtml](Views/User/Edit.cshtml) |
| `POST` | `/User/Edit/{id}` | Processes edit form, updates user in `UserStore` | Redirects to `/User` |
| `GET` | `/User/Delete/{id}` | Displays confirmation prompt for deletion | Renders [Views/User/Delete.cshtml](Views/User/Delete.cshtml) |
| `POST` | `/User/Delete/{id}` | Deletes user from `UserStore` | Redirects to `/User` |
| `GET` | `/users` | Alternative route for listing all users | Renders [Views/User/Index.cshtml](Views/User/Index.cshtml) |
| `POST` | `/users` | Alternative route for adding a new user | Redirects to `/users` |

### 4. Announcements MVC Web Routes (`AnnouncementController` & `AnnouncementAdminController`)

> **Note on Data Storage:** Announcements are stored in an **in-memory list** (`AnnouncementStore.Announcements`) pre-populated with 3 seed announcements. Data resets upon server restart.

| Method | Route | Controller | Description | Result |
| ------ | ----- | ---------- | ----------- | ------ |
| `GET` | `/Announcement` | `AnnouncementController` | Public feed displaying announcements sorted by newest first | Renders [Views/Announcement/Index.cshtml](Views/Announcement/Index.cshtml) |
| `GET` | `/Announcement/Details/{id}` | `AnnouncementController` | Public single announcement view | Renders [Views/Announcement/Details.cshtml](Views/Announcement/Details.cshtml) |
| `GET` | `/AnnouncementAdmin` | `AnnouncementAdminController` | Administrative table listing all announcements with manage links | Renders [Views/AnnouncementAdmin/Index.cshtml](Views/AnnouncementAdmin/Index.cshtml) |
| `GET` | `/AnnouncementAdmin/Create` | `AnnouncementAdminController` | Admin form to compose a new announcement | Renders [Views/AnnouncementAdmin/Create.cshtml](Views/AnnouncementAdmin/Create.cshtml) |
| `POST` | `/AnnouncementAdmin/Create` | `AnnouncementAdminController` | Processes form, auto-generates ID, adds to `AnnouncementStore` | Redirects to `/AnnouncementAdmin` |
| `GET` | `/AnnouncementAdmin/Edit/{id}` | `AnnouncementAdminController` | Admin form to edit an existing announcement | Renders [Views/AnnouncementAdmin/Edit.cshtml](Views/AnnouncementAdmin/Edit.cshtml) |
| `POST` | `/AnnouncementAdmin/Edit/{id}` | `AnnouncementAdminController` | Updates announcement title and content in `AnnouncementStore` | Redirects to `/AnnouncementAdmin` |
| `GET` | `/AnnouncementAdmin/Delete/{id}` | `AnnouncementAdminController` | Displays confirmation prompt for deleting an announcement | Renders [Views/AnnouncementAdmin/Delete.cshtml](Views/AnnouncementAdmin/Delete.cshtml) |
| `POST` | `/AnnouncementAdmin/Delete/{id}` | `AnnouncementAdminController` | Removes announcement from `AnnouncementStore` | Redirects to `/AnnouncementAdmin` |

### 5. Utility & Demonstration Endpoints

| Method | Endpoint | Description | Response Status |
| ------ | -------- | ----------- | --------------- |
| `GET` | `/hello` | Returns `"Hello World!"` | `200 OK` |
| `GET` | `/hello/miray` | Returns `"Hello Miray"` | `200 OK` |
| `GET` | `/sum/{number1}/{number2}` | Returns the sum of two integers | `200 OK` |

---

## 📑 Data Design Documentation (`ddd.md`)

A comprehensive **Data Design Document** is maintained in the root directory: [ddd.md](ddd.md).

It provides detailed technical documentation on:
* **Current Data Model:** POCO entities (`User`, `Announcement`), default values, validations, and lifecycle.
* **Storage Strategy:** In-memory collections (`UserStore`, `AnnouncementStore`), LINQ execution mechanics, and state sharing.
* **Limitations & Risks:** RAM volatility, non-thread-safe collections, lack of ACID transactions, and horizontal scaling constraints.
* **Planned Database Architecture:** PostgreSQL 17 relational schema, ER diagrams, foreign key relationships, table definitions with constraints, and an EF Core 9 migration strategy.

---

## 🛠️ Technology Stack

### Current Implementation (Active)

* **Backend Framework:** C# with **ASP.NET Core 9.0** (`net9.0`)
* **Architecture:** MVC (Model-View-Controller) with integrated Web API
* **Presentation Layer (Views):** Razor Views (`.cshtml`), HTML5, CSS3, Bootstrap 5.3.3, Bootstrap Icons
* **Data Storage:** Shared in-memory static generic collections (`UserStore.Users`, `AnnouncementStore.Announcements`)
* **Database Infrastructure (Configured):** **Docker Compose** service running **PostgreSQL 17** (`postgres:17-alpine`) on port 5432 ([docker-compose.yml](docker-compose.yml))
* **API Documentation & Testing:** Swagger / OpenAPI via Swashbuckle (`Swashbuckle.AspNetCore` 6.6.2, `Microsoft.AspNetCore.OpenApi` 9.0.18)
* **Design & Schema Documentation:** [ddd.md](ddd.md) (Data Design Document)
* **Version Control:** Git & GitHub

### Planned Architecture (Future Phases)

* **Database Runtime Integration:** Connecting ASP.NET Core directly to PostgreSQL 17 for persistent data storage
* **ORM:** **Entity Framework Core 9.0** (`Npgsql.EntityFrameworkCore.PostgreSQL`) for code-first migrations and entity mapping
* **Additional Entities:** AlumniProfile, Department, GraduationDetails, JobExperience models
* **Security:** Authentication, password hashing, and role-based authorization (Admin / Alumni / Faculty)
* **Service Layer:** Service & repository classes to decouple business logic from controllers

---

## 🚀 How to Run

### 1. Clone the Repository

```bash
git clone https://github.com/miraydestan/alumni.git
cd alumni
```

### 2. (Optional) Start the PostgreSQL Database Container

A PostgreSQL 17 container is pre-configured in [docker-compose.yml](docker-compose.yml) for local database development:

```bash
docker compose up -d db
```

> *Note: The application currently operates with in-memory stores; connecting runtime database queries via EF Core is planned for the next development phase.*

### 3. Run the Application

```bash
dotnet run
```

The application runs locally at:

```text
http://localhost:5067
```

### 4. Access the Web Pages (Razor Views)

* **Home Page:** [http://localhost:5067/](http://localhost:5067/)
* **About Page:** [http://localhost:5067/about](http://localhost:5067/about)
* **User Management (MVC):** [http://localhost:5067/User](http://localhost:5067/User)
* **Announcements Feed (Public):** [http://localhost:5067/Announcement](http://localhost:5067/Announcement)
* **Announcements Admin (Management):** [http://localhost:5067/AnnouncementAdmin](http://localhost:5067/AnnouncementAdmin)

### 5. Access Swagger UI (API Documentation)

Swagger UI is configured with route prefix `api/swagger` in [Program.cs](Program.cs):

```text
http://localhost:5067/api/swagger
```

Swagger provides an interactive web interface for inspecting and testing all API endpoints directly in your browser:

* **Swagger UI:** [http://localhost:5067/api/swagger](http://localhost:5067/api/swagger)

### 6. Test with HTTP Client

You can also run requests defined in [Alumni.http](Alumni.http) directly from Visual Studio or the VS Code REST Client extension.

---

## 📊 Project Status: Implemented vs. Planned

| Feature / Component | Status | Details |
| ------------------- | ------ | ------- |
| **ASP.NET Core 9.0 Setup** | ✅ Implemented | Configured with `AddControllersWithViews`, routing, and static file serving. |
| **User Entity Model** | ✅ Implemented | `Models/User.cs` (`Id`, `Name`, `Email`). |
| **User In-Memory Store** | ✅ Implemented | `Models/UserStore.cs` (`List<User> Users`) shared across MVC and API controllers. |
| **User MVC Controller & Views** | ✅ Implemented | `Controllers/UserController.cs` with full Razor CRUD views in `Views/User/`. |
| **User Web API Controller** | ✅ Implemented | `Controllers/ApiUserController.cs` exposing RESTful `/api/users` endpoints. |
| **Announcement Entity Model** | ✅ Implemented | `Models/Announcement.cs` (`Id`, `Title`, `Content`, `CreatedDate`). |
| **Announcement In-Memory Store**| ✅ Implemented | `Models/AnnouncementStore.cs` with 3 pre-seeded mock announcements. |
| **Announcement MVC Controllers**| ✅ Implemented | `AnnouncementController` (public) and `AnnouncementAdminController` (admin CRUD). |
| **Announcement Razor Views** | ✅ Implemented | Full public card grid and administrative management views in `Views/Announcement/` and `Views/AnnouncementAdmin/`. |
| **Data Design Documentation** | ✅ Implemented | [ddd.md](ddd.md) documenting current in-memory model, limitations, ERD, and planned PostgreSQL design. |
| **Docker Compose PostgreSQL Service** | ✅ Implemented | `docker-compose.yml` defining PostgreSQL 17 Alpine development service container. |
| **Health Check API** | ✅ Implemented | `GET` and `POST` at `/api/health`. |
| **Swagger / OpenAPI** | ✅ Implemented | Interactive API documentation at `/api/swagger`. |
| **Responsive Bootstrap UI** | ✅ Implemented | Layout with navigation header, hero cards, and tables. |
| **Static Assets** | ✅ Implemented | `wwwroot/css/site.css` served via `app.UseStaticFiles()`. |
| **PostgreSQL Runtime Database** | ⏳ Planned | Relational database connection and persistent storage at application runtime. |
| **Entity Framework Core ORM** | ⏳ Planned | `DbContext`, migrations, entity configurations, and repository data layer. |
| **Alumni & Domain Models** | ⏳ Planned | AlumniProfile, Department, GraduationDetails, and JobExperience entities. |
| **Authentication & Authorization** | ⏳ Planned | User login, registration, password hashing, and role-based access control. |

---

## 📌 Planned Features Roadmap

The system is planned to support:

* Adding, viewing, updating, and deleting alumni records
* Searching and filtering alumni by department, year, or industry
* Managing department and faculty information
* Managing graduation years and degree details
* Managing professional, job, and career experience history
* User authentication and authorization (Admin vs. Alumni roles)
* Dashboard statistics and alumni distribution charts
* Relational database integration with PostgreSQL
* Persistent data storage managed via Entity Framework Core migrations
* Multi-container application and database deployment with Docker Compose

---

## 📄 License

This project is licensed under the **MIT License**.
