using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Data;

internal sealed class EduMonitorRepository(EduMonitorDbContext dbContext) : IEduMonitorRepository
{
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

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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
            .Where(item => (item.Module.Instance ?? item.Module.Id) > 0)
            .ToList();
        var moodleActivityIds = modules.Select(item => item.Module.Instance ?? item.Module.Id)
            .Distinct()
            .ToArray();

        var activities = await dbContext.Actividades
            .Where(activity => moodleActivityIds.Contains(activity.MoodleId))
            .ToDictionaryAsync(activity => activity.MoodleId, cancellationToken);

        foreach (var item in modules)
        {
            var moodleActivityId = item.Module.Instance ?? item.Module.Id;
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
            var activityId = activities[item.Module.Instance ?? item.Module.Id].Id;
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
            .Select(item => item.ItemInstance.GetValueOrDefault() > 0
                ? item.ItemInstance!.Value
                : item.Id)
            .Distinct()
            .ToArray();
        var rows = await dbContext.Actividades
            .Where(activity => moodleActivityIds.Contains(activity.MoodleId))
            .ToDictionaryAsync(activity => activity.MoodleId, cancellationToken);

        foreach (var item in validItems)
        {
            var moodleActivityId = item.ItemInstance.GetValueOrDefault() > 0
                ? item.ItemInstance!.Value
                : item.Id;
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
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return validItems.ToDictionary(
            item => item.Id,
            item => rows[item.ItemInstance.GetValueOrDefault() > 0
                ? item.ItemInstance!.Value
                : item.Id].Id);
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
