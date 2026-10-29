using Uta.Middleware.Infrastructure;
using Uta.Middleware.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ────────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Registrar capa de Infraestructura (cliente HTTP Moodle + casos de uso)
builder.Services.AddInfrastructure(builder.Configuration);

// ─── Aplicación ───────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    servicio = "EduMonitor.Middleware"
}));
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
});

app.Run();
