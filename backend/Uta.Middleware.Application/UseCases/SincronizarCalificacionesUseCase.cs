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
    private readonly IEduMonitorRepository _repository;

    public SincronizarCalificacionesUseCase(
        IMoodleHttpClient moodleHttpClient,
        IEduMonitorRepository repository)
    {
        _moodleHttpClient = moodleHttpClient;
        _repository = repository;
    }

    public async Task<IEnumerable<Calificacion>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var courseTask = _moodleHttpClient.GetCourseAsync(courseId, cancellationToken);
        var usersTask = _moodleHttpClient.GetEnrolledUsersAsync(courseId, cancellationToken);
        var gradesTask = _moodleHttpClient.GetGradesByCourseAsync(courseId, cancellationToken);
        await Task.WhenAll(courseTask, usersTask, gradesTask);

        var course = await courseTask;
        var users = await usersTask;
        var response = await gradesTask;

        // Crear un mapa de ítems para enriquecer las calificaciones
        var itemsMap = response.Items.ToDictionary(i => i.Id);

        var calificaciones = response.Grades
            .Select(g =>
            {
                itemsMap.TryGetValue(g.ItemId, out var item);
                decimal? grade = decimal.TryParse(
                    g.Grade,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var parsedGrade)
                    ? parsedGrade
                    : null;

                return new Calificacion
                {
                    MoodleUserId       = g.UserId,
                    MoodleCourseId     = courseId,
                    MoodleGradeItemId  = g.ItemId,
                    NombreActividad    = item?.ItemName ?? $"Item-{g.ItemId}",
                    NotaObtenida       = grade,
                    NotaMaxima         = item?.GradeMax ?? 10m,
                    FechaRegistro      = long.TryParse(g.DateGraded, out var epoch)
                                            ? DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime
                                            : DateTime.UtcNow
                };
            })
            .ToList();

        await _repository.SaveGradesAsync(course, users, response, cancellationToken);

        return calificaciones;
    }
}
