# AGENTS.md

## Repo Layout

```
SCH/
  SCH.sln
  SCH.Api/          # ASP.NET Core 9 Web API — startup project
  SCH.Core/         # DI wiring, filters, middleware (no business logic)
  SCH.Services/     # Business logic
  SCH.Repositories/ # EF Core contexts, repos, migrations
  SCH.Models/       # Domain entities + DTOs
  SCH.Mappings/     # AutoMapper profiles
  SCH.Shared/       # Cross-cutting: exceptions, custom logger, utilities
  SCH.Tests/        # Empty — no backend tests yet
  SCH.Database.Core/ # SQL .sqlproj DDL files (documentation only, not deployed)
  SCH.Client/       # Angular 20 frontend
```

---

## Backend (.NET 9)

### Commands

```powershell
# Build
dotnet build SCH\SCH.sln

# Run API (HTTP :5071, HTTPS :7190)
dotnet run --project SCH\SCH.Api\SCH.Api.csproj

# Run tests (project is currently empty)
dotnet test SCH\SCH.sln
```

When run via IIS Express (Visual Studio), the API is on `https://localhost:44398`.

### EF Core Migrations — Two Contexts, One DB

There are two independent EF contexts targeting the same SQL Server database (`(localdb)\MSSQLLocalDB`, database `SCH`):

| Context | Schema | Migration folder |
|---|---|---|
| `SCHContext` | `dbo` | `SCH.Repositories/Migrations/` |
| `IdentityContext` | `identity` | `SCH.Repositories/Migrations/Identity/` |

**Always specify `-Context` and `-OutputDir` explicitly.** The startup project for migrations is `SCH.Repositories` (it has its own `IDesignTimeDbContextFactory` implementations).

```powershell
# SCHContext
dotnet ef migrations add <Name> --context SCHContext --output-dir Migrations \
  --project SCH\SCH.Repositories\SCH.Repositories.csproj \
  --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj

# IdentityContext
dotnet ef migrations add <Name> --context IdentityContext --output-dir Migrations/Identity \
  --project SCH\SCH.Repositories\SCH.Repositories.csproj \
  --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj
```

See `MIGRATION_GUIDE.md` for the full runbook.

### Reflection-Based Auto-Registration

Services, repositories, and UnitOfWork implementations are **auto-registered via assembly scanning** at startup. To register a new service or repository:

- Implement the `IService` marker interface (services) or `IRepository` marker interface (repositories)
- The concrete class must implement **exactly one** interface that inherits from the marker — `Single(...)` will throw if more than one matches

### Custom Logger — Not the Standard .NET One

Inject `SCH.Shared.Logger.ILogger<T>`, **not** `Microsoft.Extensions.Logging.ILogger<T>`. The custom interface wraps NLog. Logs go to `C:/logs/SCH/app-log-<date>.txt`.

### Dual UnitOfWork

- Domain operations → `ISCHUnitOfWork`
- Auth/identity operations → `IIdentityUnitOfWork`
- Cross-context operations (e.g., registration) are handled manually with try/catch rollback — no distributed transactions.

### Audit Fields Are Set Automatically

`SaveChangesAsync()` in both UnitOfWork implementations auto-sets `CreatedBy`, `CreatedDate`, `ModifiedBy`, `ModifiedDate` from the JWT claim. **Do not set these manually in service or controller code.**

### Optimistic Concurrency

All domain entities have a `RowVersion byte[]` column. The frontend must send `RowVersion` in update requests. Conflicts surface as HTTP 409 (`SCHErrorNumber.ConcurrencyConflict`).

### API Conventions

- Updates use `PATCH`, not `PUT`.
- The URL `id` is assigned to the DTO inside the controller; the body `Id` field is ignored.
- Error shape: `{ message, data, trace }`. `trace` is omitted when `AppSettings:HideResponseErrors` is `true`.
- `dbo.User.Id` mirrors `identity.AspNetUsers.Id` — this cross-schema link is manual; no DB-level FK enforces it.

---

## Frontend (Angular 20)

All commands run from `SCH\SCH.Client\`.

```powershell
npm install          # first time
npm start            # dev server → http://127.0.0.1:63953
npm run build        # production build → dist/
npm run watch        # dev build with watch
npm test             # Karma + Jasmine (opens Chrome)
```

### Runtime Config, Not Environment Files

`src/environments/` is git-ignored and unused. All environment config lives in `public/config.json`, which is fetched at browser startup in `main.ts` and injected via `APP_CONFIG`. To change the API URL or token timing, edit `public/config.json` — no rebuild needed.

Default `config.json` points to `https://localhost:44398/api` (IIS Express). When running `dotnet run` instead, update `apiUrl` to `https://localhost:7190/api`.

### Zoneless Change Detection

The app uses `provideZonelessChangeDetection()`. `zone.js` is **not included** in the app polyfills (only in the test build). Avoid `NgZone`; use signals and `async`/`await`.

### Standalone Components Only

All components are standalone. No `NgModule`s. Route-scoped providers are used instead of module providers.

### Port Coupling

Angular dev server is locked to `127.0.0.1:63953` (set in `angular.json`). This port is whitelisted in `SCH.Api/appsettings.json` under `AllowedOrigins`. If the port changes, update both files.

### Service and File Naming Conventions

- Angular class names have **no** `Component`/`Service` suffix (e.g., class `Auth`, class `StudentListPage`). Exception: `sidenav.service.ts`.
- API service files use the `*-api.ts` suffix (e.g., `student-api.ts`).
- Component selector prefix: `sch-`.
- Interfaces live in `src/app/interfaces/` (app-wide) or `src/app/sch/interfaces/` (feature-scoped).

### HTTP Interceptor Order

`jwt → conflictError → unauthorized → serverError`

### Route-Scoped API Services

`StudentApi`, `CourseApi`, `ImageApi` are provided at the route level in `sch.routes.ts`, not globally. `Auth` is `providedIn: 'root'`.

### Running a Focused Frontend Test

Karma does not support a file filter from the CLI. Use Jasmine's `fit` / `fdescribe` to isolate tests:

```typescript
fit('my test', () => { ... });
fdescribe('MyComponent', () => { ... });
```

---

## No CI/CD

No GitHub Actions, Azure Pipelines, Dockerfile, or deployment scripts exist. Development uses Visual Studio 2022 + IIS Express locally.
