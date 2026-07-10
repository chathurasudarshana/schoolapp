# SchApp — School Management System

A full-stack school management application built with **ASP.NET Core 9** and **Angular 20**.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 9 Web API |
| ORM | Entity Framework Core (two contexts, one SQL Server DB) |
| Auth | ASP.NET Core Identity + JWT (access + refresh tokens) |
| Logging | NLog → `C:/logs/SCH/` |
| API Docs | Swagger / OpenAPI |
| Frontend | Angular 20 (standalone, zoneless, SSR-capable) |
| UI Components | Angular Material, AG Grid Enterprise |
| Icons | Bootstrap Icons |

---

## Project Structure

```
SCH/
  SCH.sln
  SCH.Api/          # ASP.NET Core 9 Web API — startup project
  SCH.Core/         # DI wiring, filters, middleware
  SCH.Services/     # Business logic
  SCH.Repositories/ # EF Core contexts, repos, migrations
  SCH.Models/       # Domain entities + DTOs
  SCH.Mappings/     # AutoMapper profiles
  SCH.Shared/       # Custom logger, exceptions, utilities
  SCH.Tests/        # xUnit + Moq unit tests (service layer)
  SCH.Database.Core/ # SQL DDL files (documentation only)
  SCH.Client/       # Angular 20 frontend
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS)
- SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- Visual Studio 2022 (recommended) or VS Code

---

## Getting Started

### 1. Database

The API targets `(localdb)\MSSQLLocalDB`, database `SCH`. Apply migrations before first run:

```powershell
# Domain context (dbo schema)
dotnet ef migrations add MigrationInitial --context SCHContext --output-dir Migrations --project SCH\SCH.Repositories\SCH.Repositories.csproj --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj

# Identity context (identity schema)
dotnet ef migrations add MigrationInitial --context IdentityContext --output-dir Migrations/Identity --project SCH\SCH.Repositories\SCH.Repositories.csproj --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj

```

``` powershell
# Package Manager Console (Visual Studio — set Default project to SCH.Repositories)
# Domain context (dbo schema)
dotnet ef migrations add MigrationInitial --context SCHContext --output-dir Migrations --project SCH.Repositories\SCH.Repositories.csproj --startup-project SCH.Repositories\SCH.Repositories.csproj

Add-Migration <Name> -Context SCHContext -OutputDir Migrations -StartupProject SCH.Repositories
Update-Database -Context SCHContext -StartupProject SCH.Repositories

# Identity context (identity schema)
dotnet ef migrations add MigrationInitial --context IdentityContext --output-dir Migrations/Identity --project SCH\SCH.Repositories\SCH.Repositories.csproj --startup-project SCH.Repositories\SCH.Repositories.csproj

Add-Migration <Name> -Context IdentityContext -OutputDir Migrations/Identity -StartupProject SCH.Repositories
Update-Database -Context IdentityContext -StartupProject SCH.Repositories


```


See `MIGRATION_GUIDE.md` for the full runbook.

### 2. Backend

```powershell
# Build
dotnet build SCH\SCH.sln

# Run (HTTP :5071 / HTTPS :7190)
dotnet run --project SCH\SCH.Api\SCH.Api.csproj

# Run backend unit tests
dotnet test SCH\SCH.sln
```

When run via IIS Express (Visual Studio), the API is available at `https://localhost:44398`.

Swagger UI is available at `/swagger` when the app is running.

### 3. Frontend

```powershell
cd SCH\SCH.Client

npm install        # first time only
npm start          # dev server → http://127.0.0.1:63953
```

> **Note:** `public/config.json` controls the API URL and token timing — no rebuild needed to change them. Default points to `https://localhost:44398/api` (IIS Express). When using `dotnet run`, update `apiUrl` to `https://localhost:7190/api`.

---

## API Endpoints

All endpoints (except auth) require a valid JWT `Authorization: Bearer <token>` header.

| Controller | Base Route | Description |
|---|---|---|
| `AuthController` | `POST /api/auth/login` | Authenticate, returns JWT + refresh token |
| | `POST /api/auth/register` | Create a new user account |
| | `POST /api/auth/refresh` | Exchange refresh token for a new access token |
| | `POST /api/auth/logout` | Revoke refresh tokens |
| `StudentsController` | `GET /api/students` | List students (filter by active status) |
| | `GET /api/students/{id}` | Get student by ID |
| | `POST /api/students` | Create student |
| | `PATCH /api/students/{id}` | Update student |
| | `DELETE /api/students/{id}` | Delete student |
| `CoursesController` | `GET /api/courses` | List courses |
| | `GET /api/courses/{id}` | Get course by ID |
| | `POST /api/courses` | Create course |
| | `PATCH /api/courses/{id}` | Update course |
| | `DELETE /api/courses/{id}` | Delete course |
| `TeachersController` | `GET /api/teachers` | List teachers |
| | `PATCH /api/teachers/{id}` | Update teacher |
| `ImageController` | `POST /api/image` | Upload student/entity image |
| `CacheController` | `POST /api/cache/clear` | Clear all in-memory cache entries (Admin only) |
| | `DELETE /api/cache/{key}` | Remove a single cache entry by key (Admin only) |

