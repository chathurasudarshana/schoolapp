# Database Migration Guide - Dual Context Architecture

## 📋 Overview

Your application now uses **TWO separate DbContexts**:
- **IdentityContext** → `identity` schema (Auth tables)
- **SCHContext** → `dbo` schema (Domain tables + User table)

Both contexts use the **same database** but different schemas.

---

## ✅ Current State

Initial migrations are **already applied**. Both contexts have been set up and the database schemas are in place.

```
SCH/SCH.Repositories/
├── Migrations/
│   ├── 20260204123732_MigrationInitial.cs      ✅ Applied
│   ├── 20260204123732_MigrationInitial.Designer.cs
│   ├── SCHContextModelSnapshot.cs
│   └── Identity/
│       ├── 20260204123805_MigrationInitial.cs  ✅ Applied
│       ├── 20260204123805_MigrationInitial.Designer.cs
│       └── IdentityContextModelSnapshot.cs
└── DbContexts/
    ├── IdentityContext.cs         ✅
    ├── IdentityContextFactory.cs  ✅
    ├── SCHContext.cs              ✅
    └── SCHContextFactory.cs       ✅
```

---

## 🔧 Step 2: Adding Future Migrations

Run commands from the **repository root** (`c:\lsrc\schoolapp`).

### **2A. IdentityContext — identity schema changes**

```powershell
# dotnet CLI
dotnet ef migrations add <Name> --context IdentityContext --output-dir Migrations/Identity `
  --project SCH\SCH.Repositories\SCH.Repositories.csproj `
  --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj

dotnet ef database update --context IdentityContext `
  --project SCH\SCH.Repositories\SCH.Repositories.csproj `
  --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj
```

```powershell
# Package Manager Console (Visual Studio — set Default project to SCH.Repositories)
Add-Migration <Name> -Context IdentityContext -OutputDir Migrations/Identity -StartupProject SCH.Repositories
Update-Database -Context IdentityContext -StartupProject SCH.Repositories
```

**identity schema tables:**
- `identity.AspNetUsers`
- `identity.AspNetRoles`
- `identity.AspNetUserRoles`
- `identity.AspNetUserClaims`
- `identity.AspNetUserLogins`
- `identity.AspNetUserTokens`
- `identity.AspNetRoleClaims`
- `identity.RefreshTokens`
- `identity.__EFMigrationsHistory`

---

### **2B. SCHContext — dbo schema changes**

```powershell
# dotnet CLI
dotnet ef migrations add <Name> --context SCHContext --output-dir Migrations `
  --project SCH\SCH.Repositories\SCH.Repositories.csproj `
  --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj

dotnet ef database update --context SCHContext `
  --project SCH\SCH.Repositories\SCH.Repositories.csproj `
  --startup-project SCH\SCH.Repositories\SCH.Repositories.csproj
```

```powershell
# Package Manager Console
Add-Migration <Name> -Context SCHContext -OutputDir Migrations -StartupProject SCH.Repositories
Update-Database -Context SCHContext -StartupProject SCH.Repositories
```

**dbo schema tables:**
- `dbo.User` (Id is set to match `identity.AspNetUsers.Id` — no FK enforced at DB level)
- `dbo.Student`
- `dbo.Course`
- `dbo.Teacher`
- `dbo.StudentCourseMap`
- `dbo.__EFMigrationsHistory`

---

## 📂 Current Migration Files

```
SCH/SCH.Repositories/Migrations/
├── Identity/
│   ├── 20260204123805_MigrationInitial.cs
│   ├── 20260204123805_MigrationInitial.Designer.cs
│   └── IdentityContextModelSnapshot.cs
│
├── 20260204123732_MigrationInitial.cs
├── 20260204123732_MigrationInitial.Designer.cs
└── SCHContextModelSnapshot.cs
```

---

## 🗄️ Database Schema Result

Your database will have **TWO schemas**:

