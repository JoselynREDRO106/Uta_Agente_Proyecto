using Microsoft.EntityFrameworkCore;

namespace Uta.AcademicAnalysis.WebApi.Data;

public sealed class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options)
    : DbContext(options)
{
    internal DbSet<CourseProjectionRecord> Courses => Set<CourseProjectionRecord>();
    internal DbSet<ProcessedEventRecord> ProcessedEvents => Set<ProcessedEventRecord>();
    internal DbSet<AcademicPeriodRecord> Periods => Set<AcademicPeriodRecord>();
    internal DbSet<AcademicRecommendationRecord> Recommendations => Set<AcademicRecommendationRecord>();
    internal DbSet<AlertRuleRecord> AlertRules => Set<AlertRuleRecord>();
    internal DbSet<AlertStateRecord> AlertStates => Set<AlertStateRecord>();

    internal DbSet<AcademicReportRecord> Reports => Set<AcademicReportRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicReportRecord>(entity =>
        {
            entity.ToTable("ACADEMIC_REPORT", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_ACADEMIC_REPORT");
            entity.Property(row => row.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.Period).HasColumnName("periodo").HasMaxLength(150);
            entity.Property(row => row.GeneratedAtUtc).HasColumnName("fecha_generacion_utc");
            entity.Property(row => row.DataAtUtc).HasColumnName("fecha_datos_utc");
            entity.Property(row => row.IncompleteData).HasColumnName("datos_incompletos");
            entity.Property(row => row.ContentJson).HasColumnName("contenido_json")
                .HasColumnType("nvarchar(max)");
            entity.Property(row => row.CreatedBy).HasColumnName("creado_por").HasMaxLength(200);
                    });

        modelBuilder.Entity<AlertRuleRecord>(entity =>
        {
            entity.ToTable("ALERT_RULE", "dbo");
            entity.HasKey(row => new { row.MoodleCourseId, row.Tipo }).HasName("PK_ALERT_RULE");
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.Tipo).HasColumnName("tipo").HasMaxLength(30);
            entity.Property(row => row.Threshold).HasColumnName("umbral").HasPrecision(6, 2);
            entity.Property(row => row.Active).HasColumnName("activa");
            entity.Property(row => row.UpdatedBy).HasColumnName("actualizado_por").HasMaxLength(200);
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("actualizado_utc");
        });

        modelBuilder.Entity<AlertStateRecord>(entity =>
        {
            entity.ToTable("ALERT_STATE", "dbo");
            entity.HasKey(row => new { row.MoodleCourseId, row.StudentId, row.Tipo })
                .HasName("PK_ALERT_STATE");
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.StudentId).HasColumnName("id_estudiante");
            entity.Property(row => row.Tipo).HasColumnName("tipo").HasMaxLength(30);
            entity.Property(row => row.State).HasColumnName("estado").HasMaxLength(20);
            entity.Property(row => row.UpdatedBy).HasColumnName("actualizado_por").HasMaxLength(200);
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("actualizado_utc");
        });

        modelBuilder.Entity<CourseProjectionRecord>(entity =>
        {
            entity.ToTable("COURSE_PROJECTION", "dbo");
            entity.HasKey(row => row.MoodleCourseId).HasName("PK_COURSE_PROJECTION");
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.CourseName).HasColumnName("nombre").HasMaxLength(200);
            entity.Property(row => row.SnapshotJson).HasColumnName("snapshot_json")
                .HasColumnType("nvarchar(max)");
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("updated_at_utc");
            entity.Property(row => row.SourceEventId).HasColumnName("source_event_id");
        });

        modelBuilder.Entity<ProcessedEventRecord>(entity =>
        {
            entity.ToTable("PROCESSED_EVENT", "dbo");
            entity.HasKey(row => row.EventId).HasName("PK_PROCESSED_EVENT");
            entity.Property(row => row.EventId).HasColumnName("event_id").ValueGeneratedNever();
            entity.Property(row => row.EventType).HasColumnName("event_type").HasMaxLength(120);
            entity.Property(row => row.ProcessedAtUtc).HasColumnName("processed_at_utc");
        });

        modelBuilder.Entity<AcademicPeriodRecord>(entity =>
        {
            entity.ToTable("ACADEMIC_PERIOD", "dbo");
            entity.HasKey(row => row.MoodleCourseId).HasName("PK_ACADEMIC_PERIOD");
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.Name).HasColumnName("nombre").HasMaxLength(150);
            entity.Property(row => row.StartsAtUtc).HasColumnName("starts_at_utc");
            entity.Property(row => row.EndsAtUtc).HasColumnName("ends_at_utc");
            entity.Property(row => row.IsManuallyCorrected).HasColumnName("manually_corrected");
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("updated_at_utc");
        });

        modelBuilder.Entity<AcademicRecommendationRecord>(entity =>
        {
            entity.ToTable("ACADEMIC_RECOMMENDATION", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_ACADEMIC_RECOMMENDATION");
            entity.Property(row => row.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.ActivityId).HasColumnName("id_actividad_moodle");
            entity.Property(row => row.ActivityName).HasColumnName("actividad").HasMaxLength(200);
            entity.Property(row => row.Average).HasColumnName("promedio").HasPrecision(6, 2);
            entity.Property(row => row.GradeCount).HasColumnName("num_calificaciones");
            entity.Property(row => row.Threshold).HasColumnName("umbral").HasPrecision(6, 2);
            entity.Property(row => row.RecommendationText).HasColumnName("texto").HasMaxLength(1000);
            entity.Property(row => row.Justification).HasColumnName("justificacion").HasMaxLength(1000);
            entity.Property(row => row.IndicatorsJson).HasColumnName("indicadores_json")
                .HasColumnType("nvarchar(max)");
            entity.Property(row => row.State).HasColumnName("estado").HasMaxLength(20);
            entity.Property(row => row.SourceEventId).HasColumnName("source_event_id");
            entity.Property(row => row.CreatedBy).HasColumnName("creado_por").HasMaxLength(200);
            entity.Property(row => row.CreatedAtUtc).HasColumnName("fecha_utc");
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("actualizado_utc");
            entity.HasIndex(row => new { row.SourceEventId, row.ActivityId })
                .IsUnique()
                .HasDatabaseName("UQ_ACADEMIC_RECOMMENDATION_EVENT_ACTIVITY");
            entity.HasIndex(row => new { row.MoodleCourseId, row.CreatedAtUtc })
                .HasDatabaseName("IX_ACADEMIC_RECOMMENDATION_COURSE_DATE");
        });
    }
}

