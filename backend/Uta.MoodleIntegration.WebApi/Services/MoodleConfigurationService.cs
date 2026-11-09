using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Infrastructure.Moodle;
using Uta.MoodleIntegration.WebApi.Data;

namespace Uta.MoodleIntegration.WebApi.Services;

public sealed class MoodleConfigurationService(
    MoodleIntegrationDbContext db,
    IDataProtector tokenProtector)
{
    public async Task<MoodleSettings?> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var stored = await db.MoodleConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(row => row.Id == 1, cancellationToken);
        if (stored is null)
        {
            return null;
        }

        return new MoodleSettings
        {
            BaseUrl = stored.BaseUrl,
            Token = tokenProtector.Unprotect(stored.ProtectedToken),
            TimeoutSeconds = stored.TimeoutSeconds
        };
    }

    public async Task<MoodleConfigurationStatus> GetStatusAsync(CancellationToken cancellationToken)
    {
        var stored = await db.MoodleConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(row => row.Id == 1, cancellationToken);
        return stored is null
            ? new MoodleConfigurationStatus(false, string.Empty, null, null)
            : new MoodleConfigurationStatus(
                !string.IsNullOrWhiteSpace(stored.ProtectedToken),
                stored.BaseUrl,
                stored.TimeoutSeconds,
                stored.UpdatedAtUtc);
    }

    public async Task SaveAsync(
        MoodleConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.BaseUrl))
        {
            throw new ArgumentException("La URL base de Moodle es obligatoria.");
        }

        var baseUrl = request.BaseUrl.Trim().TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
            || !string.IsNullOrEmpty(uri.UserInfo)
            || !string.IsNullOrEmpty(uri.Query)
            || !string.IsNullOrEmpty(uri.Fragment)
            || (uri.Scheme == Uri.UriSchemeHttp && !uri.IsLoopback))
        {
            throw new ArgumentException(
                "La URL debe ser HTTP(S), sin credenciales ni parámetros; HTTP solo se permite para Moodle local.");
        }

        if (baseUrl.Length > 500)
        {
            throw new ArgumentException("La URL de Moodle no puede superar 500 caracteres.");
        }

        if (request.TimeoutSeconds is < 5 or > 120)
        {
            throw new ArgumentException("El tiempo de espera debe estar entre 5 y 120 segundos.");
        }

        var existing = await db.MoodleConfigurations
            .SingleOrDefaultAsync(row => row.Id == 1, cancellationToken);
        var token = string.IsNullOrWhiteSpace(request.Token)
            ? existing is null ? null : tokenProtector.Unprotect(existing.ProtectedToken)
            : request.Token.Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Ingresa el token de Moodle para la primera configuración.");
        }

        if (token.Length > 3500)
        {
            throw new ArgumentException("El token de Moodle excede la longitud permitida.");
        }

        if (existing is null)
        {
            existing = new MoodleConfigurationRecord { Id = 1 };
            db.MoodleConfigurations.Add(existing);
        }

        existing.BaseUrl = baseUrl;
        existing.ProtectedToken = tokenProtector.Protect(token);
        existing.TimeoutSeconds = request.TimeoutSeconds;
        existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}

public sealed record MoodleConfigurationRequest(string BaseUrl, string? Token, int TimeoutSeconds = 30);

public sealed record MoodleConfigurationStatus(
    bool IsConfigured,
    string BaseUrl,
    int? TimeoutSeconds,
    DateTimeOffset? UpdatedAtUtc);
