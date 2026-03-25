# 🎓 Online Learning Platform

An enterprise-grade e-learning management system designed to facilitate interactive online classrooms, secure examinations, and comprehensive course administration. The system is built with a focus on clean code, scalability, and high performance.

## 🚀 Key Features

* **Robust Authentication & Authorization:** Implemented using ASP.NET Core Identity and JWT (JSON Web Tokens). Supports Role-Based Access Control (RBAC) with clear separation of privileges (e.g., Admin, Teacher, Student).
* **Standardized API Architecture:** Utilizes a unified `ApiResponse` wrapper and dynamic pagination logic (`PagedResult`, `PagingFilterBase`) for seamless frontend integration.
* **Secure Exam Management:** Comprehensive tracking of exams, sessions, and student attempts, including advanced cheating detection logs (`CheatLog`).
* **Centralized Error Handling:** Global Exception Handler middleware ensures that all unhandled exceptions are caught and formatted into standard HTTP responses without exposing stack traces to the client.

## 🛠️ Tech Stack & Architecture

### Backend
* **Framework:** .NET 8 / ASP.NET Core Web API
* **ORM:** Entity Framework Core (EF Core)
* **Security:** Microsoft.AspNetCore.Identity, BCrypt, JWT Bearer
* **Architecture Pattern:** Onion Architecture / Clean Architecture
* **Design Patterns:** Repository Pattern, Unit of Work (UoW), Dependency Injection (DI)

### Frontend
* **Library:** React.js (Component-driven UI)
* *(Add your state management tool here, e.g., Redux Toolkit or Zustand)*

## 📂 Project Structure (Onion Architecture)

The backend is strictly divided into decoupled layers to enforce the Dependency Inversion Principle:

1.  **Domain Layer:** Contains core business entities (`ApplicationUser`, `Classroom`, `Exam`, etc.), interfaces, and shared standard objects like `PagedResult`. No external dependencies.
2.  **Application Layer:** Contains business logic, Data Transfer Objects (DTOs), and service interfaces.
3.  **Infrastructure / DataAccess Layer:** Implements `IUnitOfWork` and specific repositories (`UserRepository`, `ExamRepository`). Handles database migrations and `AppDbContext`.
4.  **Web API (Presentation Layer):** Exposes RESTful endpoints, configures middlewares (JWT Validation, Global Exceptions), and registers DI containers.

## ⚙️ Getting Started

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Node.js](https://nodejs.org/) (for React frontend)
* SQL Server (or your preferred database)

### Installation

1. Clone the repository:
   ```bash
   git clone [https://github.com/your-username/your-repo-name.git](https://github.com/your-username/your-repo-name.git)
