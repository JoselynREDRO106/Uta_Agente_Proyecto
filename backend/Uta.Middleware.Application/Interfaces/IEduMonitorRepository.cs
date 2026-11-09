using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.Middleware.Application.Interfaces;

public interface IEduMonitorRepository
{
    Task<IReadOnlyCollection<CursoDto>> GetCoursesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CursoDto>> GetCoursesForUserAsync(
        int? userId,
        CancellationToken cancellationToken = default);

    Task<bool> IsCourseAssignedAsync(
        int userId,
        int moodleCourseId,
        CancellationToken cancellationToken = default);

    Task<PaginaDto<EstudianteCursoDto>> GetCourseStudentsAsync(
        int moodleCourseId,
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IndicadoresCursoDto?> GetCourseIndicatorsAsync(
        int moodleCourseId,
        CancellationToken cancellationToken = default);

    Task RecordSyncAsync(
        int moodleCourseId,
        string status,
        string? error,
        CancellationToken cancellationToken = default);

    Task<CursoDto> SaveCourseAsync(
        MoodleCourseDto course,
        CancellationToken cancellationToken = default);

    Task SaveGradesAsync(
        MoodleCourseDto course,
        IReadOnlyCollection<MoodleEnrolledUserDto> users,
        MoodleGradesResponseDto grades,
        CancellationToken cancellationToken = default);

    Task SaveCourseContentsAsync(
        MoodleCourseDto course,
        MoodleCourseModulesResponseDto contents,
        CancellationToken cancellationToken = default);
}
