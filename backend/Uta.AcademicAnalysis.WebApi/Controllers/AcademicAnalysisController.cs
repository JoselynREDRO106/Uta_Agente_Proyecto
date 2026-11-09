using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uta.AcademicAnalysis.WebApi.Data;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.AcademicAnalysis.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/analysis")]
public sealed class AcademicAnalysisController(AnalysisDbContext dbContext) : ControllerBase
{
    private static readonly JsonSerializerOptions SnapshotJsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeZoneInfo EcuadorTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");

    [HttpGet("courses")]
    public async Task<ActionResult<IReadOnlyCollection<CursoDto>>> GetCourses(
        CancellationToken cancellationToken)
    {
        var query = dbContext.Courses.AsNoTracking();
        if (!User.IsInRole("Administrador técnico"))
        {
            var courseIds = GetCourseIds();
            query = query.Where(course => courseIds.Contains(course.MoodleCourseId));
        }

        var courses = await query.OrderBy(course => course.CourseName)
            .Select(course => new CursoDto(course.MoodleCourseId, course.CourseName))
            .ToListAsync(cancellationToken);
        return Ok(courses);
    }

    [HttpGet("courses/{courseId:int}/students")]
    public async Task<ActionResult<PaginaDto<EstudianteCursoDto>>> GetStudents(
        int courseId,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        if (page < 1 || pageSize is < 1 or > 100
            || ((long)page - 1) * pageSize > int.MaxValue)
        {
            return BadRequest("page debe ser positivo y pageSize debe estar entre 1 y 100.");
        }

        if (status is not null
            && status.Trim().ToLowerInvariant() is not ("calificado" or "pendiente" or "sin_entrega" or "sin_datos"))
        {
            return BadRequest("El filtro status debe ser calificado, pendiente, sin_entrega o sin_datos.");
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        var students = BuildStudents(snapshot, search, status?.Trim().ToLowerInvariant());
        var pageItems = students
            .OrderBy(item => item.Nombre, StringComparer.CurrentCulture)
            .ThenBy(item => item.MoodleId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();
        return Ok(new PaginaDto<EstudianteCursoDto>(
            pageItems,
            page,
            pageSize,
            students.Count,
            (int)Math.Ceiling(students.Count / (double)pageSize)));
    }

    [HttpGet("courses/{courseId:int}/indicators")]
    public async Task<ActionResult<IndicadoresCursoDto>> GetIndicators(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        var period = await GetActivePeriodAsync(snapshot, cancellationToken);
        return Ok(BuildIndicators(snapshot, period));
    }

    [HttpGet("courses/{courseId:int}/period")]
    public async Task<ActionResult<AcademicPeriodDto>> GetPeriod(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        return Ok(await GetActivePeriodAsync(snapshot, cancellationToken));
    }

    [HttpPut("courses/{courseId:int}/period")]
    [Authorize(Roles = "Administrador técnico")]
    public async Task<ActionResult<AcademicPeriodDto>> CorrectPeriod(
        int courseId,
        CorrectPeriodRequest request,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0
            || string.IsNullOrWhiteSpace(request.Name)
            || request.Name.Trim().Length > 150
            || request.StartsAt is null
            || request.EndsAt is null
            || request.StartsAt >= request.EndsAt)
        {
            return BadRequest("Indique nombre, inicio y fin válidos; el inicio debe ser anterior al fin.");
        }

        if (!await dbContext.Courses.AnyAsync(
                course => course.MoodleCourseId == courseId, cancellationToken))
        {
            return NotFound();
        }

        var period = await dbContext.Periods.SingleOrDefaultAsync(
            item => item.MoodleCourseId == courseId, cancellationToken);
        if (period is null)
        {
            period = new AcademicPeriodRecord { MoodleCourseId = courseId };
            dbContext.Periods.Add(period);
        }

        period.Name = request.Name.Trim();
        period.StartsAtUtc = request.StartsAt.Value.UtcDateTime;
        period.EndsAtUtc = request.EndsAt.Value.UtcDateTime;
        period.IsManuallyCorrected = true;
        period.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToPeriodDto(period, true));
    }