---

## Frontend Features

- **Dashboard** — overview page
- **Students** — list, detail, course enrollment management
- **Courses** — list and detail views
- **Teachers** — list and detail views
- JWT auto-refresh via HTTP interceptor chain: `jwt → conflictError → unauthorized → serverError`
- Optimistic concurrency conflict handling (HTTP 409)

---

## Key Architecture Notes

- **Cache service**: `ICacheService` (`SCH.Shared/Cache/`) wraps `IMemoryCache` and is registered as a singleton. Default TTL is controlled by `AppSettings:CacheExpirationSeconds` (300 s). Currently used by `CoursesService` to cache the course list.
- **Policy-based authorization**: Named policies (`ViewStudents`, `EditStudents`, `EditTeachers`, `ClearCache`, etc.) gate every endpoint. Resource-based handlers enforce write-own access for Students and Teachers by comparing the route `id` against the user's `own_student_id` / `own_teacher_id` claim.
- **Auto-registration**: Services and repositories are discovered via `IService` / `IRepository` marker interfaces — no manual DI wiring needed.
- **Dual UnitOfWork**: `ISCHUnitOfWork` for domain data; `IIdentityUnitOfWork` for auth/identity.
- **Audit fields** (`CreatedBy`, `CreatedDate`, `ModifiedBy`, `ModifiedDate`) are set automatically on save from the JWT claim.
- **Optimistic concurrency**: All domain entities carry a `RowVersion` column. Send `RowVersion` in update requests; conflicts return HTTP 409.
- **Updates use `PATCH`**, not `PUT`.
- **Custom logger**: Inject `SCH.Shared.Logger.ILogger<T>`, not the standard `Microsoft.Extensions.Logging.ILogger<T>`.
- **Error response shape**: `{ message, data, trace }` — `trace` is hidden when `AppSettings:HideResponseErrors = true`.

---

## Authorization Policies

Policies are registered in `SCH.Api/Authorization/AuthorizationExtensions.cs`. Permission claim strings are defined in `SCH.Models.Auth.Constants.Permission` (claim type: `permission`).

| Policy | Allowed roles / claims |
|---|---|
| `ViewStudents` | Admin, Teacher, Student role, or `students:read` |
| `AddStudents` | Admin or `students:add` |
| `EditStudents` | Admin or `students:write`; Student with `students:write-own` may edit own record only |
| `DeleteStudents` | Admin or `students:remove` |
| `ViewTeachers` | Admin, Teacher role, or `teachers:read` |
| `EditTeachers` | Admin or `teachers:write`; Teacher with `teachers:write-own` may edit own record only |
| `DeleteTeachers` | Admin or `teachers:remove` |
| `ViewCourses` | Admin, Teacher, Student role, or `courses:read` |
| `AddCourses` | Admin or `courses:add` |
| `EditCourses` | Admin or `courses:write` |
| `DeleteCourses` | Admin or `courses:remove` |
| `ClearCache` | Admin role only |

Write-own policies use resource-based `IAuthorizationHandler` implementations (`StudentRecordEditAuthorizationHandler`, `TeacherRecordEditAuthorizationHandler`) that compare the route `id` to the user's `own_student_id` / `own_teacher_id` JWT claim.

---

## Backend Unit Tests

Tests use **xUnit + Moq** — no database required. All service dependencies are mocked.

| Test class | Service under test |
|---|---|
| `Students/StudentsServiceTests.cs` | `StudentsService` |
| `Courses/CoursesServiceTests.cs` | `CoursesService` |
| `Teachers/TeachersServiceTests.cs` | `TeachersService` |
| `IdentityUsers/IdentityUsersServiceTests.cs` | `IdentityUsersService` |
| `Images/ImageServiceTests.cs` | `ImageService` |

```powershell
dotnet test SCH\SCH.sln
```

---

## Password Policy

Configured in `appsettings.json` under `IdentitySettings`:

- Minimum 8 characters
- Requires digit, lowercase, uppercase
- Account lockout after 5 failed attempts (15-minute lockout)

---

## Configuration

| Key | Default | Description |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | LocalDB `SCH` | SQL Server connection string |
| `AppSettings:ImageFolder` | `C:\learn\student\images` | Root folder for uploaded images |
| `AppSettings:AllowImageExtensions` | `.jpg,.jpeg,.png,.gif` | Allowed image file types |
| `JwtSettings:AccessTokenExpirationMinutes` | `30` | Access token lifetime |
| `JwtSettings:RefreshTokenExpirationDays` | `7` | Refresh token lifetime |
| `AppSettings:CacheExpirationSeconds` | `300` | Default in-memory cache TTL in seconds |
| `AllowedOrigins` | `http://localhost:63953` | Angular dev server origin |
