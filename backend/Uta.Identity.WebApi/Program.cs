using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Uta.Identity.WebApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var signingKey = builder.Configuration["Authentication:Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Authentication__Jwt__SigningKey con al menos 32 bytes aleatorios antes de iniciar la API.");
}

var identityConnection = builder.Configuration.GetConnectionString("Identity")
    ?? throw new InvalidOperationException("No está configurada la cadena de conexión 'Identity'.");
builder.Services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(identityConnection));
builder.Services.AddScoped<IdentityRepository>();

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
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

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
app.MapGet("/health", () => Results.Ok(new { status = "ok", servicio = "EduMonitor.Identity" }))
    .AllowAnonymous();
app.MapGet("/health/database", async (
    IdentityDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var connected = await dbContext.Database.CanConnectAsync(cancellationToken);
    return connected
        ? Results.Ok(new { status = "ok", database = "EduMonitorIdentity" })
        : Results.Problem(
            title: "No se pudo conectar con SQL Server",
            detail: "Verifique que SQL Server esté iniciado y que la base EduMonitorIdentity exista.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.Run();
