# 🎓 Alumni Tracking System

## 📖 About the Project

The **Alumni Tracking System** is a web-based application designed to help universities manage and track information about their alumni in an organized and centralized way.

The system will allow users to manage alumni records together with related information such as departments, graduation details, and professional experiences. It will also provide search and filtering capabilities for easier access to alumni information.

The application is being developed incrementally as part of a university **Web Programming** course project, with new features and architectural improvements introduced throughout the development process.

---

## 🏛️ MVC Architecture Overview

The application is built on the **Model-View-Controller (MVC)** architectural pattern using **ASP.NET Core**. The MVC pattern separates application concerns into three interconnected components:

```text
       ┌────────────────────────┐
       │    Client / Browser    │
       └───────────┬────────────┘
                   │ HTTP Request
                   ▼
       ┌────────────────────────┐
       │       CONTROLLER       │
       │ (Handles HTTP Request) │
       └─────┬────────────▲─────┘
             │            │
  Reads /    │            │ Returns Model Data /
  Updates    ▼            │ Renders View
       ┌───────────┐ ┌────┴──────┐
       │   MODEL   │ │   VIEW    │
       │  (Data /  │ │ (UI /     │
       │  State)   │ │  Razor)   │
       └───────────┘ └───────────┘
                   │
                   ▼ HTTP Response
       ┌────────────────────────┐
       │    Client / Browser    │
       └────────────────────────┘
```

### 1. MODEL (Application Data & Business Entities)
* **Responsibility:** Represents the shape of the data, business entities, and state within the application.
* **Current Implementation:**
  * [`Models/User.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Models/User.cs): Defines the `User` class with properties:
    * `int Id`: Unique identifier.
    * `string Name`: User's full name.
    * `string Email`: User's email address.
* **Data Storage Status (In-Memory):**
  * Data persistence is currently handled **in-memory** using a static collection (`private static readonly List<User> _users = new();`) located in [`Controllers/UsersController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/UsersController.cs).
  * **No database is connected at this stage.** Relational database storage (**PostgreSQL**) and Object-Relational Mapping (**Entity Framework Core**) are **planned for future iterations** and are **not yet implemented**. When the application stops or restarts, the in-memory data resets.

---

### 2. CONTROLLER (Request Processing & Coordination)
* **Responsibility:** Receives incoming HTTP requests, processes input parameters, coordinates with the data/model layer, and returns the appropriate HTTP response (JSON data or rendered HTML views).
* **Current Controllers:**
  1. [`Controllers/UsersController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/UsersController.cs):
     * Inherits from `ControllerBase`, marked with `[ApiController]` and route `api/[controller]`.
     * Implements full RESTful CRUD API endpoints for users.
     * Interacts directly with the `User` model and the in-memory `List<User>` store.
     * Handles data updates with separate **PUT** (full replacement) and **PATCH** (partial update) endpoints.
  2. [`Controllers/HealthController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/HealthController.cs):
     * Inherits from `ControllerBase`, marked with `[ApiController]` and route `api/[controller]`.
     * Provides API health check endpoints (`GET` and `POST` at `/api/health`) returning `{ status = "ok" }`.
  3. [`Controllers/HomeController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/HomeController.cs):
     * Inherits from `Controller` (standard ASP.NET Core MVC controller).
     * Serves user-facing Razor Views for the web presentation layer:
       * `Index()`: Serves the landing page view (`Views/Home/Index.cshtml`).
       * `About()`: Serves the platform information view (`Views/Home/About.cshtml`).
  4. [`Controllers/AlumniController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/AlumniController.cs):
     * Inherits from `ControllerBase`, marked with `[ApiController]` and base route `/`.
     * Implements foundational test and demonstration endpoints:
       * `GET /hello`: Returns plain text `"Hello World!"`.
       * `GET /hello/miray`: Returns plain text `"Hello Miray"`.
       * `GET /sum/{number1}/{number2}`: Returns the calculated sum of two numbers.
       * `GET /` and `GET /about`: Contains standalone HTML response helpers.

---

