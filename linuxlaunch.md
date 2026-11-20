# SCH App — Ubuntu / Linux launch guide

All paths below assume the repo is at:

```bash
/home/chathura/src/schoolapp
```

Key endpoints on Linux:

| Item | Value |
|---|---|
| Angular dev server | `http://127.0.0.1:63953` |
| API (Swagger at `/`) | `https://localhost:44398` |
| SQL Server | `localhost,1433`, user `sa`, password `StrongPass@123`, database `SCH` |
| Images | `/home/chathura/src/schappimages` |
| Logs | `/home/chathura/src/schapplogs` |

> IIS Express, LocalDB, and port `44398` via IIS are Windows-only. On Linux the API is Kestrel on `https://localhost:44398`, which is why `config.json` uses that URL.

---

## 1. Running the project

### 1.1 Start / stop Docker and SQL Server

Daily commands:

```bash
# Start Docker and the SQL Server container together
sudo systemctl start docker && sudo docker start sqlserver

# Stop the container and Docker together
sudo docker stop sqlserver && sudo systemctl stop docker.service docker.socket
```

Individual commands, when you need finer control:

```bash
sudo systemctl start docker
sudo systemctl stop docker.service docker.socket
sudo docker start sqlserver
sudo docker stop sqlserver
systemctl status docker
sudo docker ps
```

The container was already created with:

```bash
sudo docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=StrongPass@123" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Do **not** re-run `docker run` on an existing container; use `docker start sqlserver`.

### 1.2 Set the Linux runtime overrides

The committed config files keep their Windows defaults. The Linux values are supplied as environment variables in the shell where you start the API:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=https://localhost:44398
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=SCH;User Id=sa;Password=StrongPass@123;TrustServerCertificate=True;'
export AppSettings__ImageFolder=/home/chathura/src/schappimages
export SCH_LOG_DIR=/home/chathura/src/schapplogs
export AllowedOrigins='http://localhost:63953,http://127.0.0.1:63953'
```

`AllowedOrigins` is optional; the committed `appsettings.json` already includes both origins.

### 1.3 Run the backend

```bash
cd /home/chathura/src/schoolapp

# Build
dotnet build SCH/SCH.Api/SCH.Api.csproj

# Tests
dotnet test SCH/SCH.Tests/SCH.Tests.csproj

# Run with the Linux overrides from 1.2
dotnet run --project SCH/SCH.Api/SCH.Api.csproj --no-launch-profile
```

`--no-launch-profile` is essential: the `https` profile in `launchSettings.json` binds `https://localhost:7190;http://localhost:5071` and would override `ASPNETCORE_URLS`. The `IIS Express` profile does not work on Ubuntu.

Do not use `dotnet build SCH/SCH.sln` on Linux. The two `.sqlproj` files require Visual Studio SSDT targets and fail with `MSB4278`. They are documentation-only; all C# projects build fine individually.

### 1.4 Run the frontend

```bash
cd /home/chathura/src/schoolapp/SCH/SCH.Client
npm start
```

`npm start` runs `ng serve --host=127.0.0.1` and the port comes from `angular.json` (`63953`).

Equivalent direct command:

```bash
npm run ng -- serve --host=127.0.0.1 --port=63953
```

There is no need to install Angular CLI globally. The repo has CLI `22.2.7` in `node_modules`; check it with:

```bash
npm run ng -- version
```

`public/config.json` already points at `https://localhost:44398/api`. It is fetched at browser startup, so no rebuild is needed if you change it.

### 1.5 Run and debug from VS Code

VS Code currently has `/home/chathura/src` open, so the active debug configuration lives at:

```text
/home/chathura/src/.vscode/launch.json
```

A second copy for use if you open the repo folder directly is at:

```text
/home/chathura/src/schoolapp/.vscode/launch.json
```