    private async Task<AcademicCourseSnapshot?> GetSnapshotAsync(
        int courseId,
        CancellationToken cancellationToken)
    {
        var json = await dbContext.Courses.AsNoTracking()
            .Where(course => course.MoodleCourseId == courseId)
            .Select(course => course.SnapshotJson)
            .SingleOrDefaultAsync(cancellationToken);
        return json is null
            ? null
            : JsonSerializer.Deserialize<AcademicCourseSnapshot>(json, SnapshotJsonOptions)
              ?? throw new JsonException($"La proyección del curso {courseId} es inválida.");
    }

    private async Task<AcademicPeriodDto> GetActivePeriodAsync(
        AcademicCourseSnapshot snapshot,
        CancellationToken cancellationToken)
    {
        var manual = await dbContext.Periods.AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.MoodleCourseId == snapshot.MoodleCourseId,
                cancellationToken);
        if (manual is not null && manual.IsManuallyCorrected)
        {
            return ToPeriodDto(manual, true);
        }

        if (snapshot.CourseStartsAtUtc is { } startsAt
            && snapshot.CourseEndsAtUtc is { } endsAt
            && startsAt < endsAt)
        {
            var now = DateTime.UtcNow;
            return new AcademicPeriodDto(
                snapshot.MoodleCourseId,
                $"Período Moodle ({TimeZoneInfo.ConvertTimeFromUtc(startsAt, EcuadorTimeZone):yyyy-MM-dd} - {TimeZoneInfo.ConvertTimeFromUtc(endsAt, EcuadorTimeZone):yyyy-MM-dd})",
                startsAt,
                endsAt,
                now >= startsAt && now <= endsAt,
                false,
                "America/Guayaquil");
        }

