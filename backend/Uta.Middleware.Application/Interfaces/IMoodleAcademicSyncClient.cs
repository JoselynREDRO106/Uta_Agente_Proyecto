using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IMoodleAcademicSyncClient
{
    Task<CursoDto> SynchronizeCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default);

    Task<AcademicCourseSnapshot> SynchronizeCourseAndGetSnapshotAsync(
        int courseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SincronizacionDto>> GetSyncHistoryAsync(
        int limit,
        CancellationToken cancellationToken = default);
}
