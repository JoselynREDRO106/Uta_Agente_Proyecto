using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uta.AcademicAnalysis.WebApi.Analysis;
using Uta.AcademicAnalysis.WebApi.Data;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.AcademicAnalysis.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/analysis")]
public sealed partial class AcademicAnalysisController(AnalysisDbContext dbContext) : ControllerBase
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

    [HttpGet("courses/{courseId:int}/students/{studentId:int}/tracking")]
    public async Task<ActionResult<SeguimientoEstudianteDto>> GetStudentTracking(
        int courseId,
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        var student = snapshot?.EnrolledUsers.FirstOrDefault(user =>
            user.Id == studentId
            && user.Roles.Any(role =>
                role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)));
        if (snapshot is null || student is null)
        {
            return NotFound();
        }

        var items = snapshot.Grades.Items
            .Where(item => item.GradeMax > 0
                && !item.ItemType.Equals("course", StringComparison.OrdinalIgnoreCase)
                && !item.ItemType.Equals("category", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var gradesByItem = snapshot.Grades.Grades
            .Where(grade => grade.UserId == studentId)
            .GroupBy(grade => grade.ItemId)
            .ToDictionary(group => group.Key, group => group.First());

        var notes = new List<SeguimientoNotaDto>();
        var activities = new List<SeguimientoActividadDto>();
        foreach (var item in items)
        {
            gradesByItem.TryGetValue(item.Id, out var grade);
            decimal? value = grade is not null
                && decimal.TryParse(
                    grade.Grade,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var parsed)
                    ? parsed
                    : null;
            notes.Add(new SeguimientoNotaDto(
                item.Id,
                item.ItemName,
                grade is null ? null : ParseUnixDate(grade.DateGraded) ?? ParseUnixDate(grade.DateSubmitted),
                value,
                item.GradeMin,
                item.GradeMax));
            activities.Add(new SeguimientoActividadDto(
                item.Id,
                item.ItemName,
                grade is null ? "NoDisponible" : ClassifyActivity(grade, value.HasValue)));
        }

        return Ok(new SeguimientoEstudianteDto(
            courseId,
            studentId,
            student.FullName,
            "Moodle",
            snapshot.UpdatedAtUtc,
            snapshot.SyncStatus,
            notes
                .OrderBy(note => note.FechaUtc is null)
                .ThenBy(note => note.FechaUtc)
                .ThenBy(note => note.Item, StringComparer.CurrentCulture)
                .ToArray(),
            activities));
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

    [HttpGet("courses/{courseId:int}/group-analysis")]
    public async Task<ActionResult<AnalisisGrupalDto>> GetGroupAnalysis(
        int courseId,
        [FromQuery] decimal threshold = 6m,
        [FromQuery] int minimumGrades = 7,
        CancellationToken cancellationToken = default)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        if (threshold is <= 0 or > 10 || minimumGrades is < 1 or > 1000)
        {
            return BadRequest("El umbral debe estar entre 0 y 10 y el mínimo entre 1 y 1000.");
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        return Ok(GroupAnalysisCalculator.Calculate(snapshot, threshold, minimumGrades));
    }

    [HttpGet("courses/{courseId:int}/recommendations")]
    public async Task<ActionResult<IReadOnlyCollection<RecomendacionDto>>> GetRecommendations(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var records = await dbContext.Recommendations.AsNoTracking()
            .Where(item => item.MoodleCourseId == courseId)
            .OrderByDescending(item => item.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return Ok(records.Select(ToRecommendationDto).ToArray());
    }

    [HttpPost("courses/{courseId:int}/recommendations/generate")]
    public async Task<ActionResult<ResultadoGenerarRecomendacionesDto>> GenerateRecommendations(
        int courseId,
        SolicitudGenerarRecomendacionesDto? request,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        request ??= new SolicitudGenerarRecomendacionesDto();
        if (request.Umbral is <= 0 or > 10
            || request.MinimoCalificaciones is < 1 or > 1000)
        {
            return BadRequest("El umbral debe estar entre 0 y 10 y el mínimo entre 1 y 1000.");
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        var analysis = GroupAnalysisCalculator.Calculate(
            snapshot, request.Umbral, request.MinimoCalificaciones);
        var sourceEventId = await dbContext.Courses.AsNoTracking()
            .Where(item => item.MoodleCourseId == courseId)
            .Select(item => item.SourceEventId)
            .SingleAsync(cancellationToken);
        var lowActivities = analysis.Actividades
            .Where(item => item.BajoUmbral && item.Promedio.HasValue)
            .ToArray();
        var created = 0;
        var actor = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "docente";

        foreach (var activity in lowActivities)
        {
            var exists = await dbContext.Recommendations.AnyAsync(
                item => item.SourceEventId == sourceEventId
                    && item.ActivityId == activity.ItemId,
                cancellationToken);
            if (exists)
            {
                continue;
            }

            var average = activity.Promedio!.Value;
            var justification =
                $"Promedio {average:0.00}/10 con {activity.NumCalificaciones} calificaciones; "
                + $"el umbral acordado es {request.Umbral:0.##}/10.";
            dbContext.Recommendations.Add(new AcademicRecommendationRecord
            {
                Id = Guid.NewGuid(),
                MoodleCourseId = courseId,
                ActivityId = activity.ItemId,
                ActivityName = activity.Nombre,
                Average = average,
                GradeCount = activity.NumCalificaciones,
                Threshold = request.Umbral,
                RecommendationText =
                    $"Revisar la estrategia de enseñanza de «{activity.Nombre}» y considerar "
                    + "una actividad de refuerzo con retroalimentación antes de la siguiente evaluación.",
                Justification = justification,
                IndicatorsJson = JsonSerializer.Serialize(new
                {
                    activityId = activity.ItemId,
                    average,
                    scale = activity.Escala,
                    gradeCount = activity.NumCalificaciones,
                    threshold = request.Umbral
                }),
                State = "Propuesta",
                SourceEventId = sourceEventId,
                CreatedBy = actor.Length <= 200 ? actor : actor[..200],
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            });
            created++;
        }

        if (created > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var records = await dbContext.Recommendations.AsNoTracking()
            .Where(item => item.MoodleCourseId == courseId
                && item.SourceEventId == sourceEventId)
            .OrderBy(item => item.ActivityName)
            .ToListAsync(cancellationToken);
        return Ok(new ResultadoGenerarRecomendacionesDto(
            created, records.Select(ToRecommendationDto).ToArray()));
    }

    [HttpPatch("courses/{courseId:int}/recommendations/{recommendationId:guid}/status")]
    public async Task<ActionResult<RecomendacionDto>> UpdateRecommendationStatus(
        int courseId,
        Guid recommendationId,
        ActualizarEstadoRecomendacionDto request,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var requestedState = request.Estado?.Trim();
        if (requestedState is not ("Aceptada" or "Ajustada" or "Descartada"))
        {
            return BadRequest("El estado debe ser Aceptada, Ajustada o Descartada.");
        }

        if (requestedState == "Ajustada"
            && (string.IsNullOrWhiteSpace(request.Texto) || request.Texto.Trim().Length > 1000))
        {
            return BadRequest("Una recomendación ajustada requiere un texto de hasta 1000 caracteres.");
        }

        var recommendation = await dbContext.Recommendations.SingleOrDefaultAsync(
            item => item.Id == recommendationId && item.MoodleCourseId == courseId,
            cancellationToken);
        if (recommendation is null)
        {
            return NotFound();
        }

        if (recommendation.State != "Propuesta")
        {
            return Conflict("Solo se puede cambiar una recomendación que siga en estado Propuesta.");
        }

        recommendation.State = requestedState;
        if (requestedState == "Ajustada")
        {
            recommendation.RecommendationText = request.Texto!.Trim();
        }

        recommendation.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToRecommendationDto(recommendation));
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

    private static RecomendacionDto ToRecommendationDto(AcademicRecommendationRecord item) =>
        new(
            item.Id,
            item.MoodleCourseId,
            item.ActivityId,
            item.ActivityName,
            item.Average,
            10m,
            item.GradeCount,
            item.Threshold,
            item.RecommendationText,
            item.Justification,
            item.State,
            item.CreatedAtUtc);

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

    private static DateTime? ParseUnixDate(string value) =>
        long.TryParse(value, out var timestamp) && timestamp > 0
            ? DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime
            : null;

    private static string ClassifyActivity(MoodleUserGradeDto grade, bool hasNumericGrade)
    {
        if (hasNumericGrade)
        {
            return "Completada";
        }

        return long.TryParse(grade.DateSubmitted, out var submittedAt) && submittedAt > 0
            ? "PendienteCalificar"
            : "SinEntrega";
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
