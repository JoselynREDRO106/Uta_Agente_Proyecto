using Microsoft.EntityFrameworkCore;

namespace Uta.Middleware.Infrastructure.Data;

public sealed class EduMonitorDbContext(DbContextOptions<EduMonitorDbContext> options)
    : DbContext(options)
{
    internal DbSet<CursoRow> Cursos => Set<CursoRow>();
    internal DbSet<UsuarioRow> Usuarios => Set<UsuarioRow>();
    internal DbSet<ActividadRow> Actividades => Set<ActividadRow>();
    internal DbSet<CalificacionRow> Calificaciones => Set<CalificacionRow>();
    internal DbSet<MapeoTematicoRow> MapeosTematicos => Set<MapeoTematicoRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CursoRow>(entity =>
        {
            entity.ToTable("CURSO", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_CURSO");
            entity.Property(row => row.Id).HasColumnName("id_curso");
            entity.Property(row => row.MoodleId).HasColumnName("id_moodle");
            entity.Property(row => row.Name).HasColumnName("nombre_moodle").HasMaxLength(200);
            entity.HasIndex(row => row.MoodleId).IsUnique().HasDatabaseName("UQ_CURSO_id_moodle");
        });

        modelBuilder.Entity<UsuarioRow>(entity =>
        {
            entity.ToTable("USUARIO", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_USUARIO");
            entity.Property(row => row.Id).HasColumnName("id_usuario");
            entity.Property(row => row.MoodleId).HasColumnName("id_moodle");
            entity.Property(row => row.Name).HasColumnName("nombre").HasMaxLength(150);
            entity.Property(row => row.Email).HasColumnName("email").HasMaxLength(150);
            entity.Property(row => row.Role).HasColumnName("rol").HasMaxLength(30);
            entity.HasIndex(row => row.MoodleId).IsUnique().HasDatabaseName("UQ_USUARIO_id_moodle");
        });

        modelBuilder.Entity<ActividadRow>(entity =>
        {
            entity.ToTable("ACTIVIDAD", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_ACTIVIDAD");
            entity.Property(row => row.Id).HasColumnName("id_actividad");
            entity.Property(row => row.MoodleId).HasColumnName("id_moodle");
            entity.Property(row => row.CourseId).HasColumnName("id_curso");
            entity.Property(row => row.Name).HasColumnName("nombre").HasMaxLength(200);
            entity.Property(row => row.Category).HasColumnName("categoria").HasMaxLength(30);
            entity.HasIndex(row => row.MoodleId).IsUnique().HasDatabaseName("UQ_ACTIVIDAD_id_moodle");
            entity.HasOne<CursoRow>().WithMany().HasForeignKey(row => row.CourseId)
                .HasConstraintName("FK_ACTIVIDAD_CURSO");
            entity.HasIndex(row => row.CourseId).HasDatabaseName("IX_ACTIVIDAD_id_curso");
        });

        modelBuilder.Entity<CalificacionRow>(entity =>
        {
            entity.ToTable("CALIFICACION", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_CALIFICACION");
            entity.Property(row => row.Id).HasColumnName("id_calificacion");
            entity.Property(row => row.UserId).HasColumnName("id_usuario");
            entity.Property(row => row.ActivityId).HasColumnName("id_actividad");
            entity.Property(row => row.Grade).HasColumnName("nota").HasPrecision(5, 2);
            entity.Property(row => row.Status).HasColumnName("estado").HasMaxLength(30);
            entity.HasIndex(row => new { row.UserId, row.ActivityId }).IsUnique()
                .HasDatabaseName("UQ_CALIFICACION_USUARIO_ACTIVIDAD");
            entity.HasIndex(row => row.UserId).HasDatabaseName("IX_CALIFICACION_id_usuario");
            entity.HasIndex(row => row.ActivityId).HasDatabaseName("IX_CALIFICACION_id_actividad");
            entity.HasOne<UsuarioRow>().WithMany().HasForeignKey(row => row.UserId)
                .HasConstraintName("FK_CALIFICACION_USUARIO");
            entity.HasOne<ActividadRow>().WithMany().HasForeignKey(row => row.ActivityId)
                .HasConstraintName("FK_CALIFICACION_ACTIVIDAD");
        });

        modelBuilder.Entity<MapeoTematicoRow>(entity =>
        {
            entity.ToTable("MAPEO_TEMATICO", "dbo");
            entity.HasKey(row => row.Id).HasName("PK_MAPEO_TEMATICO");
            entity.Property(row => row.Id).HasColumnName("id_mapeo");
            entity.Property(row => row.ActivityId).HasColumnName("id_actividad");
            entity.Property(row => row.MinimumContent).HasColumnName("contenido_minimo").HasMaxLength(150);
            entity.HasOne<ActividadRow>().WithMany().HasForeignKey(row => row.ActivityId)
                .HasConstraintName("FK_MAPEO_TEMATICO_ACTIVIDAD");
            entity.HasIndex(row => row.ActivityId).HasDatabaseName("IX_MAPEO_TEMATICO_id_actividad");
        });
    }
}

internal sealed class CursoRow
{
    public int Id { get; set; }
    public int MoodleId { get; set; }
    public string Name { get; set; } = string.Empty;
}

internal sealed class UsuarioRow
{
    public int Id { get; set; }
    public int MoodleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

internal sealed class ActividadRow
{
    public int Id { get; set; }
    public int MoodleId { get; set; }
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

internal sealed class CalificacionRow
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ActivityId { get; set; }
    public decimal? Grade { get; set; }
    public string Status { get; set; } = string.Empty;
}

internal sealed class MapeoTematicoRow
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public string MinimumContent { get; set; } = string.Empty;
}