        return new AcademicPeriodDto(
            snapshot.MoodleCourseId,
            null,
            null,
            null,
            false,
            false,
            "America/Guayaquil",
            "Moodle no proporcionó fechas del curso; se requiere corrección administrativa.");
    }

    private static AcademicPeriodDto ToPeriodDto(AcademicPeriodRecord period, bool active)
    {
        var now = DateTime.UtcNow;
        return new AcademicPeriodDto(
            period.MoodleCourseId,
            period.Name,
            period.StartsAtUtc,
            period.EndsAtUtc,
            now >= period.StartsAtUtc && now <= period.EndsAtUtc,
            active,
            "America/Guayaquil");
    }

    private static List<EstudianteCursoDto> BuildStudents(
        AcademicCourseSnapshot snapshot,
        string? search,
        string? status)
    {
        var users = snapshot.EnrolledUsers
            .Where(user => user.Roles.Any(role =>
                role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)))
            .GroupBy(user => user.Id)
            .Select(group => group.First())
            .ToArray();
        var gradesByUser = snapshot.Grades.Grades
            .GroupBy(grade => grade.UserId)
            .ToDictionary(group => group.Key, group => group.ToArray());
        var itemsById = snapshot.Grades.Items.ToDictionary(item => item.Id);
        var result = new List<EstudianteCursoDto>();

        foreach (var user in users)
        {
            if (!string.IsNullOrWhiteSpace(search)
                && !user.FullName.Contains(search.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && !user.Email.Contains(search.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            gradesByUser.TryGetValue(user.Id, out var userGrades);
            var statuses = (userGrades ?? [])
                .Select(grade => GetGradeStatus(grade, itemsById))
                .ToArray();
            var studentStatus = statuses.Length == 0
                ? "sin_datos"
                : statuses.Contains("sin_entrega")
                    ? "sin_entrega"
                    : statuses.Contains("pendiente")
                        ? "pendiente"
                        : "calificado";
            if (status is not null && status != studentStatus && !(status == "sin_datos" && statuses.Length == 0))
            {
                continue;
            }

            var percentages = (userGrades ?? [])
                .Where(grade => decimal.TryParse(
                    grade.Grade,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out _)
                    && itemsById.TryGetValue(grade.ItemId, out var item)
                    && item.GradeMax > 0)
                .Select(grade =>
                {
                    var item = itemsById[grade.ItemId];
                    return decimal.Parse(
                        grade.Grade,
                        System.Globalization.NumberStyles.Number,
                        System.Globalization.CultureInfo.InvariantCulture) / item.GradeMax * 100m;
                })
                .ToArray();
            result.Add(new EstudianteCursoDto(
                user.Id,
                user.FullName,
                user.Email,
                percentages.Length == 0 ? null : decimal.Round(percentages.Average(), 2),
                studentStatus));
        }

        return result;
    }

    private static IndicadoresCursoDto BuildIndicators(
        AcademicCourseSnapshot snapshot,
        AcademicPeriodDto period)
    {
        var itemsById = snapshot.Grades.Items.ToDictionary(item => item.Id);
        var students = snapshot.EnrolledUsers
            .Where(user => user.Roles.Any(role =>
                role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)))
            .Select(user => user.Id)
            .Distinct()
            .ToHashSet();
        var grades = snapshot.Grades.Grades
            .Where(grade => students.Contains(grade.UserId))
            .Where(grade => IsInPeriod(grade, period))
            .ToArray();
        var numericGrades = grades
            .Where(grade => decimal.TryParse(
                grade.Grade,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out _)
                && itemsById.TryGetValue(grade.ItemId, out var item)
                && item.GradeMax > 0)
            .Select(grade => new
            {
                grade.ItemId,
                Percentage = decimal.Parse(
                    grade.Grade,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture)
                    / itemsById[grade.ItemId].GradeMax * 100m
            })
            .ToArray();
        var percentages = numericGrades.Select(grade => grade.Percentage).ToArray();
        var activityCount = snapshot.Grades.Items.Count(item => item.GradeMax > 0);
        var denominator = (long)students.Count * activityCount;
        decimal? progress = denominator == 0 || grades.Length == 0
            ? null
            : decimal.Round(Math.Min(100m,
                grades.Count(grade => decimal.TryParse(
                    grade.Grade,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out _)) * 100m / denominator), 2);
        var distribution = new[]
        {
            new DistribucionCalificacionDto("0–59", percentages.Count(value => value < 60)),
            new DistribucionCalificacionDto("60–69", percentages.Count(value => value is >= 60 and < 70)),
            new DistribucionCalificacionDto("70–84", percentages.Count(value => value is >= 70 and < 85)),
            new DistribucionCalificacionDto("85–100", percentages.Count(value => value is >= 85 and <= 100)),
            new DistribucionCalificacionDto(">100", percentages.Count(value => value > 100))
        };

        return new IndicadoresCursoDto(
            snapshot.MoodleCourseId,
            students.Count,
            percentages.Length == 0 ? null : decimal.Round(percentages.Average(), 2),
            percentages.Length == 0 ? null : 100m,
            numericGrades.Select(grade => grade.ItemId).Distinct().Count(),
            activityCount,
            progress,
            snapshot.UpdatedAtUtc,
            snapshot.SyncStatus,
            percentages.Length > 0,
            distribution);
    }

    private static bool IsInPeriod(MoodleUserGradeDto grade, AcademicPeriodDto period)
    {
        if (period.StartsAtUtc is null || period.EndsAtUtc is null
            || !long.TryParse(grade.DateGraded, out var timestamp)
            || timestamp <= 0)
        {
            return true;
        }

        var gradedAt = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        return gradedAt >= period.StartsAtUtc && gradedAt <= period.EndsAtUtc;
    }

    private static string GetGradeStatus(
        MoodleUserGradeDto grade,
        IReadOnlyDictionary<int, MoodleGradeItemDto> itemsById)
    {
        if (decimal.TryParse(
                grade.Grade,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out _))
        {
            return "calificado";
        }

        return long.TryParse(grade.DateSubmitted, out var submittedAt) && submittedAt > 0
            ? "pendiente"
            : "sin_entrega";
    }

    private bool CanAccessCourse(int courseId) =>
        courseId > 0
        && (User.IsInRole("Administrador técnico") || GetCourseIds().Contains(courseId));

    private int[] GetCourseIds() =>
        User.FindAll("course_id")
            .Select(claim => int.TryParse(claim.Value, out var courseId) ? courseId : 0)
            .Where(courseId => courseId > 0)
            .Distinct()
            .ToArray();

    public sealed record CorrectPeriodRequest(
        string Name,
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt);

    public sealed record AcademicPeriodDto(
        int CourseId,
        string? Name,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        bool IsActive,
        bool IsManuallyCorrected,
        string TimeZone,
        string? Warning = null);
}
