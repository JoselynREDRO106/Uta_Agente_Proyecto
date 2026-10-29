# Base de datos inicial

El esquema de prueba usa SQL Server 2022 y crea la base `EduMonitor`.

## Levantar SQL Server

Desde la raíz del repositorio, copia `.env.example` a `.env`, reemplaza `MSSQL_SA_PASSWORD` por una contraseña local segura y levanta el contenedor:

```powershell
docker compose up -d sqlserver
docker compose ps
```

## Ejecutar los scripts

El contenedor monta esta carpeta en `/scripts`. Ejecuta primero el esquema y luego los datos ficticios:

```powershell
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
