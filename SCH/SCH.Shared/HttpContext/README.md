# HttpContext Services

This folder contains services that access information from the HTTP request context.

## Architecture Pattern

All HttpContext services follow this pattern:

1. **Marker Interface**: Inherit from `IHttpContextService` for automatic registration
2. **Service Interface**: Define your service contract (e.g., `IUserInfo`, `ISchoolInfo`)
3. **Implementation**: Internal class with both instance methods (DI) and static methods (direct access)
4. **Auto-Registration**: Services are automatically discovered and registered via reflection

## Current Services

### IUserInfo (UserInfo)
Provides access to current authenticated user information from JWT claims.

**Methods:**
- `GetCurrentUserId()` - Returns the current user's ID
- `GetCurrentUserName()` - Returns the current user's username
- `GetCurrentUserEmail()` - Returns the current user's email
- `IsAuthenticated()` - Checks if a user is authenticated

## Adding a New HttpContext Service

### Example: Creating SchoolInfo Service

1. **Create the interface** (`ISchoolInfo.cs`):

```csharp
namespace SCH.Shared.HttpContext
{
    public interface ISchoolInfo : IHttpContextService
    {
        int? GetCurrentSchoolId();
        string? GetCurrentSchoolName();
    }
}
```

2. **Create the implementation** (`SchoolInfo.cs`):

```csharp
namespace SCH.Shared.HttpContext
{
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    internal class SchoolInfo : ISchoolInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SchoolInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region Instance Methods (for Dependency Injection)

        int? ISchoolInfo.GetCurrentSchoolId()
        {
            return GetCurrentSchoolId(_httpContextAccessor.HttpContext);
        }

        string? ISchoolInfo.GetCurrentSchoolName()
        {
            return GetCurrentSchoolName(_httpContextAccessor.HttpContext);
        }

        #endregion

        #region Static Methods (for direct access)

        public static int? GetCurrentSchoolId(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            var schoolIdClaim = httpContext.User.FindFirst("SchoolId")?.Value;
            
            if (string.IsNullOrEmpty(schoolIdClaim))
                return null;

            if (int.TryParse(schoolIdClaim, out int schoolId))
                return schoolId;

            return null;
        }

        public static string? GetCurrentSchoolName(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            return httpContext.User.FindFirst("SchoolName")?.Value;
        }

        #endregion
    }
}
```

3. **That's it!** The service is automatically registered by `HttpContextExtensions.AddHttpContextServices()`

## Usage

### Via Dependency Injection (Recommended)

```csharp
public class MyService
{
    private readonly IUserInfo _userInfo;
    private readonly ISchoolInfo _schoolInfo;
    
    public MyService(IUserInfo userInfo, ISchoolInfo schoolInfo)
    {
        _userInfo = userInfo;
        _schoolInfo = schoolInfo;
    }
    
    public void DoSomething()
    {
        var userId = _userInfo.GetCurrentUserId();
        var schoolId = _schoolInfo.GetCurrentSchoolId();
    }
}
```

### Via Static Methods (When HttpContext is available)

```csharp
public class MyController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var userId = UserInfo.GetCurrentUserId(HttpContext);
        var schoolId = SchoolInfo.GetCurrentSchoolId(HttpContext);
        // ...
    }
}
```

## Design Principles

1. **Marker Interface Pattern**: Use `IHttpContextService` for automatic discovery
2. **Internal Implementation**: Keep implementation classes internal, expose via interfaces
3. **Dual Access Pattern**: Support both DI (instance methods) and static methods
4. **Null Safety**: All methods return nullable types and handle null HttpContext gracefully
5. **Single Responsibility**: Each service focuses on one aspect of HttpContext data
