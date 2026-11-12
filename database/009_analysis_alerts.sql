IF DB_ID(N'EduMonitorAnalysis') IS NULL
BEGIN
    CREATE DATABASE EduMonitorAnalysis;
END;
GO

USE EduMonitorAnalysis;
GO

IF OBJECT_ID(N'dbo.ALERT_RULE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ALERT_RULE
    (
        id_moodle_curso INT NOT NULL,
        tipo NVARCHAR(30) NOT NULL,
        umbral DECIMAL(6,2) NOT NULL,
        activa BIT NOT NULL,
        actualizado_por NVARCHAR(200) NOT NULL,
        actualizado_utc DATETIME2 NOT NULL,
        CONSTRAINT PK_ALERT_RULE PRIMARY KEY (id_moodle_curso, tipo),
        CONSTRAINT CK_ALERT_RULE_TIPO
            CHECK (tipo IN (N'Descenso', N'Atraso', N'BajaFinalizacion')),
        CONSTRAINT CK_ALERT_RULE_UMBRAL CHECK (umbral > 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.ALERT_STATE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ALERT_STATE
    (
        id_moodle_curso INT NOT NULL,
        id_estudiante INT NOT NULL,
        tipo NVARCHAR(30) NOT NULL,
        estado NVARCHAR(20) NOT NULL,
        actualizado_por NVARCHAR(200) NOT NULL,
        actualizado_utc DATETIME2 NOT NULL,
        CONSTRAINT PK_ALERT_STATE PRIMARY KEY (id_moodle_curso, id_estudiante, tipo),
        CONSTRAINT CK_ALERT_STATE_TIPO
            CHECK (tipo IN (N'Descenso', N'Atraso', N'BajaFinalizacion')),
        CONSTRAINT CK_ALERT_STATE_ESTADO
            CHECK (estado IN (N'Nueva', N'Revisada', N'Descartada'))
    );
END;
GO