internal sealed class AcademicReportRecord
{
    public Guid Id { get; set; }
    public int MoodleCourseId { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; set; }
    public DateTime DataAtUtc { get; set; }
    public bool IncompleteData { get; set; }
    public string ContentJson { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

internal sealed class AlertRuleRecord
{
    public int MoodleCourseId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Threshold { get; set; }
    public bool Active { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
}

internal sealed class AlertStateRecord
{
    public int MoodleCourseId { get; set; }
    public int StudentId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string State { get; set; } = "Nueva";
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
}

internal sealed class CourseProjectionRecord
{
    public int MoodleCourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SnapshotJson { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
    public Guid SourceEventId { get; set; }
}

internal sealed class ProcessedEventRecord
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime ProcessedAtUtc { get; set; }
}

internal sealed class AcademicPeriodRecord
{
    public int MoodleCourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsManuallyCorrected { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

internal sealed class AcademicRecommendationRecord
{
    public Guid Id { get; set; }
    public int MoodleCourseId { get; set; }
    public int ActivityId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public decimal Average { get; set; }
    public int GradeCount { get; set; }
    public decimal Threshold { get; set; }
    public string RecommendationText { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public string IndicatorsJson { get; set; } = string.Empty;
    public string State { get; set; } = "Propuesta";
    public Guid SourceEventId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
