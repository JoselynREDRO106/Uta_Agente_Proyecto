# Base de datos inicial

El esquema de prueba usa SQL Server 2022 y crea la base `EduMonitor`.

## SQL Server local (sin Docker)

El backend de desarrollo se conecta por autenticación integrada de Windows a `localhost\SQLEXPRESS`, usando la base `EduMonitor`. Inicia el servicio SQL Server Express y, desde PowerShell en la raíz del repositorio, ejecuta:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\001_schema.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\002_seed.sql
```

`-E` usa la identidad de Windows de la terminal y `-C` confía en el certificado local. La identidad debe tener permisos para crear la base y sus tablas la primera vez.

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
| `ACTIVIDAD` | Actividades y categoría (`Evaluaciones`, `Tareas` o `AP`). |
| `CALIFICACION` | Nota y estado por estudiante/actividad; los estados son `calificado`, `sin_entrega` y `pendiente`. Una nota cero es válida cuando está calificada. |
| `MAPEO_TEMATICO` | Asociación de actividades con contenidos mínimos. |

El seed crea un curso, tres estudiantes, tres actividades y ejemplos de los tres estados. La nota de calificación puede ser cero; los estados sin entrega y pendiente se guardan sin nota.
