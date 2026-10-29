using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.Middleware.Application.Interfaces;

public interface IEduMonitorRepository
{
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
