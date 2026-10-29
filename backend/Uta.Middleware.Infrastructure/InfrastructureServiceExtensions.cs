using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Application.UseCases;
using Uta.Middleware.Infrastructure.Moodle;

namespace Uta.Middleware.Infrastructure;

/// <summary>
/// Extensiones de IServiceCollection para registrar todos los servicios
/// de la capa de Infraestructura (cliente HTTP Moodle, opciones, etc.).
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Enlazar configuración de Moodle usando la sección del appsettings.json
        var moodleSection = configuration.GetSection(MoodleSettings.SectionName);
        services.Configure<MoodleSettings>(opts => moodleSection.Bind(opts));

        // Registrar el cliente HTTP tipado para Moodle
        services.AddHttpClient<IMoodleHttpClient, MoodleHttpClient>((sp, client) =>
        {
            var settings = moodleSection.Get<MoodleSettings>() ?? new MoodleSettings();
            if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
                client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds > 0
                ? settings.TimeoutSeconds : 30);
        });

        // Registrar casos de uso
        services.AddScoped<ISincronizarCalificacionesUseCase, SincronizarCalificacionesUseCase>();
        services.AddScoped<ISincronizarMapeoTematicoUseCase, SincronizarMapeoTematicoUseCase>();

        return services;
    }
}
