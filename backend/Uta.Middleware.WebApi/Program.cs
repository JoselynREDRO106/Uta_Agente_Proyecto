using Uta.Middleware.Infrastructure;

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

app.Run();
