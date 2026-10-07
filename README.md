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
  * [Models/UserStore.cs](Models/UserStore.cs): Provides a shared in-memory data store (`public static List<User> Users { get; } = new();`) shared by both MVC and Web API controllers.
* **Data Storage Status (In-Memory):**
  * Data persistence is currently handled **in-memory** using a static collection in `UserStore.cs`.
  * **No database is connected at this stage.** Relational database storage (**PostgreSQL**) and Object-Relational Mapping (**Entity Framework Core**) are **planned for future iterations** and are **not yet implemented**. When the application stops or restarts, the in-memory data resets.

---

### 2. CONTROLLERS (Request Processing & Coordination)

The project demonstrates both ASP.NET Core approaches for user management: an **MVC Controller** returning Razor Views and a **Web API Controller** returning JSON.

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

3. [Controllers/HealthController.cs](Controllers/HealthController.cs):
   * Inherits from `ControllerBase`, route `api/[controller]`.
   * Provides API health check endpoints (`GET` and `POST` at `/api/health`) returning `{ status = "ok" }`.

4. [Controllers/HomeController.cs](Controllers/HomeController.cs):
   * Inherits from `Controller`.
   * Serves landing and platform information Razor Views (`Index` and `About`).

5. [Controllers/AlumniController.cs](Controllers/AlumniController.cs):
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
  * **Home Views ([Views/Home/](Views/Home/)):**
    * [Views/Home/Index.cshtml](Views/Home/Index.cshtml): Main landing page with hero banner and feature cards.
    * [Views/Home/About.cshtml](Views/Home/About.cshtml): Platform info, tracked categories, and goals.
  * **Shared Layout & Assets:**
    * [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml): Master layout with responsive navbar (Home, About, Users links), footer, Bootstrap 5.3, and icons.
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
|   +-- ApiUserController.cs             # RESTful CRUD Web API endpoints (/api/users)
|   +-- HealthController.cs              # API health verification endpoints (/api/health)
|   +-- HomeController.cs                # MVC controller returning landing Razor Views (/Home/Index, /Home/About)
|   `-- UserController.cs                # MVC controller for User CRUD web views (/User, /User/Create, etc.)
|
+-- Models/                              # Data models and shared state
|   +-- User.cs                          # User data entity model (Id, Name, Email)
|   `-- UserStore.cs                     # Shared in-memory data store (List<User>)
|
+-- Views/                               # Razor Views (UI presentation layer)
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
+-- Program.cs                           # Application entry point: service registration, middleware, and routing
+-- README.md                            # Comprehensive project documentation
`-- LICENSE                              # Project license (MIT)
```

### Detailed File Responsibilities

| File / Folder | Type | Responsibility |
| ------------- | ---- | -------------- |
| [Program.cs](Program.cs) | Configuration | Configures dependency injection (`AddControllersWithViews`, `AddSwaggerGen`), middleware pipeline, URL rewrites, and endpoint routing. |
| [Alumni.csproj](Alumni.csproj) | Project File | Defines target framework (`net9.0`) and NuGet dependencies (`Swashbuckle.AspNetCore`, `Microsoft.AspNetCore.OpenApi`). |
| [Alumni.http](Alumni.http) | Testing | Pre-configured HTTP requests for testing API endpoints (`/api/health`, `/api/users`, `POST`, `PUT`, `PATCH`, `DELETE`). |
| [Models/User.cs](Models/User.cs) | Model | Defines the `User` class schema (`Id`, `Name`, `Email`). |
| [Models/UserStore.cs](Models/UserStore.cs) | Storage | Shared static in-memory collection (`List<User> Users`) shared between `UserController` and `ApiUserController`. |
| [Controllers/UserController.cs](Controllers/UserController.cs) | Controller (MVC) | Handles web user requests, performs CRUD on `UserStore.Users`, and returns Razor views (`Index`, `Details`, `Create`, `Edit`, `Delete`). |
| [Controllers/ApiUserController.cs](Controllers/ApiUserController.cs) | Controller (API) | RESTful API controller exposing `/api/users` endpoints (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`) with JSON responses. |
| [Controllers/HealthController.cs](Controllers/HealthController.cs) | Controller (API) | Provides GET and POST health endpoints returning `{ status = "ok" }`. |
| [Controllers/HomeController.cs](Controllers/HomeController.cs) | Controller (MVC) | MVC Controller returning Razor views for `Index` and `About` pages. |
| [Controllers/AlumniController.cs](Controllers/AlumniController.cs) | Controller (API) | API Controller containing test routes (`/hello`, `/sum`) and fallback responses. |
| [Views/User/](Views/User/) | Views (MVC) | Razor views for User CRUD operations (`Index`, `Details`, `Create`, `Edit`, `Delete`). |
| [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml) | View | Master layout defining the standard page skeleton, navigation bar with Users link, and footer. |
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

### 4. Utility & Demonstration Endpoints

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
* **Data Storage:** Shared in-memory static collection (`UserStore.Users`)
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

* **Home Page:** [http://localhost:5067/](http://localhost:5067/)
* **About Page:** [http://localhost:5067/about](http://localhost:5067/about)
* **User Management (MVC):** [http://localhost:5067/User](http://localhost:5067/User)

### 4. Access Swagger UI (API Documentation)

Swagger UI is configured with route prefix `api/swagger` in [Program.cs](Program.cs):

```text
http://localhost:5067/api/swagger
```

Swagger provides an interactive web interface for inspecting and testing all API endpoints directly in your browser:

* **Swagger UI:** [http://localhost:5067/api/swagger](http://localhost:5067/api/swagger)

### 5. Test with HTTP Client

You can also run requests defined in [Alumni.http](Alumni.http) directly from Visual Studio or the VS Code REST Client extension.

---

## 📊 Project Status: Implemented vs. Planned

| Feature / Component | Status | Details |
| ------------------- | ------ | ------- |
| **ASP.NET Core 9.0 Setup** | ✅ Implemented | Configured with `AddControllersWithViews`, routing, and static file serving. |
| **User Entity Model** | ✅ Implemented | `Models/User.cs` (`Id`, `Name`, `Email`). |
| **Shared In-Memory Store** | ✅ Implemented | `Models/UserStore.cs` (`List<User> Users`) shared across MVC and API controllers. |
| **User MVC Controller & Views** | ✅ Implemented | `Controllers/UserController.cs` with full Razor CRUD views in `Views/User/`. |
| **User Web API Controller** | ✅ Implemented | `Controllers/ApiUserController.cs` exposing RESTful `/api/users` endpoints. |
| **Health Check API** | ✅ Implemented | `GET` and `POST` at `/api/health`. |
| **Swagger / OpenAPI** | ✅ Implemented | Interactive API documentation at `/api/swagger`. |
| **Responsive Bootstrap UI** | ✅ Implemented | Layout with navigation header, hero cards, and tables. |
| **Static Assets** | ✅ Implemented | `wwwroot/css/site.css` served via `app.UseStaticFiles()`. |
| **PostgreSQL Database** | ⏳ Planned | Relational database setup for persistent data storage. |
| **Entity Framework Core ORM** | ⏳ Planned | Data context, migrations, and database access layer. |
| **Alumni & Related Models** | ⏳ Planned | Alumni, Department, Graduation, and Job Experience entities. |
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
