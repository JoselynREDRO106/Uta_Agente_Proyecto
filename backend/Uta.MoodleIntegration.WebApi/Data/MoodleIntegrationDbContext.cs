using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Application.DTOs;

namespace Uta.MoodleIntegration.WebApi.Data;

public sealed class MoodleIntegrationDbContext(
    DbContextOptions<MoodleIntegrationDbContext> options) : DbContext(options)
{
    public DbSet<MoodleConfigurationRecord> MoodleConfigurations => Set<MoodleConfigurationRecord>();
    public DbSet<AcademicCourseRecord> AcademicCourses => Set<AcademicCourseRecord>();
    public DbSet<IntegrationOutboxRecord> Outbox => Set<IntegrationOutboxRecord>();
    public DbSet<AcademicSyncRecord> AcademicSyncs => Set<AcademicSyncRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoodleConfigurationRecord>(entity =>
        {
            entity.ToTable("MOODLE_CONFIGURATION", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_MOODLE_CONFIGURATION");
            entity.Property(row => row.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(row => row.BaseUrl).HasColumnName("base_url").HasMaxLength(500).IsRequired();
            entity.Property(row => row.ProtectedToken).HasColumnName("protected_token")
                .HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(row => row.TimeoutSeconds).HasColumnName("timeout_seconds");
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("updated_at_utc");
        });

        modelBuilder.Entity<AcademicCourseRecord>(entity =>
        {
            entity.ToTable("ACADEMIC_COURSE", "dbo");
            entity.HasKey(row => row.MoodleCourseId).HasName("PK_ACADEMIC_COURSE");
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.CourseName).HasColumnName("nombre").HasMaxLength(200);
            entity.Property(row => row.SnapshotJson).HasColumnName("snapshot_json").HasColumnType("nvarchar(max)");
            entity.Property(row => row.UpdatedAtUtc).HasColumnName("updated_at_utc");
        });

        modelBuilder.Entity<IntegrationOutboxRecord>(entity =>
        {
            entity.ToTable("INTEGRATION_OUTBOX", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_INTEGRATION_OUTBOX");
            entity.Property(row => row.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(row => row.EventType).HasColumnName("event_type").HasMaxLength(120);
            entity.Property(row => row.PayloadJson).HasColumnName("payload_json").HasColumnType("nvarchar(max)");
            entity.Property(row => row.CreatedAtUtc).HasColumnName("created_at_utc");
            entity.Property(row => row.PublishedAtUtc).HasColumnName("published_at_utc");
            entity.Property(row => row.Attempts).HasColumnName("attempts");
            entity.Property(row => row.NextAttemptAtUtc).HasColumnName("next_attempt_at_utc");
            entity.Property(row => row.LastError).HasColumnName("last_error").HasMaxLength(1000);
            entity.HasIndex(row => new { row.PublishedAtUtc, row.NextAttemptAtUtc, row.CreatedAtUtc })
                .HasDatabaseName("IX_INTEGRATION_OUTBOX_PENDING");
        });

        modelBuilder.Entity<AcademicSyncRecord>(entity =>
        {
            entity.ToTable("ACADEMIC_SYNC_LOG", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_ACADEMIC_SYNC_LOG");
            entity.Property(row => row.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.Property(row => row.CourseName).HasColumnName("nombre_curso").HasMaxLength(200);
            entity.Property(row => row.CompletedAtUtc).HasColumnName("completed_at_utc");
            entity.Property(row => row.Status).HasColumnName("status").HasMaxLength(20);
            entity.Property(row => row.ErrorSummary).HasColumnName("error_summary").HasMaxLength(1000);
            entity.HasIndex(row => row.CompletedAtUtc).HasDatabaseName("IX_ACADEMIC_SYNC_LOG_DATE");
        });
    }
}

public sealed class MoodleConfigurationRecord
{
    public int Id { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ProtectedToken { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public sealed class AcademicCourseRecord
{
    public int MoodleCourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SnapshotJson { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
}

public sealed class IntegrationOutboxRecord
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public int Attempts { get; set; }
    public DateTime NextAttemptAtUtc { get; set; }
    public string? LastError { get; set; }
}

public sealed class AcademicSyncRecord
{
    public int Id { get; set; }
    public int MoodleCourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime CompletedAtUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorSummary { get; set; }
}
