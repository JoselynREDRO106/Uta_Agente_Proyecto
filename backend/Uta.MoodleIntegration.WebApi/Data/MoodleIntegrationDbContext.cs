using Microsoft.EntityFrameworkCore;

namespace Uta.MoodleIntegration.WebApi.Data;

public sealed class MoodleIntegrationDbContext(
    DbContextOptions<MoodleIntegrationDbContext> options) : DbContext(options)
{
    public DbSet<MoodleConfigurationRecord> MoodleConfigurations => Set<MoodleConfigurationRecord>();

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
