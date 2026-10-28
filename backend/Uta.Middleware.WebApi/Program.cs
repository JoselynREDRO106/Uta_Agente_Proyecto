using Uta.Middleware.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ────────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Registrar capa de Infraestructura (cliente HTTP Moodle + casos de uso)
builder.Services.AddInfrastructure(builder.Configuration);

// ─── Aplicación ───────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
