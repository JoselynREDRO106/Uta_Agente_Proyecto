IF DB_ID(N'EduMonitorAnalysis') IS NULL
BEGIN
    CREATE DATABASE EduMonitorAnalysis;
END;
GO

USE EduMonitorAnalysis;
GO

IF OBJECT_ID(N'dbo.ACADEMIC_RECOMMENDATION', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACADEMIC_RECOMMENDATION
    (
        id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_ACADEMIC_RECOMMENDATION PRIMARY KEY,
        id_moodle_curso INT NOT NULL,
        id_actividad_moodle INT NOT NULL,
        actividad NVARCHAR(200) NOT NULL,
        promedio DECIMAL(6,2) NOT NULL,
        num_calificaciones INT NOT NULL,
        umbral DECIMAL(6,2) NOT NULL,
        texto NVARCHAR(1000) NOT NULL,
        justificacion NVARCHAR(1000) NOT NULL,
        indicadores_json NVARCHAR(MAX) NOT NULL,
        estado NVARCHAR(20) NOT NULL,
        source_event_id UNIQUEIDENTIFIER NOT NULL,
        creado_por NVARCHAR(200) NOT NULL,
        fecha_utc DATETIME2 NOT NULL,
        actualizado_utc DATETIME2 NOT NULL,
        CONSTRAINT CK_ACADEMIC_RECOMMENDATION_COUNT
            CHECK (num_calificaciones > 0),
        CONSTRAINT CK_ACADEMIC_RECOMMENDATION_STATE
            CHECK (estado IN (N'Propuesta', N'Aceptada', N'Ajustada', N'Descartada'))
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UQ_ACADEMIC_RECOMMENDATION_EVENT_ACTIVITY'
      AND object_id = OBJECT_ID(N'dbo.ACADEMIC_RECOMMENDATION')
)
    CREATE UNIQUE INDEX UQ_ACADEMIC_RECOMMENDATION_EVENT_ACTIVITY
        ON dbo.ACADEMIC_RECOMMENDATION(source_event_id, id_actividad_moodle);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ACADEMIC_RECOMMENDATION_COURSE_DATE'
      AND object_id = OBJECT_ID(N'dbo.ACADEMIC_RECOMMENDATION')
)
    CREATE INDEX IX_ACADEMIC_RECOMMENDATION_COURSE_DATE
        ON dbo.ACADEMIC_RECOMMENDATION(id_moodle_curso, fecha_utc DESC);
GO
