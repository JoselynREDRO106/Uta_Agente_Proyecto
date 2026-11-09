using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Application.UseCases;

public sealed class GestionarCursosUseCase(
    IMoodleHttpClient moodleHttpClient,
    IEduMonitorRepository repository) : IGestionarCursosUseCase
{
    public Task<IReadOnlyCollection<CursoDto>> ObtenerAsync(
        CancellationToken cancellationToken = default) =>
        repository.GetCoursesAsync(cancellationToken);

    public async Task<CursoDto> SincronizarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var course = await moodleHttpClient.GetCourseAsync(courseId, cancellationToken);
        return await repository.SaveCourseAsync(course, cancellationToken);
    }
}
