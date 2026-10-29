IF DB_ID(N'EduMonitor') IS NULL
BEGIN
    CREATE DATABASE EduMonitor;
END;
GO

USE EduMonitor;
GO

IF OBJECT_ID(N'dbo.CURSO', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CURSO
    (
        id_curso INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_CURSO PRIMARY KEY,
        id_moodle INT NOT NULL
            CONSTRAINT UQ_CURSO_id_moodle UNIQUE,
        nombre_moodle NVARCHAR(200) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.USUARIO', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.USUARIO
    (
        id_usuario INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_USUARIO PRIMARY KEY,
        id_moodle INT NOT NULL
            CONSTRAINT UQ_USUARIO_id_moodle UNIQUE,
        nombre NVARCHAR(150) NOT NULL,
        email NVARCHAR(150) NOT NULL,
        rol NVARCHAR(30) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.ACTIVIDAD', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACTIVIDAD
    (
        id_actividad INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_ACTIVIDAD PRIMARY KEY,
        id_moodle INT NOT NULL
            CONSTRAINT UQ_ACTIVIDAD_id_moodle UNIQUE,
        id_curso INT NOT NULL,
        nombre NVARCHAR(200) NOT NULL,
        categoria NVARCHAR(30) NOT NULL,
        CONSTRAINT FK_ACTIVIDAD_CURSO
            FOREIGN KEY (id_curso) REFERENCES dbo.CURSO(id_curso),
        CONSTRAINT CK_ACTIVIDAD_Categoria
            CHECK (categoria IN (N'Evaluaciones', N'Tareas', N'AP'))
    );
END;
GO

IF OBJECT_ID(N'dbo.CALIFICACION', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CALIFICACION
    (
        id_calificacion INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_CALIFICACION PRIMARY KEY,
        id_usuario INT NOT NULL,
        id_actividad INT NOT NULL,
        nota DECIMAL(5,2) NULL,
        estado NVARCHAR(30) NOT NULL,
        CONSTRAINT FK_CALIFICACION_USUARIO
            FOREIGN KEY (id_usuario) REFERENCES dbo.USUARIO(id_usuario),
        CONSTRAINT FK_CALIFICACION_ACTIVIDAD
            FOREIGN KEY (id_actividad) REFERENCES dbo.ACTIVIDAD(id_actividad),
        CONSTRAINT UQ_CALIFICACION_USUARIO_ACTIVIDAD
            UNIQUE (id_usuario, id_actividad),
        CONSTRAINT CK_CALIFICACION_Estado
            CHECK (estado IN (N'calificado', N'sin_entrega', N'pendiente')),
        CONSTRAINT CK_CALIFICACION_NotaEstado
            CHECK (
                (estado = N'calificado' AND nota IS NOT NULL)
                OR (estado IN (N'sin_entrega', N'pendiente') AND nota IS NULL)
            )
    );
END;
GO

IF OBJECT_ID(N'dbo.MAPEO_TEMATICO', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MAPEO_TEMATICO
    (
        id_mapeo INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_MAPEO_TEMATICO PRIMARY KEY,
        id_actividad INT NOT NULL,
        contenido_minimo NVARCHAR(150) NOT NULL,
        CONSTRAINT FK_MAPEO_TEMATICO_ACTIVIDAD
            FOREIGN KEY (id_actividad) REFERENCES dbo.ACTIVIDAD(id_actividad)
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ACTIVIDAD_id_curso'
      AND object_id = OBJECT_ID(N'dbo.ACTIVIDAD')
)
    CREATE INDEX IX_ACTIVIDAD_id_curso ON dbo.ACTIVIDAD(id_curso);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_CALIFICACION_id_usuario'
      AND object_id = OBJECT_ID(N'dbo.CALIFICACION')
)
    CREATE INDEX IX_CALIFICACION_id_usuario ON dbo.CALIFICACION(id_usuario);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_CALIFICACION_id_actividad'
      AND object_id = OBJECT_ID(N'dbo.CALIFICACION')
)
    CREATE INDEX IX_CALIFICACION_id_actividad ON dbo.CALIFICACION(id_actividad);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MAPEO_TEMATICO_id_actividad'
      AND object_id = OBJECT_ID(N'dbo.MAPEO_TEMATICO')
)
    CREATE INDEX IX_MAPEO_TEMATICO_id_actividad ON dbo.MAPEO_TEMATICO(id_actividad);
GO