```sql
-- Identity Schema (Auth)
identity.__EFMigrationsHistory
identity.AspNetUsers
identity.AspNetRoles
identity.AspNetUserRoles
identity.AspNetUserClaims
identity.AspNetUserLogins
identity.AspNetUserTokens
identity.AspNetRoleClaims
identity.RefreshTokens

-- DBO Schema (Domain)
dbo.__EFMigrationsHistory
dbo.User                    -- NEW! Links to identity.AspNetUsers
dbo.Student
dbo.Course
dbo.Teacher
dbo.StudentCourseMap
```

---

## 📖 Understanding Migration Files

### **What is a Snapshot file?**
`*ContextModelSnapshot.cs` represents the **current state** of your database model.

- **Auto-generated** by EF Core
- Used to calculate differences for new migrations
- **Don't edit manually**
- One snapshot per context

### **What are Migration files?**
`YYYYMMDDhhmmss_MigrationName.cs` files contain:
- `Up()` method - applies changes (creates tables, columns, etc.)
- `Down()` method - reverts changes (drops tables, columns, etc.)

### **What are Designer files?**
`*_MigrationName.Designer.cs` files contain metadata:
- Model snapshot at the time of migration
- Used by EF Core internally
- **Don't edit manually**

---

## 🔄 Future Migrations

When you change models, create new migrations. See **Step 2** above for the full commands.

---

## ⚠️ Important Notes

### **1. Migration History Tables**
Each context has its own migration history:
- `identity.__EFMigrationsHistory` (for IdentityContext)
- `dbo.__EFMigrationsHistory` (for SCHContext)

### **2. Same Database, Different Schemas**
Both contexts use the **same connection string** but manage different schemas.

### **3. User Table Linking**
`dbo.User.Id` is the primary key and is set to the same value as `identity.AspNetUsers.Id` at registration time. There is **no `AspNetUserId` column** and **no DB-level foreign key** enforcing this link across schemas — it is maintained manually in application code.

### **4. Seeding Roles**
After running migrations, seed default roles:
```csharp
// In your startup or seed class
await roleManager.CreateAsync(new ApplicationRole 
{ 
    Name = "Admin", 
    Description = "Administrator role" 
});

await roleManager.CreateAsync(new ApplicationRole 
{ 
    Name = "Basic", 
    Description = "Basic user role" 
});
```

---

## 🐛 Troubleshooting

### **Error: "A context with type 'X' was not found"**
- Ensure you're in `SCH.Repositories` directory
- Check that `*ContextFactory.cs` files exist

### **Error: "Unable to create an object of type 'X'"**
- Verify `appsettings.json` exists in `SCH.Api`
- Check `DefaultConnection` connection string is valid

### **Error: "The entity type 'X' requires a primary key"**
- Check your entity configurations in `OnModelCreating`
- Ensure all entities have `HasKey()` configured

### **Database already has tables?**
If your database already has the old tables:
1. **Backup your data!**
2. Drop all existing tables
3. Run fresh migrations
4. Restore data if needed

---

## ✅ Verification

After migrations, verify your database:

```sql
-- Check schemas exist
SELECT * FROM sys.schemas WHERE name IN ('identity', 'dbo');

-- Check Identity tables
SELECT * FROM identity.__EFMigrationsHistory;
SELECT * FROM identity.AspNetUsers;
SELECT * FROM identity.RefreshTokens;

-- Check Domain tables
SELECT * FROM dbo.__EFMigrationsHistory;
SELECT * FROM dbo.[User];
SELECT * FROM dbo.Student;
SELECT * FROM dbo.Course;
```

---

## 🎯 Initial Setup Checklist

- [x] Delete old migration files from `/Migrations/`
- [x] Create `IdentityContextFactory.cs`
- [x] Create `SCHContextFactory.cs` with schema config
- [x] Run initial migration for `IdentityContext` (`MigrationInitial`)
- [x] Apply `IdentityContext` migration to database
- [x] Run initial migration for `SCHContext` (`MigrationInitial`)
- [x] Apply `SCHContext` migration to database
- [ ] Verify both schemas in database
- [ ] Seed default roles (Admin, Basic)
- [ ] Test registration and login

---

**Your database is now ready with clean schema separation!** 🚀

