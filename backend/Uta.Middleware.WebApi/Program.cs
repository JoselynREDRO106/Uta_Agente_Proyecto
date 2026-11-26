using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Uta.Middleware.Infrastructure;
using Uta.Middleware.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
ApplySharedAgentKey(builder);

static void ApplySharedAgentKey(WebApplicationBuilder builder)
{
    if (!builder.Environment.IsDevelopment())
    {
        return;
    }

    var envFile = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", ".env"));
    if (!File.Exists(envFile))
    {
        return;
    }

    string? agentKey = null;
    foreach (var rawLine in File.ReadLines(envFile))
    {
        var line = rawLine.Trim();
        const string prefix = "ACADEMIC_AGENT_API_KEY=";
        if (!line.StartsWith(prefix, StringComparison.Ordinal))
        {
            continue;
        }

        agentKey = line[prefix.Length..].Trim().Trim('"');
    }

    if (string.IsNullOrWhiteSpace(agentKey))
    {
        return;
    }

    var values = new Dictionary<string, string?>
    {
        ["AcademicAgent:ApiKey"] = agentKey,
    };
    if (string.IsNullOrWhiteSpace(builder.Configuration["AcademicAgent:BaseUrl"]))
    {
        values["AcademicAgent:BaseUrl"] = "http://localhost:8010";
    }

    builder.Configuration.AddInMemoryCollection(values);
}

// ─── Servicios ────────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddControllers();
var signingKey = builder.Configuration["Authentication:Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Authentication__Jwt__SigningKey con al menos 32 bytes aleatorios antes de iniciar la API.");
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Registrar capa de Infraestructura (cliente HTTP Moodle + casos de uso)
builder.Services.AddInfrastructure(builder.Configuration);

// ─── Aplicación ───────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    servicio = "EduMonitor.Middleware"
})).AllowAnonymous();
app.MapGet("/health/database", async (
    EduMonitorDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var connected = await dbContext.Database.CanConnectAsync(cancellationToken);
    return connected
        ? Results.Ok(new { status = "ok", database = "EduMonitor" })
        : Results.Problem(
            title: "No se pudo conectar con SQL Server",
            detail: "Verifique que SQL Server Express esté iniciado y que la base EduMonitor exista.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.Run();

public partial class Program;
