IF DB_ID(N'MoodleIntegration') IS NULL
BEGIN
    THROW 51001, 'Ejecute primero database/003_moodle_integration.sql.', 1;
END;
GO

USE MoodleIntegration;
GO

IF OBJECT_ID(N'dbo.ACADEMIC_COURSE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACADEMIC_COURSE
    (
        id_moodle_curso INT NOT NULL
            CONSTRAINT PK_ACADEMIC_COURSE PRIMARY KEY,
        nombre NVARCHAR(200) NOT NULL,
        snapshot_json NVARCHAR(MAX) NOT NULL,
        updated_at_utc DATETIME2 NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.ACADEMIC_SYNC_LOG', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACADEMIC_SYNC_LOG
    (
        id INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_ACADEMIC_SYNC_LOG PRIMARY KEY,
        id_moodle_curso INT NOT NULL,
        nombre_curso NVARCHAR(200) NOT NULL,
        completed_at_utc DATETIME2 NOT NULL,
        status NVARCHAR(20) NOT NULL,
        error_summary NVARCHAR(1000) NULL,
        CONSTRAINT CK_ACADEMIC_SYNC_LOG_status CHECK (status IN (N'Completa', N'Incompleta'))
    );
END;
GO

IF OBJECT_ID(N'dbo.INTEGRATION_OUTBOX', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.INTEGRATION_OUTBOX
    (
        id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_INTEGRATION_OUTBOX PRIMARY KEY,
        event_type NVARCHAR(120) NOT NULL,
        payload_json NVARCHAR(MAX) NOT NULL,
        created_at_utc DATETIME2 NOT NULL,
        published_at_utc DATETIME2 NULL,
        attempts INT NOT NULL CONSTRAINT DF_INTEGRATION_OUTBOX_attempts DEFAULT (0),
        next_attempt_at_utc DATETIME2 NOT NULL,
        last_error NVARCHAR(1000) NULL
    );

    CREATE INDEX IX_INTEGRATION_OUTBOX_PENDING
        ON dbo.INTEGRATION_OUTBOX (published_at_utc, next_attempt_at_utc, created_at_utc);
END;
GO

IF DB_ID(N'EduMonitor') IS NOT NULL
   AND OBJECT_ID(N'EduMonitor.dbo.CURSO', N'U') IS NOT NULL
   AND OBJECT_ID(N'EduMonitor.dbo.USUARIO', N'U') IS NOT NULL
   AND OBJECT_ID(N'EduMonitor.dbo.ACTIVIDAD', N'U') IS NOT NULL
   AND OBJECT_ID(N'EduMonitor.dbo.CALIFICACION', N'U') IS NOT NULL
   AND OBJECT_ID(N'EduMonitor.dbo.MATRICULA', N'U') IS NOT NULL
BEGIN
    DECLARE @courseId INT;
    DECLARE @courseName NVARCHAR(200);
    DECLARE @updatedAtUtc DATETIME2;
    DECLARE @snapshot NVARCHAR(MAX);
    DECLARE @eventId UNIQUEIDENTIFIER;
    DECLARE @eventPayload NVARCHAR(MAX);
    DECLARE @eventExists BIT;

    DECLARE course_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT id_moodle, nombre_moodle
        FROM EduMonitor.dbo.CURSO;

    OPEN course_cursor;
    FETCH NEXT FROM course_cursor INTO @courseId, @courseName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.ACADEMIC_COURSE
                WHERE id_moodle_curso = @courseId
            )
            OR EXISTS
            (
                SELECT 1
                FROM dbo.ACADEMIC_COURSE
                WHERE id_moodle_curso = @courseId
                  AND JSON_QUERY(snapshot_json, '$.Contents.Sections') IS NULL
            )
            OR EXISTS
            (
                SELECT 1
                FROM dbo.INTEGRATION_OUTBOX
                WHERE event_type = N'academic.course.updated.v1'
                  AND TRY_CONVERT(INT, JSON_VALUE(payload_json, '$.Snapshot.MoodleCourseId')) = @courseId
                  AND JSON_QUERY(payload_json, '$.Snapshot.Contents.Sections') IS NULL
            )
        BEGIN
            SET @updatedAtUtc = NULL;
            SELECT TOP (1) @updatedAtUtc = fecha_utc
            FROM EduMonitor.dbo.SINCRONIZACION_LOG AS log
            JOIN EduMonitor.dbo.CURSO AS course ON course.id_curso = log.id_curso
            WHERE course.id_moodle = @courseId
            ORDER BY log.fecha_utc DESC, log.id_sincronizacion DESC;
            SET @updatedAtUtc = COALESCE(@updatedAtUtc, SYSUTCDATETIME());

            SELECT @snapshot =
            (
                SELECT
                    @courseId AS MoodleCourseId,
                    @courseName AS CourseName,
                    @updatedAtUtc AS UpdatedAtUtc,
                    CAST(NULL AS DATETIME2) AS CourseStartsAtUtc,
                    CAST(NULL AS DATETIME2) AS CourseEndsAtUtc,
                    JSON_QUERY
                    (
                        (
                            SELECT
                                user_row.id_moodle AS Id,
                                user_row.nombre AS FullName,
                                user_row.email AS Email,
                                JSON_QUERY
                                (
                                    (
                                        SELECT N'student' AS ShortName
                                        FOR JSON PATH
                                    )
                                ) AS Roles
                            FROM EduMonitor.dbo.MATRICULA AS enrollment
                            JOIN EduMonitor.dbo.CURSO AS course
                              ON course.id_curso = enrollment.id_curso
                            JOIN EduMonitor.dbo.USUARIO AS user_row
                              ON user_row.id_usuario = enrollment.id_usuario
                            WHERE course.id_moodle = @courseId
                            FOR JSON PATH
                        )
                    ) AS EnrolledUsers,
                    JSON_QUERY
                    (
                        (
                            SELECT
                                JSON_QUERY
                                (
                                    (
                                        SELECT
                                            activity.id_moodle AS Id,
                                            activity.nombre AS ItemName,
                                            N'mod' AS ItemType,
                                            activity.categoria AS ItemModule,
                                            activity.id_moodle AS cmid,
                                            CAST(0 AS DECIMAL(5,2)) AS GradeMin,
                                            COALESCE(activity.nota_maxima, 0) AS GradeMax
                                        FROM EduMonitor.dbo.ACTIVIDAD AS activity
                                        JOIN EduMonitor.dbo.CURSO AS course
                                          ON course.id_curso = activity.id_curso
                                        WHERE course.id_moodle = @courseId
                                        FOR JSON PATH
                                    )
                                ) AS Items,
                                JSON_QUERY
                                (
                                    (
                                        SELECT
                                            user_row.id_moodle AS UserId,
                                            activity.id_moodle AS ItemId,
                                            COALESCE(CONVERT(NVARCHAR(40), grade.nota), N'') AS Grade,
                                            CASE WHEN grade.estado = N'pendiente' THEN N'1' ELSE N'0' END AS DateSubmitted,
                                            CASE WHEN grade.estado = N'calificado'
                                                THEN CONVERT(NVARCHAR(40), DATEDIFF_BIG(SECOND, '19700101', @updatedAtUtc))
                                                ELSE N''
                                            END AS DateGraded
                                        FROM EduMonitor.dbo.CALIFICACION AS grade
                                        JOIN EduMonitor.dbo.USUARIO AS user_row
                                          ON user_row.id_usuario = grade.id_usuario
                                        JOIN EduMonitor.dbo.ACTIVIDAD AS activity
                                          ON activity.id_actividad = grade.id_actividad
                                        JOIN EduMonitor.dbo.CURSO AS course
                                          ON course.id_curso = activity.id_curso
                                        WHERE course.id_moodle = @courseId
                                        FOR JSON PATH
                                    )
                                ) AS Grades,
                                JSON_QUERY(N'[]') AS Warnings
                            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                        )
                    ) AS Grades,
                    JSON_QUERY
                    (
                        (
                            SELECT JSON_QUERY
                            (
                                (
                                    SELECT
                                        @courseId AS Id,
                                        @courseId AS CourseId,
                                        N'Contenido importado' AS Name,
                                        0 AS Section,
                                        JSON_QUERY
                                        (
                                            (
                                                SELECT
                                                    activity.id_moodle AS Id,
                                                    activity.nombre AS Name,
                                                    activity.categoria AS ModName
                                                FROM EduMonitor.dbo.ACTIVIDAD AS activity
                                                JOIN EduMonitor.dbo.CURSO AS course
                                                  ON course.id_curso = activity.id_curso
                                                WHERE course.id_moodle = @courseId
                                                FOR JSON PATH
                                            )
                                        ) AS Modules
                                    FOR JSON PATH
                                )
                            ) AS Sections
                            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                        )
                    ) AS Contents,
                    N'Completa' AS SyncStatus,
                    CAST(NULL AS NVARCHAR(1000)) AS ErrorSummary
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            );

            BEGIN TRANSACTION;
            IF EXISTS (SELECT 1 FROM dbo.ACADEMIC_COURSE WHERE id_moodle_curso = @courseId)
            BEGIN
                UPDATE dbo.ACADEMIC_COURSE
                SET nombre = @courseName,
                    snapshot_json = @snapshot,
                    updated_at_utc = @updatedAtUtc
                WHERE id_moodle_curso = @courseId;
            END
            ELSE
            BEGIN
                INSERT INTO dbo.ACADEMIC_COURSE
                    (id_moodle_curso, nombre, snapshot_json, updated_at_utc)
                VALUES
                    (@courseId, @courseName, @snapshot, @updatedAtUtc);
            END;

            SET @eventId = NULL;
            SET @eventExists = 0;
            SELECT TOP (1)
                @eventId = id,
                @eventExists = 1
            FROM dbo.INTEGRATION_OUTBOX
            WHERE event_type = N'academic.course.updated.v1'
              AND TRY_CONVERT(INT, JSON_VALUE(payload_json, '$.Snapshot.MoodleCourseId')) = @courseId
              AND JSON_QUERY(payload_json, '$.Snapshot.Contents.Sections') IS NULL
            ORDER BY created_at_utc;
            SET @eventId = COALESCE(@eventId, NEWID());
            SELECT @eventPayload =
            (
                SELECT
                    @eventId AS EventId,
                    N'academic.course.updated.v1' AS EventType,
                    @updatedAtUtc AS OccurredAtUtc,
                    JSON_QUERY(@snapshot) AS Snapshot
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            );
            IF @eventExists = 1
            BEGIN
                UPDATE dbo.INTEGRATION_OUTBOX
                SET payload_json = @eventPayload,
                    created_at_utc = @updatedAtUtc,
                    published_at_utc = NULL,
                    attempts = 0,
                    next_attempt_at_utc = @updatedAtUtc,
                    last_error = NULL
                WHERE id = @eventId;
            END
            ELSE
            BEGIN
                INSERT INTO dbo.INTEGRATION_OUTBOX
                    (id, event_type, payload_json, created_at_utc, next_attempt_at_utc)
                VALUES
                    (@eventId, N'academic.course.updated.v1', @eventPayload, @updatedAtUtc, @updatedAtUtc);
            END;
            COMMIT TRANSACTION;
        END;

        FETCH NEXT FROM course_cursor INTO @courseId, @courseName;
    END;

    CLOSE course_cursor;
    DEALLOCATE course_cursor;
END;
GO