### 3. VIEW (User Interface & Presentation)
* **Responsibility:** Renders the user-facing interface presented to clients in web browsers.
* **Current Implementation:**
  * Razor Views (`.cshtml`) are implemented for informational and landing pages:
    * [`Views/Home/Index.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/Home/Index.cshtml): Main landing page featuring hero banner, mission statements, and feature cards (Alumni Network, Career & Experience, Easy Access).
    * [`Views/Home/About.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/Home/About.cshtml): Informational page detailing the platform purpose, tracked data categories (Alumni, Department, Graduation, Job Experience), and platform governance.
    * [`Views/Shared/_Layout.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/Shared/_Layout.cshtml): Shared master layout with Bootstrap 5.3 navigation header, responsive container structure, footer, and scripts.
    * [`Views/_ViewStart.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/_ViewStart.cshtml): Specifies the default layout (`_Layout.cshtml`) for all views.
    * [`Views/_ViewImports.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/_ViewImports.cshtml): Imports global namespaces (`Alumni`) and ASP.NET Core MVC Tag Helpers.
    * [`wwwroot/css/site.css`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/wwwroot/css/site.css): Custom CSS styles for UI cards, badges, buttons, and navigation.
* **Status of Data-Driven / CRUD Views:**
  * While landing/informational Razor views currently exist, **entity-specific dynamic views** (e.g. Razor forms and tables for listing, creating, or editing Users and Alumni) are **not yet implemented**.
  * User and Alumni management is currently exposed exclusively through JSON REST API endpoints (`/api/users`). Full Razor CRUD views bound to database models are planned for future iterations.

---

## 🔄 Request Lifecycle & Flow

### 1. Conceptual Flow (Lecture MVC Model)

```text
Client / Browser
      ↓
  Controller
      ↓
 Model / Data
      ↓
  Controller
      ↓
HTTP Response
```

### 2. Actual Code Implementation Flows

Depending on whether a client requests a Web API endpoint or a web page, the request flows through ASP.NET Core as follows:

#### A. Web API Request Flow (e.g., `POST /api/users` or `GET /api/users/{id}`)
```text
1. Client (Browser / Swagger UI / HTTP Client) sends HTTP Request:
   POST /api/users  (with JSON body { "id": 1, "name": "Miray", "email": "miray@example.com" })
      │
      ▼
2. ASP.NET Core Routing Pipeline
   Routes the request to UsersController.CreateUser(User user)
      │
      ▼
3. Controller Processes Request & Model:
   - Deserializes JSON payload into Alumni.Models.User instance
   - Appends user to in-memory store (_users.Add(user))
      │
      ▼
4. Controller Prepares HTTP Response:
   - Generates HTTP 201 Created with Location header "/api/users/1"
      │
      ▼
5. Client receives JSON Response with status code 201 Created
```

#### B. Web UI Request Flow (e.g., `GET /` or `GET /about`)
```text
1. User enters http://localhost:5067/ in Web Browser
      │
      ▼
2. Middleware in Program.cs rewrites root request to /Home/Index
      │
      ▼
3. Routing invokes HomeController.Index()
      │
      ▼
4. Controller returns View():
   - Razor View Engine executes Views/Home/Index.cshtml
   - Layout is applied from Views/Shared/_Layout.cshtml
   - Static assets (CSS) are referenced from wwwroot/css/site.css
      │
      ▼
5. Client receives rendered HTML/CSS page with HTTP 200 OK
```

#### C. API Health Check Flow (`GET /api/health`)
```text
1. Client sends GET /api/health
      │
      ▼
2. Routing invokes HealthController.Get()
      │
      ▼
3. Controller returns Ok(new { status = "ok" })
      │
      ▼
4. Client receives { "status": "ok" } with HTTP 200 OK
```

---

## 📂 Project Directory & File Structure

The current repository structure and the responsibility of each file and directory:

```text
alumni/
│
├── Controllers/                         # Request handling and API/UI controllers
│   ├── AlumniController.cs              # Introductory test endpoints (/hello, /sum) & fallback pages
│   ├── HealthController.cs              # API health verification endpoints (/api/health)
│   ├── HomeController.cs                # MVC controller returning landing Razor Views (/Home/Index, /Home/About)
│   └── UsersController.cs               # RESTful CRUD API endpoints for users with in-memory storage
│
├── Models/                              # Data models and entity representations
│   └── User.cs                          # User data entity model (Id, Name, Email)
│
├── Views/                               # Razor Views (UI presentation layer)
│   ├── Home/                            # Views corresponding to HomeController
│   │   ├── About.cshtml                 # Platform purpose, tracked entities, and goals page
│   │   └── Index.cshtml                 # Main landing page with hero banner and feature cards
│   ├── Shared/                          # Shared templates across views
│   │   └── _Layout.cshtml               # Master HTML layout, navigation navbar, footer, Bootstrap CDN
│   ├── _ViewImports.cshtml              # Global Razor directives and MVC Tag Helpers
│   └── _ViewStart.cshtml                # Configures the default layout for views
│
├── Properties/                          # Project launch settings
│   └── launchSettings.json              # Development profiles, ports (http://localhost:5067), environment variables
│
├── wwwroot/                             # Static web assets served directly to the client
│   └── css/
│       └── site.css                     # Custom styles for Razor views (cards, navbar, badges, buttons)
│
├── Alumni.csproj                        # .NET 9 project file, SDK configuration, and NuGet dependencies
├── Alumni.http                          # HTTP test requests file for Visual Studio and VS Code REST Client
├── appsettings.json                     # General application configuration settings
├── appsettings.Development.json         # Development environment-specific configuration settings
├── Program.cs                           # Application entry point: service registration, middleware, and routing
├── README.md                            # Comprehensive project documentation
└── LICENSE                              # Project license (MIT)
```

### Detailed File Responsibilities

| File / Folder | Type | Responsibility |
| ------------- | ---- | -------------- |
| [`Program.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Program.cs) | Configuration | Configures the dependency injection container (`AddControllersWithViews`, `AddSwaggerGen`), configures the HTTP request pipeline, middleware, custom URL rewrites, and sets up endpoint routing. |
| [`Alumni.csproj`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Alumni.csproj) | Project File | Defines target framework (`net9.0`) and NuGet package references (`Swashbuckle.AspNetCore`, `Microsoft.AspNetCore.OpenApi`). |
| [`Alumni.http`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Alumni.http) | Testing | Contains pre-configured HTTP requests for local testing of API endpoints (`/api/health`, `/api/users`, `/hello`, `/sum`). |
| [`Models/User.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Models/User.cs) | Model | Defines the `User` class schema (`Id`, `Name`, `Email`) used by the Users API. |
| [`Controllers/UsersController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/UsersController.cs) | Controller | Handles CRUD operations for users using an in-memory list (`_users`). Implements `GET`, `POST`, `PUT`, `PATCH`, and `DELETE`. |
| [`Controllers/HealthController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/HealthController.cs) | Controller | Provides GET and POST health endpoints returning `{ status = "ok" }`. |
| [`Controllers/HomeController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/HomeController.cs) | Controller | MVC Controller that returns Razor views for `Index` and `About` pages. |
| [`Controllers/AlumniController.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Controllers/AlumniController.cs) | Controller | API Controller containing test routes (`/hello`, `/sum`) and fallback responses. |
| [`Views/Shared/_Layout.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/Shared/_Layout.cshtml) | View | Master layout defining the standard page skeleton, navigation bar, and footer. |
| [`Views/Home/Index.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/Home/Index.cshtml) | View | Homepage presentation content displaying project intro and feature highlights. |
| [`Views/Home/About.cshtml`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Views/Home/About.cshtml) | View | Informational presentation content explaining the scope of the alumni system. |
| [`wwwroot/css/site.css`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/wwwroot/css/site.css) | Static Asset | Custom stylesheet providing modern UI styling for Razor views. |

---

## 🔌 Current API Endpoints

The following API endpoints are currently implemented and active in the application:

### 1. Health Endpoints

| Method | Endpoint | Description | Response Status |
| ------ | -------- | ----------- | --------------- |
| `GET` | `/api/health` | Returns health status JSON `{ status = "ok" }` | `200 OK` |
| `POST` | `/api/health` | Returns health status JSON `{ status = "ok" }` | `200 OK` |

### 2. Users CRUD Endpoints

> **Note on Data Storage:** The Users API currently uses an **in-memory list** (`List<User> _users`) inside `UsersController`. No database is connected at this stage. Data is reset upon server restart.

| Method | Endpoint | Description | Request Body | Response Status |
| ------ | -------- | ----------- | ------------ | --------------- |
| `GET` | `/api/users` | Returns list of all users | *None* | `200 OK` |
| `GET` | `/api/users/{id}` | Returns a single user by ID | *None* | `200 OK` or `404 Not Found` |
| `POST` | `/api/users` | Creates a new user in the in-memory list | JSON `User` object | `201 Created` (with `Location` header) |
| `PUT` | `/api/users/{id}` | **Full update:** Replaces both `Name` and `Email` of the user | JSON `User` object | `200 OK` or `404 Not Found` |
| `PATCH` | `/api/users/{id}` | **Partial update:** Updates `Name` and/or `Email` only if provided in request | JSON `User` object | `200 OK` or `404 Not Found` |
| `DELETE` | `/api/users/{id}` | Deletes a user by ID from the in-memory list | *None* | `204 NoContent` or `404 Not Found` |

#### Key Difference Between PUT and PATCH in Current Code:
* **`PUT /api/users/{id}`**: Expects a complete user object and unconditionally overwrites both `existingUser.Name` and `existingUser.Email` with the incoming values.
* **`PATCH /api/users/{id}`**: Performs partial updates. Checks `!string.IsNullOrEmpty(updatedUser.Name)` and `!string.IsNullOrEmpty(updatedUser.Email)` individually, modifying only fields that contain non-empty values.

### 3. Utility & Demonstration Endpoints

| Method | Endpoint | Description | Response Status |
| ------ | -------- | ----------- | --------------- |
| `GET` | `/hello` | Returns `"Hello World!"` | `200 OK` |
| `GET` | `/hello/miray` | Returns `"Hello Miray"` | `200 OK` |
| `GET` | `/sum/{number1}/{number2}` | Returns the sum of two integers | `200 OK` |

---

## 🛠️ Technology Stack

### Current Implementation (Active)

* **Backend Framework:** C# with **ASP.NET Core 9.0** (`net9.0`)
* **Architecture:** MVC (Model-View-Controller) with integrated Web API
* **Presentation Layer (Views):** Razor Views (`.cshtml`), HTML5, CSS3, Bootstrap 5.3.3, Bootstrap Icons
* **Data Storage:** In-memory static collection (`List<User>`)
* **API Documentation & Testing:** Swagger / OpenAPI via Swashbuckle (`Swashbuckle.AspNetCore` 6.6.2, `Microsoft.AspNetCore.OpenApi` 9.0.18)
* **Version Control:** Git & GitHub

### Planned Architecture (Future Phases)

* **Database:** **PostgreSQL** relational database for persistent data storage
* **ORM:** **Entity Framework Core** for database migrations, entity mapping, and queries
* **Additional Entities:** Alumni, Department, Graduation Details, Job / Experience models
* **Containerization:** **Docker** & **Docker Compose** for reproducible containerized deployment
* **Security:** Authentication and role-based authorization (Admin / Alumni / Faculty)
* **Service Layer:** Service classes to decouple business logic from controllers

---

## 🚀 How to Run

### 1. Clone the Repository

```bash
git clone https://github.com/miraydestan/alumni.git
cd alumni
```

### 2. Run the Application

```bash
dotnet run
```

The application runs locally at:

```text
http://localhost:5067
```

### 3. Access the Web Pages (Razor Views)

* **Home Page:** [http://localhost:5067/](http://localhost:5067/) or [http://localhost:5067/Home/Index](http://localhost:5067/Home/Index)
* **About Page:** [http://localhost:5067/about](http://localhost:5067/about) or [http://localhost:5067/Home/About](http://localhost:5067/Home/About)

### 4. Access Swagger UI (API Documentation)

Swagger UI is configured with route prefix `api/swagger` in [`Program.cs`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Program.cs):

```text
http://localhost:5067/api/swagger
```

Swagger provides an interactive web interface for inspecting and testing all API endpoints directly in your browser.

### 5. Test with HTTP Client

You can also run requests defined in [`Alumni.http`](file:///c:/Users/M%C4%B0RAY/Desktop/alumni/Alumni.http) directly from Visual Studio or the VS Code REST Client extension.

---

## 📊 Project Status: Implemented vs. Planned

| Feature / Component | Status | Details |
| ------------------- | ------ | ------- |
| **ASP.NET Core 9.0 Setup** | ✅ Implemented | Configured with `AddControllersWithViews`, routing, and static file serving. |
| **User Entity Model** | ✅ Implemented | `Models/User.cs` (`Id`, `Name`, `Email`). |
| **User CRUD API** | ✅ Implemented | `GET`, `POST`, `PUT`, `PATCH`, `DELETE` endpoints in `UsersController`. |
| **In-Memory Data Storage** | ✅ Implemented | In-memory `List<User>` used for rapid prototyping and testing. |
| **Health Check API** | ✅ Implemented | `GET` and `POST` at `/api/health`. |
| **Swagger / OpenAPI** | ✅ Implemented | Interactive API documentation at `/api/swagger`. |
| **Landing & About Razor Views** | ✅ Implemented | Responsive Bootstrap 5 views in `Views/Home/` with master layout. |
| **Static Assets** | ✅ Implemented | `wwwroot/css/site.css` served via `app.UseStaticFiles()`. |
| **PostgreSQL Database** | ⏳ Planned | Relational database setup for persistent data storage. |
| **Entity Framework Core ORM** | ⏳ Planned | Data context, migrations, and database access layer. |
| **Alumni & Related Models** | ⏳ Planned | Alumni, Department, Graduation, and Job Experience entities. |
| **Dynamic Entity Views (CRUD UI)** | ⏳ Planned | Razor forms and tables for managing Alumni and Users directly via UI. |
| **Authentication & Authorization** | ⏳ Planned | User login, registration, and role-based access control. |
| **Docker & Docker Compose** | ⏳ Planned | Containerization for consistent multi-container app and database setup. |

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
* Docker-based containerized setup with Docker Compose

---

## 📄 License

This project is licensed under the **MIT License**.
