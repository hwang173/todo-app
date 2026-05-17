# Todo App

A simple full-stack TODO application built with Angular and .NET Web API.

---

# Features

- View todo items
- Add todo items
- Delete todo items
- In-memory backend storage
- Reactive Angular frontend using RxJS
- Swagger API documentation

---

# Test

Complete testing documentation is available in [TESTS.md](./TESTS.md)

---

# Tech Stack

## Frontend

- Angular
- TypeScript
- RxJS
- Angular Forms
- Angular HttpClient

## Backend

- .NET Web API
- ASP.NET Core
- Swagger / OpenAPI

---

# Architecture

## Frontend

The Angular frontend uses:

- Standalone Components
- Reactive RxJS streams
- Service-based API communication
- Observable-based state refresh

## Backend

The .NET backend uses:

- REST API architecture
- Controller + Service separation
- Dependency Injection
- In-memory data storage

---

# Project Structure

```txt
todo-app/

├── frontend/
│   ├── src/
│   │   └── app/
│   │       ├── services/
│   │       ├── models/
│   │       ├── app.ts
│   │       ├── app.html
│   │       └── app.css
│
└── backend/
    └── TodoApi/
        ├── Controllers/
        ├── Services/
        ├── Models/
        └── Program.cs
```

---

# Prerequisites

Before running the application, ensure the following tools are installed:

- Node.js
- Angular CLI
- .NET SDK
- Git

---

# Running the Application

## 1. Clone the Repository

```bash
git clone <your-repository-url>
```

Navigate into the project folder:

```bash
cd todo-app
```

## Backend Setup

Navigate to the backend project:

```bash
cd backend/TodoApi
```

Restore NuGet packages:

```bash
dotnet restore
```

Run the API:

```bash
dotnet run
```

The backend API will run on: `http://localhost:5209`

Swagger UI: `http://localhost:5209/swagger`

## Frontend Setup

Open a new terminal.

Navigate to the frontend project:

```bash
cd frontend/todo-ui
```

Install npm packages:

```bash
npm install
```

Run the Angular application:

```bash
ng serve
```

The frontend application will run on: `http://localhost:4200`

---

# Notes

- The backend stores data in memory only
- No database setup is required
- CORS is configured for local Angular development
- Swagger is enabled for API testing
- The Angular frontend uses RxJS-based reactive refresh handling

---

# Example Workflow

1. Start the backend API
2. Start the Angular frontend
3. Open the frontend in the browser
4. Add and delete todo items
5. Verify API endpoints using Swagger

---

# API Endpoints

## Get Todos

```
GET /api/todos
```

## Add Todo

```
POST /api/todos
```

Request Body:

```json
{
  "title": "Buy milk"
}
```

## Delete Todo

```
DELETE /api/todos/{id}
```

---

# Development Notes

This project was created as a technical assessment to demonstrate:

- Angular frontend development
- .NET Web API development
- RESTful API communication
- Dependency Injection
- Reactive programming with RxJS
- Basic full-stack application architecture

---

# Author

Created by He Wang