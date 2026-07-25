using System.Text;
using System.Text.Json.Serialization;

using InstallmentCRM.API.Extensions;
using InstallmentCRM.Application;
using InstallmentCRM.Application.Common;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Infrastructure.Authentication;
using InstallmentCRM.Infrastructure.Identity;
using InstallmentCRM.Persistence.Context;
using InstallmentCRM.Persistence.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// =======================
// Railway: слушаем порт, который выдаёт платформа
// =======================
var railwayPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(railwayPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{railwayPort}");
}


// =======================
// Controllers + Enum JSON
// =======================

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });


// =======================
// PostgreSQL
// =======================

var connectionString = BuildConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());


// =======================
// JWT Settings
// =======================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));


// =======================
// Identity
// =======================

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// =======================
// Services
// =======================

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IIdentityService, IdentityService>();


// =======================
// JWT Authentication
// =======================

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
    });


// =======================
// CORS
// =======================
// Список разрешенных источников задается в appsettings.json (Cors:AllowedOrigins),
// чтобы фронтенд (например, дашборд на другом порту/хосте) мог обращаться к API.

const string CorsPolicyName = "Frontend";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            // Нет явно заданных источников (например, локальная разработка) -
            // разрешаем любой origin, но без credentials.
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});


// =======================
// Application Layer
// =======================

builder.Services.AddApplication();


// =======================
// Swagger
// =======================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,

            Scheme = "Bearer",

            BearerFormat = "JWT",

            In = Microsoft.OpenApi.Models.ParameterLocation.Header,

            Description =
                "Введите JWT токен в формате: Bearer {token}"
        });


    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


// =======================
// Build App
// =======================

var app = builder.Build();


// =======================
// Middleware
// =======================

app.UseGlobalExceptionHandler();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Раздача статического дашборда (wwwroot/index.html) - открывается на "/"
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(CorsPolicyName);

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();


// =======================
// Миграции + Seed Roles
// =======================

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    await IdentitySeeder.SeedRolesAsync(
        scope.ServiceProvider);
}


app.Run();


// =======================
// Helpers
// =======================

static string BuildConnectionString(IConfiguration configuration)
{
    // Railway предоставляет переменную DATABASE_URL в формате:
    // postgresql://user:password@host:port/database
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrWhiteSpace(databaseUrl))
    {
        // Локальная разработка — берём строку из appsettings.json
        return configuration.GetConnectionString("DefaultConnection")!;
    }

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    return new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = userInfo[0],
        Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
        SslMode = Npgsql.SslMode.Require,
        TrustServerCertificate = true
    }.ConnectionString;
}
