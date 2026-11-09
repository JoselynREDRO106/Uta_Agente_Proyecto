using Uta.Middleware.Application.DTOs.Moodle;
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
    private readonly IMoodleAcademicSyncClient? _integrationSyncClient;

    public SincronizarCalificacionesUseCase(
        IMoodleHttpClient moodleHttpClient,
        IEduMonitorRepository repository,
        IMoodleAcademicSyncClient? integrationSyncClient = null)
    {
        _moodleHttpClient = moodleHttpClient;
        _repository = repository;
        _integrationSyncClient = integrationSyncClient;
    }

    public async Task<IEnumerable<Calificacion>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        if (_integrationSyncClient is not null)
        {
            var snapshot = await _integrationSyncClient.SynchronizeCourseAndGetSnapshotAsync(
                courseId, cancellationToken);
            return MapGrades(courseId, snapshot.Grades);
        }

        var courseTask = _moodleHttpClient.GetCourseAsync(courseId, cancellationToken);
        var usersTask = _moodleHttpClient.GetEnrolledUsersAsync(courseId, cancellationToken);
        await Task.WhenAll(courseTask, usersTask);

        var course = await courseTask;
        var users = await usersTask;
        var response = await _moodleHttpClient.GetGradesByCourseAsync(
            courseId, users
                .Where(user => user.Roles.Any(role =>
                    role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                    || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)))
                .Select(user => user.Id),
            cancellationToken);

        await _repository.SaveGradesAsync(course, users, response, cancellationToken);
        return MapGrades(courseId, response);
    }

    private static List<Calificacion> MapGrades(
        int courseId,
        MoodleGradesResponseDto response)
    {
        var itemsMap = response.Items.ToDictionary(i => i.Id);

        return response.Grades
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
    }
}
