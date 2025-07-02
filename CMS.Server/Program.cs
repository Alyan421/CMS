using CMS.Server.EntityFrameworkCore;
using CMS.Server.Services;
using CMS.Server.Managers;
using CMS.Server.Repository;
using CMS.Server.Models;
using Microsoft.EntityFrameworkCore;
using CMS.Server.Managers.Images;
using CMS.Server.Managers.Cloths;
using CMS.Server.Managers.Colors;
using CMS.Server.Managers.Users;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    var ex = args.ExceptionObject as Exception;
    File.WriteAllText("unhandled_exception.txt", ex?.ToString() ?? "Unknown error");
    Console.WriteLine($"CRITICAL UNHANDLED EXCEPTION: {ex}");
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); // Prevent reference loop issues

// Configure image storage service based on configuration
var storageProvider = builder.Configuration["Storage:Provider"] ?? "Cloudinary";
if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    // Register local file storage
    builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();

    // Enable static files for serving local images
    builder.Services.AddDirectoryBrowser();
}
else
{
    // Register Cloudinary storage (default)
    builder.Services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
}

// Configure DbContext with SQL Server
builder.Services.AddDbContext<AMSDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository and Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // Generic repository for any model

// Register ImageManager
builder.Services.AddScoped<IClothManager, ClothManager>();
builder.Services.AddScoped<IColorManager, ColorManager>();
builder.Services.AddScoped<IImageManager, ImageManager>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddHttpContextAccessor();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                throw new InvalidOperationException("JWT key is not configured.")))
    };
});

//Rate limiting
// Add rate-limiting services
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configure Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    c.OperationFilter<AuthResponsesOperationFilter>();
});


//Register Automappers
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Automatically scans for mapping profiles

// Handle environment-specific configurations
if (builder.Environment.IsProduction())
{
    // Override Cloudinary configuration with environment variables
    builder.Configuration["Cloudinary:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
    builder.Configuration["Cloudinary:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
    builder.Configuration["Cloudinary:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

    // Get allowed origins from environment variable
    var allowedOrigins =new[] { "https://green-tree-0e8213e00.2.azurestaticapps.net" };//Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',')
      //  ?? new[] { "http://localhost:4200" };

    // Log the allowed origins for debugging
    Console.WriteLine("ALLOWED_ORIGINS environment variable value: " + allowedOrigins);
    Console.WriteLine("Origins allowed by CORS:");
    foreach (var origin in allowedOrigins)
    {
        Console.WriteLine($"- {origin}");
    }

    // Add CORS services with environment-based configuration
    // Replace your current CORS configuration in the Production block with this
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.AllowAnyOrigin() // This allows requests from any origin
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            // Note: AllowAnyOrigin() is incompatible with AllowCredentials()
        });
    });
}
else
{
    // Development CORS configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

var app = builder.Build();

// Add this right after var app = builder.Build();
app.Use(async (context, next) =>
{
    try
    {
        await next();

        // Capture 500 errors that didn't throw exceptions
        if (context.Response.StatusCode == 500)
        {
            Console.WriteLine("500 error detected in middleware");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled exception: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");

        // Prevent ASP.NET Core from handling the exception
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        // Add CORS headers to error responses
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            error = "An error occurred while processing your request.",
            message = ex.Message,
            stackTrace = app.Environment.IsDevelopment() ? ex.StackTrace : null
        }));
    }
});

// Enable static files for local image storage if using local provider
if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    app.UseStaticFiles(); // Enable serving static files

    // Optional: Enable directory browsing for debugging purposes
    if (app.Environment.IsDevelopment())
    {
        app.UseDirectoryBrowser();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();

app.MapControllers();
//app.MapFallbackToFile("/index.html");

// Add before app.Run()
app.MapGet("/api/diagnostics/db", async (AMSDbContext dbContext) =>
{
    try
    {
        // Test database connectivity
        var canConnect = await dbContext.Database.CanConnectAsync();
        var databaseName = dbContext.Database.GetDbConnection().Database;

        // Try to run a simple query
        int userCount = 0;
        try
        {
            userCount = await dbContext.Users.CountAsync();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                databaseConnected = canConnect,
                databaseName,
                error = $"Query failed: {ex.Message}"
            });
        }

        return Results.Ok(new
        {
            databaseConnected = canConnect,
            databaseName,
            userCount
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message, stack = ex.StackTrace });
    }
});

app.MapGet("/api/diagnostics/jwt", () =>
{
    try
    {
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];
        var jwtKeyLength = builder.Configuration["Jwt:Key"]?.Length ?? 0;

        return Results.Ok(new
        {
            issuer = jwtIssuer ?? "Not configured",
            audience = jwtAudience ?? "Not configured",
            keyLength = jwtKeyLength,
            hasKey = jwtKeyLength > 0
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/diagnostics/connection", () =>
{
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        return Results.Ok(new
        {
            hasConnectionString = !string.IsNullOrEmpty(connectionString),
            connectionStringStart = connectionString?.Length > 20
                ? connectionString.Substring(0, 20) + "..."
                : "Not configured or too short"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();