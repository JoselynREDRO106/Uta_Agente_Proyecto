using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IEduMonitorRepository
{
    Task<IReadOnlyCollection<CursoDto>> GetCoursesAsync(
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
