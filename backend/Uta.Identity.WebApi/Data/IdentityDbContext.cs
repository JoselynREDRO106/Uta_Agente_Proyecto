using Microsoft.EntityFrameworkCore;

namespace Uta.Identity.WebApi.Data;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : DbContext(options)
{
    internal DbSet<IdentityUserRow> Users => Set<IdentityUserRow>();
    internal DbSet<IdentityCourseAssignmentRow> CourseAssignments => Set<IdentityCourseAssignmentRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRow>(entity =>
        {
            entity.ToTable("USUARIO_SISTEMA", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_USUARIO_SISTEMA");
            entity.Property(row => row.Id).HasColumnName("id_usuario_sistema");
            entity.Property(row => row.Name).HasColumnName("nombre").HasMaxLength(150);
            entity.Property(row => row.Email).HasColumnName("email").HasMaxLength(254);
            entity.Property(row => row.PasswordHash).HasColumnName("clave_hash").HasMaxLength(256);
            entity.Property(row => row.Role).HasColumnName("rol").HasMaxLength(40);
            entity.Property(row => row.IsActive).HasColumnName("activo");
            entity.HasIndex(row => row.Email).IsUnique().HasDatabaseName("UQ_USUARIO_SISTEMA_email");
        });

        modelBuilder.Entity<IdentityCourseAssignmentRow>(entity =>
        {
            entity.ToTable("ASIGNACION_DOCENTE_CURSO", "dbo");
            entity.HasKey(row => new { row.UserId, row.MoodleCourseId })
                .HasName("PK_ASIGNACION_DOCENTE_CURSO");
            entity.Property(row => row.UserId).HasColumnName("id_usuario_sistema");
            entity.Property(row => row.MoodleCourseId).HasColumnName("id_moodle_curso");
            entity.HasOne<IdentityUserRow>().WithMany().HasForeignKey(row => row.UserId)
                .HasConstraintName("FK_ASIGNACION_USUARIO_SISTEMA");
        });
    }
}

internal sealed class IdentityUserRow
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

internal sealed class IdentityCourseAssignmentRow
{
    public int UserId { get; set; }
    public int MoodleCourseId { get; set; }
}