Both define the same configuration, `SCH.Api (Linux 44398)`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "SCH.Api (Linux 44398)",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/schoolapp/SCH/SCH.Api/bin/Debug/net11.0/SCH.Api.dll",
      "cwd": "${workspaceFolder}/schoolapp/SCH/SCH.Api",
      "console": "integratedTerminal",
      "launchSettingsProfile": "",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "https://localhost:44398",
        "ConnectionStrings__DefaultConnection": "Server=localhost,1433;Database=SCH;User Id=sa;Password=StrongPass@123;TrustServerCertificate=True;",
        "AppSettings__ImageFolder": "/home/chathura/src/schappimages",
        "SCH_LOG_DIR": "/home/chathura/src/schapplogs",
        "AllowedOrigins": "http://localhost:63953,http://127.0.0.1:63953"
      }
    }
  ]
}
```

In the repo-root copy, `program` and `cwd` omit the `schoolapp/` segment:

```json
"program": "${workspaceFolder}/SCH/SCH.Api/bin/Debug/net11.0/SCH.Api.dll",
"cwd": "${workspaceFolder}/SCH/SCH.Api"
```

`"launchSettingsProfile": ""` tells the debugger to ignore `launchSettings.json`, which is what keeps the app on `44398` instead of `7190`.

Steps:

1. Build once: `dotnet build /home/chathura/src/schoolapp/SCH/SCH.Api/SCH.Api.csproj`
2. Open **Run and Debug** with `Ctrl+Shift+D`
3. Select **SCH.Api (Linux 44398)** in the dropdown
4. Press **F5**

Set breakpoints by clicking the gutter next to a line number. The `.vscode/` folders are git-ignored, so the password in `launch.json` is never committed.

---

## 2. Tools — what is needed and what is already installed

### 2.1 Already installed and configured on this machine

| Tool | Version / state |
|---|---|
| .NET SDK | `11.0.100` (`net11.0` matches every `.csproj`) |
| Node.js | `v26.13.1` (via nvm) |
| npm | `11.20.0` |
| Angular CLI | `22.2.7`, local to the project only |
| Docker | with running container `sqlserver` (SQL Server 2022, port `1433`) |
| VS Code | with SQL Server (mssql), .NET Install Tool, C#, C# Dev Kit, Angular Language Service extensions |
| EF Core CLI | `dotnet-ef 11.0.0` installed as a **local tool** via `dotnet-tools.json` |
| `libnss3-tools` | installed (`certutil` available) |
| HTTPS dev cert | trusted; `dotnet dev-certs https --check --trust` succeeds |
| npm packages | installed, including the `allowScripts` approvals in `package.json` |

### 2.2 Install on a fresh Ubuntu machine

```bash
# Docker (optional: disable auto-start to save resources)
sudo apt update && sudo apt install docker.io -y
sudo usermod -aG docker $USER
sudo systemctl disable docker.service docker.socket

# Git and VS Code
sudo apt install git -y
sudo snap install code --classic
code --install-extension ms-mssql.mssql eamodio.gitlens Angular.ng-template ms-dotnettools.csharp ms-dotnettools.csdevkit

# nvm, then Node (Angular 22 supports Node ^22.22.3, ^24.15.0 or >=26)
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.3/install.sh | bash
source ~/.bashrc
nvm install 26
node -v && npm -v

# .NET 11 SDK
sudo add-apt-repository ppa:dotnet/backports -y
sudo apt update && sudo apt install -y dotnet-sdk-11.0
dotnet --list-sdks

# EF Core CLI (local to this repo, already in git)
cd /home/chathura/src/schoolapp
dotnet tool restore
dotnet ef --version

# Browser trust for the HTTPS dev certificate
sudo apt install -y libnss3-tools
echo 'export SSL_CERT_DIR="$HOME/.aspnet/dev-certs/trust:/usr/lib/ssl/certs"' >> ~/.bashrc
source ~/.bashrc
dotnet dev-certs https --trust
dotnet dev-certs https --check --trust
```

Optional, only for `npm test` (Karma is configured with `karma-chrome-launcher` and `browsers: ['Chrome']` — Firefox will not work without changing the config):

```bash
sudo apt install -y chromium
export CHROME_BIN=/usr/bin/chromium
```

### 2.3 Common setup mistakes to avoid

These are common setup assumptions that are wrong for this repo:

| Misconception | Correct value |
|---|---|
| .NET 9 SDK / `dotnet-sdk-9.0` | **.NET 11 SDK** / `dotnet-sdk-11.0` — all projects target `net11.0` |
| `npm install -g @angular/cli` | Not needed; CLI `22.2.7` is local. Use `npm run ng -- …` or `npx ng …` |
| `ng new my-app` | Not applicable; this repo already contains the Angular app |
| "Latest LTS Node" | Must be `^22.22.3`, `^24.15.0`, or `>=26` for Angular 22 |
| No cert-trust step | On Linux you must install `libnss3-tools`, set `SSL_CERT_DIR`, and run `dotnet dev-certs https --trust` |
| No EF CLI step | `dotnet-ef 11.0.0` is required for migrations; installed locally in this repo |
| No test-browser note | Karma requires Chrome/Chromium, not Firefox |
| `Trusted_Connection` / LocalDB | Docker SQL uses `sa` auth with `TrustServerCertificate=True` |

Not needed on Ubuntu: **IIS Express**, **LocalDB**, **global Angular CLI**, **global `dotnet-ef`**, and the `44398` IIS Express binding.

---

## 3. Project setup completed so far

### 3.1 Configuration approach

The committed config files remain Windows-friendly:

- `SCH/SCH.Client/public/config.json` → `apiUrl: https://localhost:44398/api`
- `SCH/SCH.Api/appsettings.json` → LocalDB connection string, `C:\learn\student\images`, both CORS origins
- `SCH/SCH.Api/nlog.config` → uses an environment variable with a Windows fallback:

