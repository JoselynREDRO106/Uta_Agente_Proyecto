IF DB_ID(N'EduMonitorAnalysis') IS NULL
BEGIN
    CREATE DATABASE EduMonitorAnalysis;
END;
GO

USE EduMonitorAnalysis;
GO

IF OBJECT_ID(N'dbo.COURSE_PROJECTION', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.COURSE_PROJECTION
    (
        id_moodle_curso INT NOT NULL
            CONSTRAINT PK_COURSE_PROJECTION PRIMARY KEY,
        nombre NVARCHAR(200) NOT NULL,
        snapshot_json NVARCHAR(MAX) NOT NULL,
        updated_at_utc DATETIME2 NOT NULL,
        source_event_id UNIQUEIDENTIFIER NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.PROCESSED_EVENT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PROCESSED_EVENT
    (
        event_id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_PROCESSED_EVENT PRIMARY KEY,
        event_type NVARCHAR(120) NOT NULL,
        processed_at_utc DATETIME2 NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.ACADEMIC_PERIOD', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACADEMIC_PERIOD
    (
        id_moodle_curso INT NOT NULL
            CONSTRAINT PK_ACADEMIC_PERIOD PRIMARY KEY,
        nombre NVARCHAR(150) NOT NULL,
        starts_at_utc DATETIME2 NOT NULL,
        ends_at_utc DATETIME2 NOT NULL,
        manually_corrected BIT NOT NULL,
        updated_at_utc DATETIME2 NOT NULL,
        CONSTRAINT CK_ACADEMIC_PERIOD_DATES CHECK (starts_at_utc < ends_at_utc)
    );
END;
GO
