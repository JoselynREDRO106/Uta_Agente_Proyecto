using System.Security.Cryptography;
using System.Text;

namespace Uta.MoodleIntegration.WebApi.Middleware;

public sealed class InternalApiKeyMiddleware(
    RequestDelegate next,
    IntegrationApiKey apiKey)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/internal/moodle"))
        {
            var suppliedKey = context.Request.Headers["X-Integration-Key"].ToString();
            var expectedBytes = Encoding.UTF8.GetBytes(apiKey.Value);
            var suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);
            if (suppliedBytes.Length != expectedBytes.Length
                || !CryptographicOperations.FixedTimeEquals(suppliedBytes, expectedBytes))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Credencial de servicio inválida.");
                return;
            }
        }

        await next(context);
    }
}
