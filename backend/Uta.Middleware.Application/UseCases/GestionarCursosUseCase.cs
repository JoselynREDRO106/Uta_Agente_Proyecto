using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Application.UseCases;

public sealed class GestionarCursosUseCase(
    IMoodleHttpClient moodleHttpClient,
    IEduMonitorRepository repository) : IGestionarCursosUseCase
{
    public Task<IReadOnlyCollection<CursoDto>> ObtenerAsync(
        int? userId,
        CancellationToken cancellationToken = default) =>
        repository.GetCoursesForUserAsync(userId, cancellationToken);

    public async Task<CursoDto> SincronizarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var course = await moodleHttpClient.GetCourseAsync(courseId, cancellationToken);
        var savedCourse = await repository.SaveCourseAsync(course, cancellationToken);
        try
        {
            var usersTask = moodleHttpClient.GetEnrolledUsersAsync(courseId, cancellationToken);
            var gradesTask = moodleHttpClient.GetGradesByCourseAsync(courseId, cancellationToken);
            await Task.WhenAll(usersTask, gradesTask);
            await repository.SaveGradesAsync(
                course, await usersTask, await gradesTask, cancellationToken);
            return savedCourse;
        }
        catch (HttpRequestException)
        {
            await repository.RecordSyncAsync(
                courseId, "Incompleta", "No se pudieron consultar todos los datos de Moodle.",
                cancellationToken);
            throw;
        }
        catch (System.Text.Json.JsonException)
        {
            await repository.RecordSyncAsync(
                courseId, "Incompleta", "Moodle devolvió datos con formato inválido.",
                cancellationToken);
            throw;
        }
    }
}
