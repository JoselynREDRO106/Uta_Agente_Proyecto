# Diccionario de datos inicial

Esquema de prueba del Sprint 1 en SQL Server. `id_moodle` conserva el identificador de Moodle para facilitar la sincronización. Las llaves `id_*` sin sufijo externo son identidades internas de EduMonitor.

| Tabla | Columna | Tipo | Descripción |
|---|---|---|---|
| CURSO | id_curso | INT IDENTITY, PK | Identificador interno del curso. |
| CURSO | id_moodle | INT, UNIQUE | Identificador del curso en Moodle. |
| CURSO | nombre_moodle | NVARCHAR(200) | Nombre del curso según Moodle. |
| USUARIO | id_usuario | INT IDENTITY, PK | Identificador interno del usuario. |
| USUARIO | id_moodle | INT, UNIQUE | Identificador del usuario en Moodle. |
| USUARIO | nombre | NVARCHAR(150) | Nombre de visualización. |
| USUARIO | email | NVARCHAR(150) | Correo del usuario. |
| USUARIO | rol | NVARCHAR(30) | Rol que representa el usuario en EduMonitor. |
| ACTIVIDAD | id_actividad | INT IDENTITY, PK | Identificador interno de la actividad. |
| ACTIVIDAD | id_moodle | INT, UNIQUE | Identificador de la actividad en Moodle. |
| ACTIVIDAD | id_curso | INT, FK | Curso al que pertenece la actividad. |
| ACTIVIDAD | nombre | NVARCHAR(200) | Nombre de la actividad. |
| ACTIVIDAD | categoria | NVARCHAR(30) | Categoría: `Evaluaciones`, `Tareas` o `AP` (Actividades Prácticas). |
| CALIFICACION | id_calificacion | INT IDENTITY, PK | Identificador interno del registro de calificación. |
| CALIFICACION | id_usuario | INT, FK | Usuario al que corresponde la calificación. |
| CALIFICACION | id_actividad | INT, FK | Actividad calificada o pendiente. |
| CALIFICACION | nota | DECIMAL(5,2), NULL | Nota obtenida; cero es un valor calificado válido. |
| CALIFICACION | estado | NVARCHAR(30) | `calificado`, `sin_entrega` o `pendiente`; los dos últimos no llevan nota. |
| MAPEO_TEMATICO | id_mapeo | INT IDENTITY, PK | Identificador interno de la asociación temática. |
| MAPEO_TEMATICO | id_actividad | INT, FK | Actividad asociada con el contenido mínimo. |
| MAPEO_TEMATICO | contenido_minimo | NVARCHAR(150) | Contenido mínimo relacionado con la actividad. |

El esquema agrega `id_moodle`, `categoria` y `estado` al diagrama entidad-relación inicial para permitir sincronización y distinguir los estados de calificación requeridos. Las categorías usan el peso indicado en la planificación: Evaluaciones 40 %, Tareas 30 % y Actividades Prácticas 30 %; el peso se configura en el libro de Moodle, no como columna del esquema inicial.

## Ampliación del Sprint 2

Las tablas nuevas no duplican `CURSO` ni `ACTIVIDAD`. Se relacionan por el identificador de Moodle. `ALERT_RULE`, `ALERT_STATE`, `ACADEMIC_REPORT` y `ACADEMIC_RECOMMENDATION` están en `EduMonitorAnalysis`. `AUDIT_LOG` está en `EduMonitor`. El detalle de columnas, los diagramas de clases y de secuencia y el modelo consolidado están en [Modelos_Sprint2_Manobanda.md](Modelos_Sprint2_Manobanda.md).

| Tabla | Base | Clave | Uso |
|---|---|---|---|
| ALERT_RULE | EduMonitorAnalysis | id_moodle_curso + tipo | Umbral y si la regla está activa. Tipos: `Descenso`, `Atraso`, `BajaFinalizacion`. |
| ALERT_STATE | EduMonitorAnalysis | id_moodle_curso + id_estudiante + tipo | Estado pedagógico: `Nueva`, `Revisada` o `Descartada`. |
| ACADEMIC_REPORT | EduMonitorAnalysis | id | Informe generado, con período, fechas, aviso de datos incompletos y contenido en JSON. |
| ACADEMIC_RECOMMENDATION | EduMonitorAnalysis | id | Propuesta con promedio, umbral, justificación y estado `Propuesta`, `Aceptada`, `Ajustada` o `Descartada`. |
| AUDIT_LOG | EduMonitor | id | Usuario, rol, fecha, tipo y resultado. No guarda tokens, contraseñas ni el texto de la pregunta al agente. |
