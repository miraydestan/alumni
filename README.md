# 🎓 Alumni Tracking System

## 📖 About the Project

The **Alumni Tracking System** is a web-based application designed to help universities manage and track information about their alumni in an organized and centralized way.

The system will allow users to manage alumni records together with related information such as departments, graduation details, and professional experiences. It will also provide search and filtering capabilities for easier access to alumni information.

The application is being developed incrementally, with new features and improvements introduced throughout the development process.

---

## 🏛️ Architecture and Design Decisions

The following technologies and architectural decisions were selected based on the requirements of the system:

1. **ASP.NET Core MVC:**
   ASP.NET Core MVC is used to build the web application and provide a clear separation between the user interface, application logic, and request handling.

2. **Entity Framework Core:**
   Entity Framework Core is planned to be used as the ORM to manage communication between the application and the database.

3. **PostgreSQL:**
   PostgreSQL is planned to be used as the relational database management system for storing alumni and related information.

4. **Layered Application Structure:**
   Controllers, services, and data access responsibilities will be separated as the application grows to make the system easier to maintain and extend.

5. **Docker:**
   Docker and Docker Compose are planned to be used to provide a consistent development environment and simplify application and database setup.

---

## 🛠️ Technology Stack

### Backend

**C# with ASP.NET Core**

ASP.NET Core is used to build the web application, handle HTTP requests, and implement API endpoints.

### Frontend

**ASP.NET Core MVC with Razor Views**

Razor Views, HTML5, CSS3, and Bootstrap will be used to create the web-based user interface.

### Database

**PostgreSQL**

PostgreSQL is planned to be used as the relational database for storing alumni and related information.

### ORM

**Entity Framework Core**

Entity Framework Core is planned to be used to communicate with PostgreSQL and manage database entities and relationships.

### API Documentation

**Swagger / OpenAPI**

Swagger is used to document and test the API endpoints.

Swagger UI:

```text
http://localhost:5067/swagger
```

### Containerization

**Docker & Docker Compose**

Docker will be used to provide a consistent development environment for the application and database.

### Version Control

**Git & GitHub**

Git and GitHub are used for version control and tracking the development process.

---

## 🔌 Current API Endpoints

The following API endpoints are currently implemented.

### Health

| Method | Endpoint      | Description                   |
| ------ | ------------- | ----------------------------- |
| GET    | `/api/health` | Returns the API health status |
| POST   | `/api/health` | Returns the API health status |

### Users

The Users API currently uses an **in-memory list** for development and testing. No database is used at this stage.

| Method | Endpoint          | Description          |
| ------ | ----------------- | -------------------- |
| GET    | `/api/users`      | Returns all users    |
| GET    | `/api/users/{id}` | Returns a user by ID |
| POST   | `/api/users`      | Creates a new user   |
| PUT    | `/api/users/{id}` | Updates a user       |
| PATCH  | `/api/users/{id}` | Updates a user       |
| DELETE | `/api/users/{id}` | Deletes a user       |

---

## 📌 Planned Features

The system is planned to support:

* Adding, viewing, updating, and deleting alumni records
* Searching and filtering alumni
* Managing department information
* Managing graduation information
* Managing professional and job experience information
* User authentication and authorization
* Basic dashboard statistics
* A web-based user interface
* Relational database management
* Persistent data storage with PostgreSQL
* Entity Framework Core database integration
* Docker-based application and database setup

---

## 🏗️ Architecture

The planned application structure is:

```text
Browser
   ↓
ASP.NET Core MVC
   ↓
Controllers / Services
   ↓
Entity Framework Core
   ↓
PostgreSQL
```

This structure separates the presentation layer, application logic, and database operations.

The architecture will evolve as the application grows.

---
## 🚀 How to Run

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

The application is currently available at:

```text
http://localhost:5067
```

### 3. Open Swagger

```text
http://localhost:5067/api/swagger
```

Swagger provides an interface for viewing and testing the available API endpoints.


### Future Docker Setup

After Docker and PostgreSQL integration are completed, the application will be started with:

```bash
docker compose up --build
```

---

## 📂 Current Project Structure

```text
alumni/
│
├── Controllers/
│   ├── HealthController.cs
│   ├── UsersController.cs
│   └── ...
│
├── Models/
│   ├── User.cs
│   └── ...
│
├── Views/
│   └── ...
│
├── wwwroot/
│   └── ...
│
├── Alumni.csproj
├── Alumni.http
├── Program.cs
└── README.md
```

---

## 📊 Project Status

**In Development**

### Currently Completed

* ASP.NET Core project setup
* Basic API routing
* Health check endpoints
* User model
* User CRUD API endpoints
* In-memory user storage for development
* Swagger / OpenAPI integration
* GitHub repository and version control setup

### Planned

* Alumni entity and CRUD operations
* PostgreSQL database integration
* Entity Framework Core integration
* Department management
* Graduation information
* Job and professional experience management
* Search and filtering
* Authentication and authorization
* Dashboard statistics
* Razor Views and Bootstrap UI improvements
* Docker and Docker Compose setup

The README and Swagger documentation will be updated as new features and API endpoints are added.

---

## 📄 License

This project is licensed under the **MIT License**.