```xml
<target name="logfile" xsi:type="File"
        fileName="${environment:variable=SCH_LOG_DIR:default=C\:/logs/SCH}/app-log-${shortdate}.txt" />
```

The `\:` escape is required; without it NLog parses only `C` and ignores `SCH_LOG_DIR`.

Linux values come from section 1.2, so nothing machine-specific is committed. `README.md` was updated from .NET 9 to .NET 11.

### 3.2 EF Core local tool

```bash
cd /home/chathura/src/schoolapp
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 11.0.0
```

This created `dotnet-tools.json` at the repo root. On another machine, `dotnet tool restore` reinstalls it.

### 3.3 HTTPS development certificate

Installed `libnss3-tools`, added `SSL_CERT_DIR` to `~/.bashrc`, ran `dotnet dev-certs https --trust`, and verified with `dotnet dev-certs https --check --trust`. Restart any browser that was open before the trust was added.

### 3.4 Frontend dependencies

```bash
cd SCH/SCH.Client
npm install
```

`package.json` contains an `allowScripts` block approving `esbuild`, `@parcel/watcher`, `lmdb`, and `msgpackr-extract`, so their postinstall steps are no longer skipped.

### 3.5 Database — migrations, not `.sqlproj` publish

The `.sqlproj` files are documentation only. The database was created and populated with EF migrations plus two manually applied stored procedures and a seed script.

```bash
cd /home/chathura/src/schoolapp
export SA_PASSWORD='StrongPass@123'
export DB_CONN='Server=localhost,1433;Database=SCH;User Id=sa;Password=StrongPass@123;TrustServerCertificate=True;'

# 1. Create the database
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "IF DB_ID('SCH') IS NULL CREATE DATABASE SCH;"

# 2. Apply both migrations (do NOT run `migrations add`; they are already in the repo)
dotnet ef database update --context SCHContext --project SCH/SCH.Repositories/SCH.Repositories.csproj --startup-project SCH/SCH.Repositories/SCH.Repositories.csproj --connection "$DB_CONN"

dotnet ef database update --context IdentityContext --project SCH/SCH.Repositories/SCH.Repositories.csproj --startup-project SCH/SCH.Repositories/SCH.Repositories.csproj --connection "$DB_CONN"

# 3. Stored procedures used by the student grid and dashboard
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -d SCH < "SCH/SCH.Database/dbo/Stored Procedures/GetStudentGrid.sql"
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -d SCH < "SCH/SCH.Database/dbo/Stored Procedures/GetCourseStudentCount.sql"

# 4. Seed roles, claims, users, students and courses
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -d SCH < "SCH/SCH.Database.Core/PostScript/Script.PostDeployment.sql"
```

Notes on fixes already applied:

- `GetStudentGrid.sql` had a UTF-8 BOM that made `CREATE PROCEDURE` fail with `Incorrect syntax near '﻿'`. The BOM was removed from the file, so the plain command above now works.
- `Script.PostDeployment.sql` now starts with `SET QUOTED_IDENTIFIER ON;` + `GO`, which is required because the Identity tables have filtered indexes. Without it the seed fails with `Msg 1934` (and a follow-on `Msg 515` on `CreatedBy`). This also means `-I` is no longer needed on `sqlcmd`.
- A stray `Ww==',` after a comment on the `studentviewuser` hash line was removed (comment-only cleanup; the hash itself was correct).

Verification:

```bash
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -d SCH -Q "SELECT (SELECT COUNT(*) FROM [identity].[AspNetUsers]) AS Users, (SELECT COUNT(*) FROM [identity].[AspNetRoles]) AS Roles, (SELECT COUNT(*) FROM dbo.Student) AS Students, (SELECT COUNT(*) FROM dbo.Course) AS Courses;"
```

Expected: `Users 9`, `Roles 4`, `Students 6`, `Courses 4`. Seeded logins include `admin` / `Admin123!` and several `Basic123!` users.

### 3.6 Debug configuration

Created `.vscode/launch.json` (section 1.5) so F5 starts the API with the Linux environment and port `44398`. These files are git-ignored.

### 3.7 Build status

All C# projects build successfully. `dotnet build SCH/SCH.sln` still reports `MSB4278`/`NU1503` for the two SSDT `.sqlproj` projects on Linux; this is expected and does not affect running or debugging the app.
