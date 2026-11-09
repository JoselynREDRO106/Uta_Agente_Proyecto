using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Uta.MoodleIntegration.WebApi.Data;
using Uta.MoodleIntegration.WebApi.Middleware;
using Uta.MoodleIntegration.WebApi.Messaging;
using Uta.MoodleIntegration.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var signingKey = builder.Configuration["Authentication:Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Authentication__Jwt__SigningKey with at least 32 random bytes.");
}

var issuer = builder.Configuration["Authentication:Jwt:Issuer"] ?? "EduMonitor";
var audience = builder.Configuration["Authentication:Jwt:Audience"] ?? "EduMonitor";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var integrationApiKey = builder.Configuration["MoodleIntegration:ApiKey"];
if (string.IsNullOrWhiteSpace(integrationApiKey) || Encoding.UTF8.GetByteCount(integrationApiKey) < 32)
{
    throw new InvalidOperationException(
        "Configure MoodleIntegration__ApiKey with at least 32 random bytes.");
}

var connectionString = builder.Configuration.GetConnectionString("MoodleIntegration");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings__MoodleIntegration for the independent Moodle Integration database.");
}

builder.Services.AddDbContext<MoodleIntegrationDbContext>(options =>
    options.UseSqlServer(connectionString));
var keyDirectory = builder.Configuration["MoodleIntegration:DataProtectionKeysPath"];
if (string.IsNullOrWhiteSpace(keyDirectory))
{
    keyDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "EduMonitor",
        "MoodleIntegration",
        "keys");
}

Directory.CreateDirectory(keyDirectory);
builder.Services.AddDataProtection()
    .SetApplicationName("EduMonitor.MoodleIntegration")
    .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IDataProtectionProvider>()
        .CreateProtector("EduMonitor.MoodleIntegration.MoodleToken.v1"));
builder.Services.AddHttpClient("MoodleConnectionTest");
builder.Services.AddScoped<MoodleConfigurationService>();
builder.Services.AddScoped<MoodleConnectionTester>();
builder.Services.AddSingleton(new IntegrationApiKey(integrationApiKey));
var rabbitMqSection = builder.Configuration.GetSection(RabbitMqSettings.SectionName);
var rabbitMqSettings = rabbitMqSection.Get<RabbitMqSettings>() ?? new RabbitMqSettings();
if (!string.IsNullOrWhiteSpace(rabbitMqSettings.HostName))
{
    if (string.IsNullOrWhiteSpace(rabbitMqSettings.UserName)
        || string.IsNullOrWhiteSpace(rabbitMqSettings.Password)
        || rabbitMqSettings.Port is < 1 or > 65535)
    {
        throw new InvalidOperationException(
            "RabbitMq__HostName requires RabbitMq__UserName, RabbitMq__Password, and a valid RabbitMq__Port.");
    }

    builder.Services.Configure<RabbitMqSettings>(rabbitMqSection);
    builder.Services.AddHostedService<AcademicOutboxPublisher>();
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseMiddleware<InternalApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    servicio = "EduMonitor.MoodleIntegration"
})).AllowAnonymous();
app.MapGet("/health/database", async (
    MoodleIntegrationDbContext db,
    CancellationToken cancellationToken) =>
{
    var connected = await db.Database.CanConnectAsync(cancellationToken);
    return connected
        ? Results.Ok(new { status = "ok", database = "MoodleIntegration" })
        : Results.Problem(
            title: "No se pudo conectar con la base del servicio de Integración Moodle.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.Run();

public partial class Program;
