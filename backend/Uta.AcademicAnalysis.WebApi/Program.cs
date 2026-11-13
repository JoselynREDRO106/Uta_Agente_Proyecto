using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Uta.AcademicAnalysis.WebApi.Data;
using Uta.AcademicAnalysis.WebApi.Messaging;

QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var signingKey = builder.Configuration["Authentication:Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Authentication__Jwt__SigningKey with at least 32 random bytes.");
}

var connectionString = builder.Configuration.GetConnectionString("AcademicAnalysis")
    ?? throw new InvalidOperationException("Configure ConnectionStrings__AcademicAnalysis.");
builder.Services.AddDbContext<AnalysisDbContext>(options => options.UseSqlServer(connectionString));

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
    builder.Services.AddHostedService<AcademicProjectionConsumer>();
}

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
app.MapGet("/health", () => Results.Ok(new { status = "ok", servicio = "EduMonitor.AcademicAnalysis" }))
    .AllowAnonymous();
app.MapGet("/health/database", async (
    AnalysisDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var connected = await dbContext.Database.CanConnectAsync(cancellationToken);
    return connected
        ? Results.Ok(new { status = "ok", database = "EduMonitorAnalysis" })
        : Results.Problem(
            title: "No se pudo conectar con SQL Server",
            detail: "Verifique que SQL Server esté iniciado y que la base EduMonitorAnalysis exista.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.Run();
