using Microsoft.EntityFrameworkCore;

namespace Uta.AcademicAnalysis.WebApi.Data;

public sealed class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options)
    : DbContext(options)
{
    internal DbSet<CourseProjectionRecord> Courses => Set<CourseProjectionRecord>();
    internal DbSet<ProcessedEventRecord> ProcessedEvents => Set<ProcessedEventRecord>();
    internal DbSet<AcademicPeriodRecord> Periods => Set<AcademicPeriodRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
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
