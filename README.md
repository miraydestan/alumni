# 🎓 Alumni Tracking System

## 📖 About the Project

The **Alumni Tracking System** is a web-based application designed to help universities manage and track information about their alumni in an organized and centralized way.

The system will allow users to manage alumni records together with related information such as departments, graduation details, and professional experiences. It will also provide search and filtering capabilities for easier access to alumni information.

The application will be developed incrementally, with new features and improvements introduced throughout the development process.

---

## 🏛️ Architecture and Design Decisions

The following technologies and architectural decisions were selected based on the requirements of the system:

1. **ASP.NET Core MVC:**
   ASP.NET Core MVC will be used to build the web application and provide a clear separation between the user interface, application logic, and request handling.

2. **Entity Framework Core:**
   Entity Framework Core will be used as the ORM to manage communication between the application and the database. It will also be used to manage relationships between entities.

3. **PostgreSQL:**
   PostgreSQL will be used as the relational database management system because the system contains related data such as alumni, departments, graduation information, and job experiences.

4. **Layered Application Structure:**
   Controllers, services, and data access responsibilities will be separated to make the application easier to maintain and extend.

5. **Docker:**
   Docker and Docker Compose will be used to provide a consistent development environment and simplify application and database setup.

---

## 🛠️ Technology Stack

### Backend

**C# with ASP.NET Core**

ASP.NET Core is used to build the web application, handle requests, implement CRUD operations, and provide authentication and authorization.

### Frontend

**ASP.NET Core MVC with Razor Views**

Razor Views, HTML5, CSS3, and Bootstrap will be used to create the web-based user interface.

### Database

**PostgreSQL**

PostgreSQL is used as the relational database for storing alumni and related information.

### ORM

**Entity Framework Core**

Entity Framework Core is used to communicate with PostgreSQL and manage database entities and relationships.

### Containerization

**Docker & Docker Compose**

Docker will be used to provide a consistent development environment for the application and database.

### Version Control

**Git & GitHub**

Git and GitHub are used for version control and tracking the development process.

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

## 🐳 How to Run

The final goal is to make the application and database runnable through Docker Compose.

### 1. Clone the Repository

```bash
git clone https://github.com/miraydestan/alumni.git
cd alumni
```

### 2. Start the Application

After the Docker configuration is completed, the application will be started with:

```bash
docker compose up --build
```

Detailed setup and usage instructions will be added as the project develops.

---

## 📂 Planned Project Structure

```text
alumni/
│
├── Controllers/          # Handles HTTP requests
├── Models/               # Application and database models
├── Data/                 # Database context and configuration
├── Services/             # Business logic
├── Views/                # Razor Views
├── wwwroot/              # CSS, JavaScript and static files
│
├── Migrations/           # Entity Framework Core migrations
├── Dockerfile            # Application container configuration
├── docker-compose.yml    # Application and PostgreSQL configuration
├── Program.cs            # Application entry point
└── README.md             # Project documentation
```

---

## 📊 Project Status

**In Development**

The initial repository and technology stack have been established. The application will be developed incrementally, starting with the database structure and core alumni management features.

---

## 📄 License

This project is licensed under the **MIT License**.
