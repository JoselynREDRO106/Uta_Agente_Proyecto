using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Domain.Entities;

namespace Uta.Middleware.Application.UseCases;

/// <summary>
/// Implementación del caso de uso para sincronizar calificaciones desde Moodle.
/// Orquesta el cliente HTTP y mapea los DTOs a entidades del dominio.
/// </summary>
public class SincronizarCalificacionesUseCase : ISincronizarCalificacionesUseCase
{
    private readonly IMoodleHttpClient _moodleHttpClient;

    public SincronizarCalificacionesUseCase(IMoodleHttpClient moodleHttpClient)
    {
        _moodleHttpClient = moodleHttpClient;
    }

    public async Task<IEnumerable<Calificacion>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var response = await _moodleHttpClient.GetGradesByCourseAsync(courseId, cancellationToken);

        // Crear un mapa de ítems para enriquecer las calificaciones
        var itemsMap = response.Items.ToDictionary(i => i.Id);

        var calificaciones = response.Grades
            .Where(g => !string.IsNullOrWhiteSpace(g.Grade) && g.Grade != "-")
            .Select(g =>
            {
                itemsMap.TryGetValue(g.ItemId, out var item);

                return new Calificacion
                {
                    MoodleUserId       = g.UserId,
                    MoodleCourseId     = courseId,
                    MoodleGradeItemId  = g.ItemId,
                    NombreActividad    = item?.ItemName ?? $"Item-{g.ItemId}",
                    NotaObtenida       = decimal.TryParse(g.Grade, out var nota) ? nota : null,
                    NotaMaxima         = item?.GradeMax ?? 10m,
                    FechaRegistro      = long.TryParse(g.DateGraded, out var epoch)
                                            ? DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime
                                            : DateTime.UtcNow
                };
            })
            .ToList();

        return calificaciones;
    }
}
