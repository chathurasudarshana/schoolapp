using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using SCH.API.Authorization;
using SCH.Core.Cors;
using SCH.Core.Extensions;
using SCH.Core.ErrorHandling;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string allowOriginsPolicy = "AllowOrigins";

builder.Services.AddAllowedOrigins(
    allowOriginsPolicy, builder.Configuration.GetSection("AllowedOrigins").Value);

// Add HttpContext services (IHttpContextAccessor, IUserInfo, etc.)
builder.Services.AddHttpContextServices();

// Add services to the container.

builder.Services.ConfigureControllersWithFilters(); // Includes ModelValidationFilter
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddMappings();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Register both contexts (same database, different schemas)
builder.Services.AddDbContexts(connectionString, connectionString);

builder.Services.AddUnitOfWorks();
builder.Services.AddAuthenticationWithJwt(builder.Configuration);
builder.Services.AddSchoolAppPolicies();

builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");
builder.Services.AddLogger();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SCH API",
        Version = "v1",
        Description = "School Management System API with JWT Authentication"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the text input below.\n\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SCH.Api v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI as the default page
    });
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors(allowOriginsPolicy);

app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
