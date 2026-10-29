USE EduMonitor;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM dbo.CURSO WHERE id_moodle = 1001)
BEGIN
    INSERT INTO dbo.CURSO (id_moodle, nombre_moodle)
    VALUES (1001, N'Servicios Web - Curso de prueba');
END;

INSERT INTO dbo.USUARIO (id_moodle, nombre, email, rol)
SELECT sample.id_moodle, sample.nombre, sample.email, N'Estudiante'
FROM (VALUES
    (2001, N'Ana Ejemplo', N'ana@example.test'),
    (2002, N'Luis Ejemplo', N'luis@example.test'),
    (2003, N'Maria Ejemplo', N'maria@example.test')
) AS sample(id_moodle, nombre, email)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.USUARIO AS existing
    WHERE existing.id_moodle = sample.id_moodle
);

INSERT INTO dbo.ACTIVIDAD (id_moodle, id_curso, nombre, categoria)
SELECT sample.id_moodle, course.id_curso, sample.nombre, sample.categoria
FROM (VALUES
    (3001, N'Evaluacion U1', N'Evaluaciones'),
    (3002, N'Tarea U1', N'Tareas'),
    (3003, N'Practica U1', N'AP')
) AS sample(id_moodle, nombre, categoria)
CROSS JOIN dbo.CURSO AS course
WHERE course.id_moodle = 1001
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.ACTIVIDAD AS existing
      WHERE existing.id_moodle = sample.id_moodle
  );

INSERT INTO dbo.CALIFICACION (id_usuario, id_actividad, nota, estado)
SELECT user_record.id_usuario, activity_record.id_actividad, sample.nota, sample.estado
FROM (VALUES
    (2001, 3001, CONVERT(DECIMAL(5,2), 0.00), N'calificado'),
    (2001, 3002, CONVERT(DECIMAL(5,2), NULL), N'pendiente'),
    (2001, 3003, CONVERT(DECIMAL(5,2), NULL), N'sin_entrega'),
    (2002, 3001, CONVERT(DECIMAL(5,2), 78.00), N'calificado'),
    (2002, 3002, CONVERT(DECIMAL(5,2), 82.00), N'calificado'),
    (2002, 3003, CONVERT(DECIMAL(5,2), 91.00), N'calificado'),
    (2003, 3001, CONVERT(DECIMAL(5,2), 63.00), N'calificado'),
    (2003, 3002, CONVERT(DECIMAL(5,2), 70.00), N'calificado'),
    (2003, 3003, CONVERT(DECIMAL(5,2), 60.00), N'calificado')
) AS sample(id_usuario_moodle, id_actividad_moodle, nota, estado)
INNER JOIN dbo.USUARIO AS user_record
    ON user_record.id_moodle = sample.id_usuario_moodle
INNER JOIN dbo.ACTIVIDAD AS activity_record
    ON activity_record.id_moodle = sample.id_actividad_moodle
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.CALIFICACION AS existing
    WHERE existing.id_usuario = user_record.id_usuario
      AND existing.id_actividad = activity_record.id_actividad
);

COMMIT TRANSACTION;
GO
