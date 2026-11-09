IF DB_ID(N'EduMonitorIdentity') IS NULL
BEGIN
    CREATE DATABASE EduMonitorIdentity;
END;
GO

USE EduMonitorIdentity;
GO

IF OBJECT_ID(N'dbo.USUARIO_SISTEMA', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.USUARIO_SISTEMA
    (
        id_usuario_sistema INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_USUARIO_SISTEMA PRIMARY KEY,
        nombre NVARCHAR(150) NOT NULL,
        email NVARCHAR(254) NOT NULL
            CONSTRAINT UQ_USUARIO_SISTEMA_email UNIQUE,
        clave_hash NVARCHAR(256) NOT NULL,
        rol NVARCHAR(40) NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_USUARIO_SISTEMA_activo DEFAULT (1)
    );
END;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_USUARIO_SISTEMA_rol'
      AND CHARINDEX(N'233', definition) = 0
)
BEGIN
    ALTER TABLE dbo.USUARIO_SISTEMA DROP CONSTRAINT CK_USUARIO_SISTEMA_rol;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_USUARIO_SISTEMA_rol'
)
BEGIN
    ALTER TABLE dbo.USUARIO_SISTEMA
        ADD CONSTRAINT CK_USUARIO_SISTEMA_rol
        CHECK (rol IN (N'Docente', N'Administrador t' + NCHAR(233) + N'cnico'));
END;
GO

IF OBJECT_ID(N'dbo.ASIGNACION_DOCENTE_CURSO', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ASIGNACION_DOCENTE_CURSO
    (
        id_usuario_sistema INT NOT NULL,
        id_moodle_curso INT NOT NULL,
        CONSTRAINT PK_ASIGNACION_DOCENTE_CURSO
            PRIMARY KEY (id_usuario_sistema, id_moodle_curso),
        CONSTRAINT FK_ASIGNACION_USUARIO_SISTEMA
            FOREIGN KEY (id_usuario_sistema)
            REFERENCES dbo.USUARIO_SISTEMA(id_usuario_sistema)
    );
END;
GO

IF DB_ID(N'EduMonitor') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM EduMonitor.dbo.USUARIO_SISTEMA AS source
        JOIN dbo.USUARIO_SISTEMA AS target
          ON source.id_usuario_sistema = target.id_usuario_sistema
          OR LOWER(LTRIM(RTRIM(source.email))) = target.email
        WHERE source.email IS NOT NULL
          AND (source.id_usuario_sistema <> target.id_usuario_sistema
               OR LOWER(LTRIM(RTRIM(source.email))) <> target.email)
    )
    BEGIN
        THROW 51000, 'Conflicto de identidad al migrar cuentas; revise los IDs y correos antes de continuar.', 1;
    END;

    SET IDENTITY_INSERT dbo.USUARIO_SISTEMA ON;
    INSERT INTO dbo.USUARIO_SISTEMA
        (id_usuario_sistema, nombre, email, clave_hash, rol, activo)
    SELECT source.id_usuario_sistema,
           source.nombre,
           LOWER(LTRIM(RTRIM(source.email))),
           source.clave_hash,
           source.rol,
           source.activo
    FROM EduMonitor.dbo.USUARIO_SISTEMA AS source
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.USUARIO_SISTEMA AS target
        WHERE target.id_usuario_sistema = source.id_usuario_sistema
           OR target.email = LOWER(LTRIM(RTRIM(source.email)))
    );
    SET IDENTITY_INSERT dbo.USUARIO_SISTEMA OFF;

    INSERT INTO dbo.ASIGNACION_DOCENTE_CURSO (id_usuario_sistema, id_moodle_curso)
    SELECT assignment.id_usuario_sistema, course.id_moodle
    FROM EduMonitor.dbo.ASIGNACION_DOCENTE_CURSO AS assignment
    JOIN EduMonitor.dbo.CURSO AS course ON course.id_curso = assignment.id_curso
    JOIN dbo.USUARIO_SISTEMA AS target
      ON target.id_usuario_sistema = assignment.id_usuario_sistema
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.ASIGNACION_DOCENTE_CURSO AS existing
        WHERE existing.id_usuario_sistema = assignment.id_usuario_sistema
          AND existing.id_moodle_curso = course.id_moodle
    );
END;
GO
