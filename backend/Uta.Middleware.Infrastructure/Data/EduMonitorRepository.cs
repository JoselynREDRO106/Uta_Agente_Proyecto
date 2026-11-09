using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Data;

internal sealed class EduMonitorRepository(EduMonitorDbContext dbContext) : IEduMonitorRepository
{
    public async Task<IReadOnlyCollection<CursoDto>> GetCoursesAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Cursos
            .AsNoTracking()
            .OrderBy(course => course.Name)
            .Select(course => new CursoDto(course.MoodleId, course.Name))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<CursoDto>> GetCoursesForUserAsync(
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var courses = dbContext.Cursos.AsNoTracking();
        if (userId.HasValue)
        {
            courses = courses.Where(course => dbContext.AsignacionesCurso.Any(
                assignment => assignment.UserId == userId.Value && assignment.CourseId == course.Id));
        }

        return await courses.OrderBy(course => course.Name)
            .Select(course => new CursoDto(course.MoodleId, course.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsCourseAssignedAsync(
        int userId,
        int moodleCourseId,
        CancellationToken cancellationToken = default) =>
        dbContext.AsignacionesCurso.AnyAsync(
            assignment => assignment.UserId == userId
                && dbContext.Cursos.Any(course =>
                    course.Id == assignment.CourseId && course.MoodleId == moodleCourseId),
            cancellationToken);

    public async Task<PaginaDto<EstudianteCursoDto>> GetCourseStudentsAsync(
        int moodleCourseId,
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var course = await dbContext.Cursos.AsNoTracking()
            .SingleOrDefaultAsync(item => item.MoodleId == moodleCourseId, cancellationToken);
        if (course is null)
        {
            return new PaginaDto<EstudianteCursoDto>([], page, pageSize, 0, 0);
        }

        var query = dbContext.Usuarios.AsNoTracking()
            .Where(user => dbContext.Matriculas.Any(
                enrollment => enrollment.CourseId == course.Id && enrollment.UserId == user.Id));
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(user => user.Name.Contains(term) || user.Email.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var requestedStatus = status.Trim().ToLowerInvariant();
            query = requestedStatus == "sin_datos"
                ? query.Where(user => !dbContext.Calificaciones.Any(grade =>
                    grade.UserId == user.Id
                    && dbContext.Actividades.Any(activity =>
                        activity.Id == grade.ActivityId && activity.CourseId == course.Id)))
                : query.Where(user => dbContext.Calificaciones.Any(grade =>
                    grade.UserId == user.Id
                    && dbContext.Actividades.Any(activity =>
                        activity.Id == grade.ActivityId && activity.CourseId == course.Id)
                    && grade.Status == requestedStatus));
        }

        var total = await query.CountAsync(cancellationToken);
        var students = await query.OrderBy(user => user.Name)
            .ThenBy(user => user.MoodleId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new
            {
                user.Id,
                user.MoodleId,
                user.Name,
                user.Email
            })
            .ToListAsync(cancellationToken);

        var userIds = students.Select(student => student.Id).ToArray();
        var grades = await (
            from grade in dbContext.Calificaciones.AsNoTracking()
            join activity in dbContext.Actividades.AsNoTracking()
                on grade.ActivityId equals activity.Id
            where activity.CourseId == course.Id && userIds.Contains(grade.UserId)
            select new { grade.UserId, grade.Grade, grade.Status, activity.MaxGrade })
            .ToListAsync(cancellationToken);

        var gradesByUser = grades.GroupBy(grade => grade.UserId)
            .ToDictionary(group => group.Key, group => group.ToArray());
        var result = students.Select(student =>
        {
            gradesByUser.TryGetValue(student.Id, out var userGrades);
            var percentages = (userGrades ?? [])
                .Where(grade => grade.Status == "calificado"
                    && grade.Grade.HasValue
                    && grade.MaxGrade is > 0)
                .Select(grade => grade.Grade!.Value / grade.MaxGrade!.Value * 100m)
                .ToArray();
            var studentStatus = userGrades is null || userGrades.Length == 0
                ? "sin_datos"
                : userGrades.Any(grade => grade.Status == "sin_entrega")
                    ? "sin_entrega"
                    : userGrades.Any(grade => grade.Status == "pendiente")
                        ? "pendiente"
                        : "calificado";

            return new EstudianteCursoDto(
                student.MoodleId,
                student.Name,
                student.Email,
                percentages.Length == 0 ? null : decimal.Round(percentages.Average(), 2),
                studentStatus);
        }).ToArray();

        return new PaginaDto<EstudianteCursoDto>(
            result, page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
    }

    public async Task<IndicadoresCursoDto?> GetCourseIndicatorsAsync(
        int moodleCourseId,
        CancellationToken cancellationToken = default)
    {
        var course = await dbContext.Cursos.AsNoTracking()
            .SingleOrDefaultAsync(item => item.MoodleId == moodleCourseId, cancellationToken);
        if (course is null)
        {
            return null;
        }

        var studentCount = await dbContext.Matriculas.CountAsync(
            enrollment => enrollment.CourseId == course.Id, cancellationToken);
        var activityCount = await dbContext.Actividades.CountAsync(
            activity => activity.CourseId == course.Id, cancellationToken);
        var gradeRows = await (
            from grade in dbContext.Calificaciones.AsNoTracking()
            join activity in dbContext.Actividades.AsNoTracking()
                on grade.ActivityId equals activity.Id
            where activity.CourseId == course.Id
            select new { grade.UserId, grade.Grade, grade.Status, activity.Id, activity.MaxGrade })
            .ToListAsync(cancellationToken);

        var numericGrades = gradeRows
            .Where(grade => grade.Status == "calificado"
                && grade.Grade.HasValue
                && grade.MaxGrade is > 0)
            .Select(grade => new
            {
                grade.UserId,
                grade.Id,
                Percentage = grade.Grade!.Value / grade.MaxGrade!.Value * 100m,
                grade.MaxGrade
            })
            .ToArray();
        var percentages = numericGrades.Select(grade => grade.Percentage).ToArray();
        var gradedActivityCount = numericGrades.Select(grade => grade.Id).Distinct().Count();
        var denominator = (long)studentCount * activityCount;
        decimal? progress = denominator == 0
            ? null
            : gradeRows.Count == 0
                ? null
                : decimal.Round(Math.Min(100m,
                    gradeRows.Count(grade => grade.Status == "calificado") * 100m / denominator), 2);
        var lastSync = await dbContext.Sincronizaciones.AsNoTracking()
            .Where(sync => sync.CourseId == course.Id)
            .OrderByDescending(sync => sync.CompletedAtUtc)
            .Select(sync => new { sync.CompletedAtUtc, sync.Status })
            .FirstOrDefaultAsync(cancellationToken);

        var distribution = new[]
        {
            new DistribucionCalificacionDto("0–59", percentages.Count(value => value < 60)),
            new DistribucionCalificacionDto("60–69", percentages.Count(value => value is >= 60 and < 70)),
            new DistribucionCalificacionDto("70–84", percentages.Count(value => value is >= 70 and < 85)),
            new DistribucionCalificacionDto("85–100", percentages.Count(value => value is >= 85 and <= 100)),
            new DistribucionCalificacionDto(">100", percentages.Count(value => value > 100))
        };

        return new IndicadoresCursoDto(
            moodleCourseId,
            studentCount,
            percentages.Length == 0 ? null : decimal.Round(percentages.Average(), 2),
            numericGrades.Length == 0 ? null : 100m,
            gradedActivityCount,
            activityCount,
            progress,
            lastSync?.CompletedAtUtc,
            lastSync?.Status,
            percentages.Length > 0,
            distribution);
    }

    public async Task<IReadOnlyCollection<SincronizacionDto>> GetSyncHistoryAsync(
        int limit,
        CancellationToken cancellationToken = default) =>
        await (
            from sync in dbContext.Sincronizaciones.AsNoTracking()
            join course in dbContext.Cursos.AsNoTracking()
                on sync.CourseId equals course.Id
            orderby sync.CompletedAtUtc descending, sync.Id descending
            select new SincronizacionDto(
                course.MoodleId,
                course.Name,
                sync.CompletedAtUtc,
                sync.Status,
                sync.ErrorSummary))
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task RecordSyncAsync(
        int moodleCourseId,
        string status,
        string? error,
        CancellationToken cancellationToken = default)
    {
        var course = await dbContext.Cursos.SingleOrDefaultAsync(
            item => item.MoodleId == moodleCourseId, cancellationToken);
        if (course is null)
        {
            return;
        }

        dbContext.Sincronizaciones.Add(new SincronizacionRow
        {
            CourseId = course.Id,
            CompletedAtUtc = DateTime.UtcNow,
            Status = status,
            ErrorSummary = error is null ? null : Limit(error, 1000)
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CursoDto> SaveCourseAsync(
        MoodleCourseDto course,
        CancellationToken cancellationToken = default)
    {
        var row = await EnsureCourseAsync(course, cancellationToken);
        return new CursoDto(row.MoodleId, row.Name);
    }

    public async Task SaveGradesAsync(
        MoodleCourseDto course,
        IReadOnlyCollection<MoodleEnrolledUserDto> users,
        MoodleGradesResponseDto grades,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var courseRow = await EnsureCourseAsync(course, cancellationToken);
        var usersByMoodleId = await EnsureUsersAsync(users, grades.Grades, cancellationToken);
        var activitiesByGradeItemId = await EnsureGradeActivitiesAsync(
            courseRow.Id, grades.Items, cancellationToken);

        foreach (var grade in grades.Grades)
        {
            if (!usersByMoodleId.TryGetValue(grade.UserId, out var userId)
                || !activitiesByGradeItemId.TryGetValue(grade.ItemId, out var activityId))
            {
                continue;
            }

            var row = await dbContext.Calificaciones.SingleOrDefaultAsync(
                existing => existing.UserId == userId && existing.ActivityId == activityId,
                cancellationToken);
            var parsedGrade = ParseGrade(grade.Grade);
            var status = parsedGrade.HasValue
                ? "calificado"
                : HasSubmission(grade.DateSubmitted) ? "pendiente" : "sin_entrega";

            if (row is null)
            {
                row = new CalificacionRow { UserId = userId, ActivityId = activityId };
                dbContext.Calificaciones.Add(row);
            }

            row.Grade = parsedGrade;
            row.Status = status;
        }

        await EnsureEnrollmentsAsync(courseRow.Id, usersByMoodleId, users, cancellationToken);
        dbContext.Sincronizaciones.Add(new SincronizacionRow
        {
            CourseId = courseRow.Id,
            CompletedAtUtc = DateTime.UtcNow,
            Status = grades.Warnings.Count == 0 ? "Completa" : "Incompleta",
            ErrorSummary = grades.Warnings.Count == 0
                ? null
                : Limit(string.Join("; ", grades.Warnings), 1000)
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task EnsureEnrollmentsAsync(
        int courseId,
        IReadOnlyDictionary<int, int> userIdsByMoodleId,
        IReadOnlyCollection<MoodleEnrolledUserDto> users,
        CancellationToken cancellationToken)
    {
        var userIds = users
            .Where(user => user.Id > 0
                && userIdsByMoodleId.ContainsKey(user.Id)
                && user.Roles.Any(role =>
                    role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                    || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)))
            .Select(user => userIdsByMoodleId[user.Id])
            .Distinct()
            .ToArray();
        var existing = await dbContext.Matriculas
            .Where(enrollment => enrollment.CourseId == courseId && userIds.Contains(enrollment.UserId))
            .Select(enrollment => enrollment.UserId)
            .ToListAsync(cancellationToken);
        var existingIds = existing.ToHashSet();

        foreach (var userId in userIds.Where(id => !existingIds.Contains(id)))
        {
            dbContext.Matriculas.Add(new MatriculaRow { CourseId = courseId, UserId = userId });
        }
    }

    public async Task SaveCourseContentsAsync(
        MoodleCourseDto course,
        MoodleCourseModulesResponseDto contents,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var courseRow = await EnsureCourseAsync(course, cancellationToken);
        var modules = contents.Sections
            .SelectMany(section => section.Modules.Select(module => new
            {
                Module = module,
                Content = string.IsNullOrWhiteSpace(section.Name)
                    ? $"Unidad {section.Section}: {module.Name}"
                    : $"{section.Name}: {module.Name}"
            }))
            .Where(item => item.Module.Id > 0)
            .ToList();
        var moodleActivityIds = modules.Select(item => item.Module.Id)
            .Distinct()
            .ToArray();

        var activities = await dbContext.Actividades
            .Where(activity => moodleActivityIds.Contains(activity.MoodleId))
            .ToDictionaryAsync(activity => activity.MoodleId, cancellationToken);

        foreach (var item in modules)
        {
            var moodleActivityId = item.Module.Id;
            if (!activities.TryGetValue(moodleActivityId, out var activity))
            {
                activity = new ActividadRow
                {
                    MoodleId = moodleActivityId,
                    CourseId = courseRow.Id,
                    Name = Limit(item.Module.Name, 200),
                    Category = GetCategory(item.Module.ModName)
                };
                dbContext.Actividades.Add(activity);
                activities.Add(moodleActivityId, activity);
            }
            else
            {
                activity.Name = Limit(item.Module.Name, 200);
                activity.Category = GetCategory(item.Module.ModName);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var activityIds = activities.Values.Select(activity => activity.Id).ToArray();
        var existingMappings = await dbContext.MapeosTematicos
            .Where(mapping => activityIds.Contains(mapping.ActivityId))
            .Select(mapping => new { mapping.ActivityId, mapping.MinimumContent })
            .ToListAsync(cancellationToken);

        foreach (var item in modules)
        {
            var activityId = activities[item.Module.Id].Id;
            var content = Limit(item.Content, 150);
            if (!existingMappings.Any(mapping =>
                    mapping.ActivityId == activityId && mapping.MinimumContent == content))
            {
                dbContext.MapeosTematicos.Add(new MapeoTematicoRow
                {
                    ActivityId = activityId,
                    MinimumContent = content
                });
                existingMappings.Add(new { ActivityId = activityId, MinimumContent = content });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<CursoRow> EnsureCourseAsync(
        MoodleCourseDto course,
        CancellationToken cancellationToken)
    {
        var row = await dbContext.Cursos.SingleOrDefaultAsync(
            existing => existing.MoodleId == course.Id,
            cancellationToken);
        if (row is null)
        {
            row = new CursoRow { MoodleId = course.Id };
            dbContext.Cursos.Add(row);
        }

        row.Name = Limit(
            string.IsNullOrWhiteSpace(course.FullName) ? course.ShortName : course.FullName,
            200);
        await dbContext.SaveChangesAsync(cancellationToken);
        return row;
    }

    private async Task<Dictionary<int, int>> EnsureUsersAsync(
        IReadOnlyCollection<MoodleEnrolledUserDto> users,
        IReadOnlyCollection<MoodleUserGradeDto> grades,
        CancellationToken cancellationToken)
    {
        var usersByMoodleId = users
            .Where(user => user.Id > 0)
            .GroupBy(user => user.Id)
            .ToDictionary(group => group.Key, group => group.First());
        var moodleUserIds = grades.Select(grade => grade.UserId)
            .Where(id => id > 0)
            .Concat(usersByMoodleId.Keys)
            .Distinct()
            .ToArray();
        var rows = await dbContext.Usuarios
            .Where(user => moodleUserIds.Contains(user.MoodleId))
            .ToDictionaryAsync(user => user.MoodleId, cancellationToken);

        foreach (var moodleUserId in moodleUserIds)
        {
            usersByMoodleId.TryGetValue(moodleUserId, out var user);
            if (!rows.TryGetValue(moodleUserId, out var row))
            {
                row = new UsuarioRow { MoodleId = moodleUserId };
                dbContext.Usuarios.Add(row);
                rows.Add(moodleUserId, row);
            }

            row.Name = Limit(
                string.IsNullOrWhiteSpace(user?.FullName)
                    ? $"Usuario Moodle {moodleUserId}"
                    : user.FullName,
                150);
            row.Email = Limit(user?.Email ?? string.Empty, 150);
            row.Role = Limit(
                user?.Roles?.FirstOrDefault(role => !string.IsNullOrWhiteSpace(role.ShortName))?.ShortName
                    ?? "Estudiante",
                30);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return rows.ToDictionary(pair => pair.Key, pair => pair.Value.Id);
    }

    private async Task<Dictionary<int, int>> EnsureGradeActivitiesAsync(
        int courseId,
        IReadOnlyCollection<MoodleGradeItemDto> items,
        CancellationToken cancellationToken)
    {
        var validItems = items.Where(item => item.Id > 0).ToArray();
        var moodleActivityIds = validItems
            .Select(item => item.CourseModuleId > 0 ? item.CourseModuleId : item.Id)
            .Distinct()
            .ToArray();
        var rows = await dbContext.Actividades
            .Where(activity => moodleActivityIds.Contains(activity.MoodleId))
            .ToDictionaryAsync(activity => activity.MoodleId, cancellationToken);

        foreach (var item in validItems)
        {
            var moodleActivityId = item.CourseModuleId > 0 ? item.CourseModuleId : item.Id;
            if (!rows.TryGetValue(moodleActivityId, out var row))
            {
                row = new ActividadRow
                {
                    MoodleId = moodleActivityId,
                    CourseId = courseId
                };
                dbContext.Actividades.Add(row);
                rows.Add(moodleActivityId, row);
            }

            row.Name = Limit(
                string.IsNullOrWhiteSpace(item.ItemName) ? $"Actividad {item.Id}" : item.ItemName,
                200);
            row.Category = GetCategory(item.ItemModule);
            row.MaxGrade = item.GradeMax > 0 ? item.GradeMax : null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return validItems.ToDictionary(
            item => item.Id,
            item => rows[item.CourseModuleId > 0 ? item.CourseModuleId : item.Id].Id);
    }

    private static decimal? ParseGrade(string grade) =>
        decimal.TryParse(
            grade,
            System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture,
            out var parsedGrade)
            ? decimal.Round(parsedGrade, 2)
            : null;

    private static bool HasSubmission(string dateSubmitted) =>
        long.TryParse(dateSubmitted, out var timestamp) && timestamp > 0;

    private static string GetCategory(string module) =>
        module.ToLowerInvariant() switch
        {
            "quiz" => "Evaluaciones",
            "assign" => "Tareas",
            _ => "AP"
        };

    private static string Limit(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
