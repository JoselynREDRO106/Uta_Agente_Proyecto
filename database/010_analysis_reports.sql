IF DB_ID(N'EduMonitorAnalysis') IS NULL
BEGIN
    CREATE DATABASE EduMonitorAnalysis;
END;
GO

USE EduMonitorAnalysis;
GO

IF OBJECT_ID(N'dbo.ACADEMIC_REPORT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACADEMIC_REPORT
    (
        id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_ACADEMIC_REPORT PRIMARY KEY,
        id_moodle_curso INT NOT NULL,
        periodo NVARCHAR(150) NOT NULL,
        fecha_generacion_utc DATETIME2 NOT NULL,
        fecha_datos_utc DATETIME2 NOT NULL,
        datos_incompletos BIT NOT NULL,
        contenido_json NVARCHAR(MAX) NOT NULL,
        creado_por NVARCHAR(200) NOT NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ACADEMIC_REPORT_COURSE_DATE'
      AND object_id = OBJECT_ID(N'dbo.ACADEMIC_REPORT')
)
    CREATE INDEX IX_ACADEMIC_REPORT_COURSE_DATE
        ON dbo.ACADEMIC_REPORT(id_moodle_curso, fecha_generacion_utc DESC);
GO
