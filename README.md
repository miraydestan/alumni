# Alumni Tracking System

A web-based application designed to help universities manage and track information about their alumni in an organized way.

## About

The Alumni Tracking System provides a centralized platform for storing, viewing, searching, and managing alumni information.

The system is designed to manage alumni records along with related information such as departments, graduation details, and professional information. The application will be developed incrementally, with additional features and improvements introduced throughout development.

## Features

The system is planned to support:

* Adding, viewing, updating, and deleting alumni records
* Searching and filtering alumni
* Managing department and graduation information
* Managing professional and job-related information
* User authentication and authorization
* A web-based user interface
* Relational database management

## Technology Stack

### Backend

**C# with ASP.NET Core**

ASP.NET Core was chosen for building a structured web application with features such as routing, CRUD operations, middleware, authentication, and database integration.

### Database

**PostgreSQL**

PostgreSQL was chosen because the system contains relational data, such as alumni, departments, graduation information, and jobs. It is an open-source relational database suitable for managing these relationships.

### Containerization

**Docker**

Docker will be used to provide a consistent development and deployment environment.

## Architecture

The planned application structure is:

```text
Browser
   ↓
ASP.NET Core
   ↓
Controllers / Services
   ↓
ORM
   ↓
PostgreSQL
```

The architecture will evolve as the application grows.

## How to Run

The final goal is to make the entire application runnable with a single command:

```bash
docker compose up
```

Detailed setup and usage instructions will be added as the project develops.

## Project Status

**In Development**

The initial repository and technology stack have been established. Development of the application will continue incrementally.

## License

This project is licensed under the **MIT License**.
