# Base de datos inicial

El esquema de prueba usa SQL Server 2022 y crea la base `EduMonitor`.

## SQL Server local (sin Docker)

El backend de desarrollo se conecta por autenticación integrada de Windows a `localhost\SQLEXPRESS`, usando la base `EduMonitor`. Inicia el servicio SQL Server Express y, desde PowerShell en la raíz del repositorio, ejecuta:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\001_schema.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\002_seed.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\003_moodle_integration.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -f 65001 -i database\004_identity.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\005_academic_integration.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\006_academic_analysis.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\007_sprint2_audit.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\008_analysis_recommendations.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\009_analysis_alerts.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\010_analysis_reports.sql
```

`-E` usa la identidad de Windows de la terminal y `-C` confía en el certificado local. La identidad debe tener permisos para crear la base y sus tablas la primera vez.

`004_identity.sql` crea `EduMonitorIdentity` y copia cuentas/asignaciones desde `EduMonitor` sin eliminar las tablas de origen. Se puede ejecutar de nuevo sin duplicar registros. Si hay una colisión de ID/correo, el script se detiene para evitar asociar cuentas incorrectamente. Tras el corte, inicia `Uta.Identity.WebApi`; el middleware conserva temporalmente las tablas antiguas para compatibilidad con JWT y rutas heredadas.

`005_academic_integration.sql` crea las tablas de propiedad académica y outbox en `MoodleIntegration`, e importa allí una copia inicial de los cursos, matrículas, actividades y calificaciones existentes. `006_academic_analysis.sql` crea la base lógica independiente `EduMonitorAnalysis`; los snapshots existentes se proyectan allí mediante RabbitMQ y el outbox. Ninguno de estos pasos elimina los datos de `EduMonitor`.

`007_sprint2_audit.sql` agrega la bitácora administrativa a `EduMonitor`. `008_analysis_recommendations.sql` agrega el almacenamiento de propuestas a `EduMonitorAnalysis`. Ejecuta ambos después de `006_academic_analysis.sql`; los scripts son idempotentes y no eliminan datos.

Para activar la sincronización entre servicios, configura en Middleware `MoodleIntegration__BaseUrl` y `MoodleIntegration__ApiKey` (la misma clave local configurada como `MoodleIntegration__ApiKey` en Integration, con al menos 32 bytes). Configura las mismas credenciales RabbitMQ bajo `RabbitMq__*` en Integration y Analysis. No guardes estas claves en archivos versionados.

## SQL Server en Docker (alternativa)

Si prefieres usar Docker, desde la raíz copia `.env.example` a `.env`, cambia las contraseñas de ejemplo y levanta el servicio `sqlserver`:

```powershell
docker compose up -d sqlserver
docker compose ps
docker exec -it edumonitor-sql bash -lc '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/001_schema.sql'
docker exec -it edumonitor-sql bash -lc '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/002_seed.sql'
```

`001_schema.sql` crea las tablas si no existen y puede ejecutarse más de una vez. `002_seed.sql` inserta por identificador Moodle y también puede repetirse sin duplicar registros. Usa únicamente información ficticia de `example.test`.

## Esquema

| Tabla | Propósito |
|---|---|
| `CURSO` | Cursos Moodle, con identificador externo único. |
| `USUARIO` | Docentes o estudiantes Moodle. |
| `MATRICULA` | Relación entre cursos y estudiantes obtenida de Moodle. |
| `ACTIVIDAD` | Actividades y categoría (`Evaluaciones`, `Tareas` o `AP`). |
| `CALIFICACION` | Nota y estado por estudiante/actividad; los estados son `calificado`, `sin_entrega` y `pendiente`. Una nota cero es válida cuando está calificada. |
| `MAPEO_TEMATICO` | Asociación de actividades con contenidos mínimos. |
| `USUARIO_SISTEMA` | Cuentas del middleware con hash de contraseña PBKDF2 y rol. |
| `ASIGNACION_DOCENTE_CURSO` | Cursos Moodle que puede consultar cada docente del sistema. |
| `SINCRONIZACION_LOG` | Fecha UTC, resultado y resumen de errores por sincronización. |
| `AUDIT_LOG` | Usuario, rol, fecha, tipo y resultado de acciones auditables; no almacena preguntas, tokens ni calificaciones individuales. |
| `ALERT_RULE` | En `EduMonitorAnalysis`. Umbral y estado activo de `Descenso`, `Atraso` o `BajaFinalizacion` por curso. |
| `ALERT_STATE` | En `EduMonitorAnalysis`. Estado pedagógico de una alerta: `Nueva`, `Revisada` o `Descartada`. |
| `ACADEMIC_REPORT` | En `EduMonitorAnalysis`. Informe del curso, con período, fecha de los datos y contenido JSON. |
| `ACADEMIC_RECOMMENDATION` | En `EduMonitorAnalysis`. Propuesta justificada por indicadores y su estado pedagógico. |

El modelo consolidado, el diagrama entidad-relación y los diagramas de secuencia están en [docs/Modelos_Sprint2_Manobanda.md](../docs/Modelos_Sprint2_Manobanda.md).

Con la extracción de Identidad, `USUARIO_SISTEMA` y `ASIGNACION_DOCENTE_CURSO` se copian también a la base lógica `EduMonitorIdentity`; el middleware heredado mantiene sus tablas de origen durante la migración gradual. Los nuevos JWT de Identidad llevan la lista de cursos asignados y el middleware la valida localmente.

`ACTIVIDAD.nota_maxima` conserva la escala informada por Moodle; las escalas desconocidas permanecen nulas y no se usan para fabricar promedios. El seed crea un curso, tres estudiantes matriculados, tres actividades y ejemplos de los tres estados. La nota de calificación puede ser cero; los estados sin entrega y pendiente se guardan sin nota. Las cuentas del sistema no tienen contraseñas de demostración: el primer administrador se crea con el bootstrap de una sola vez descrito en el README raíz.
