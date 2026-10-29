# Moodle 4.5.15+ para Sprint 1

El entorno local descarga la distribución oficial Moodle 4.5.15 desde [Moodle Downloads](https://download.moodle.org/download.php/direct/stable405/moodle-4.5.15.zip), usa PHP 8.3 con Apache y almacena Moodle en MariaDB 10.11. El servicio web se publica únicamente en `http://localhost:8080`; la base de Moodle y `moodledata` se conservan en volúmenes de Docker.

Esta instancia es solo de laboratorio y usa datos ficticios. El script de arranque instala el sitio y crea la cuenta administradora con los valores locales de `.env`. Cursos, matrículas, calificaciones, Web Services y respaldos se terminan desde la interfaz de Moodle siguiendo esta guía.

## 1. Iniciar la instancia

Requiere Docker Desktop con el motor Linux/WSL2 iniciado. Desde la raíz del repositorio:

```powershell
Copy-Item .env.example .env
```

Edita `.env` y sustituye **todas** las contraseñas de ejemplo (`MSSQL_SA_PASSWORD`, `MARIADB_ROOT_PASSWORD`, `MOODLE_DB_PASSWORD` y `MOODLE_ADMIN_PASSWORD`) por valores locales fuertes y distintos. No compartas ni subas ese archivo. Inicia Moodle:

```powershell
docker compose --env-file .env -f infra/moodle/docker-compose.yml up -d --build
docker compose --env-file .env -f infra/moodle/docker-compose.yml ps
docker compose --env-file .env -f infra/moodle/docker-compose.yml logs --tail 50 moodle
```

La primera construcción descarga Moodle 4.5.15 y puede tardar varios minutos. Espera a que termine el instalador y el servicio indique estado saludable; luego abre `http://localhost:8080`. Inicia sesión con `MOODLE_ADMIN_USER` y `MOODLE_ADMIN_PASSWORD` definidos en `.env`. La interfaz inicia en inglés; cambia el idioma del sitio a español desde la administración si lo necesitas.

Si la compilación o instalación falla, consulta `docker compose ... logs moodle mariadb`. No borres volúmenes para resolverlo sin antes confirmar que no contienen datos que debas conservar.

## 2. Crear los cursos y el libro de calificaciones

Desde la página de administración del sitio:

1. Crea dos cursos de prueba: **Servicios Web** (short name `SW-4B`) y **Bases de Datos** (short name `BD-4B`).
2. Organiza cada curso con cuatro unidades (U1 a U4).
3. En el libro de calificaciones, crea categorías ponderadas **Evaluaciones 40 %**, **Tareas 30 %** y **Actividades Prácticas 30 %**. Configura la agregación ponderada para que los pesos sumen 100 %.
4. Crea aproximadamente dos actividades evaluables en cada categoría y en cada curso (por ejemplo, cuestionarios, tareas y prácticas).
5. Crea un docente y unos diez estudiantes ficticios; matricula al docente y a los estudiantes en los cursos de prueba.
6. Registra calificaciones variadas. Incluye una nota cero **calificada**, al menos una actividad sin entrega y una pendiente de calificar. No confundas cero con ausencia de nota.

Usa solo nombres y correos de prueba. Los detalles del modelo relacional y los estados de ejemplo del middleware están en [database/README.md](../database/README.md).

## 3. Activar y probar Web Services REST

Desde la administración del sitio:

1. En **Administración del sitio → Características avanzadas**, habilita los servicios web.
2. En **Administración del sitio → Servidor → Gestionar protocolos**, activa REST.
3. Crea un rol técnico de permisos mínimos que pueda utilizar el protocolo REST y consultar solo lo requerido.
4. Crea un usuario técnico, por ejemplo `ws_edumonitor`, asígnale ese rol y autorízalo para el servicio.
5. Crea un servicio externo llamado `EduMonitor` y agrega las funciones:
   - `core_webservice_get_site_info`
   - `core_course_get_courses`
   - `core_course_get_contents`
   - `core_enrol_get_enrolled_users`
   - `core_grades_get_grades`
6. Genera un token para el usuario técnico y el servicio. Guárdalo localmente; **no** lo escribas en archivos versionados ni en capturas.
7. Prueba en PowerShell; sustituye `PEGA_EL_TOKEN` solo en la terminal:

```powershell
$token = 'PEGA_EL_TOKEN'
$base = 'http://localhost:8080/webservice/rest/server.php'
Invoke-RestMethod "$base?wstoken=$token&wsfunction=core_webservice_get_site_info&moodlewsrestformat=json"
Invoke-RestMethod "$base?wstoken=$token&wsfunction=core_course_get_courses&moodlewsrestformat=json"
```

Guarda una captura sin token visible y anota la versión del sitio y las funciones efectivamente disponibles. Si una función no aparece en esta versión, documenta el resultado; no amplíes los permisos del servicio para compensarlo sin revisar el acceso necesario.

Para probar después el middleware en la terminal donde arrancará .NET:

```powershell
$env:MoodleSettings__BaseUrl = 'http://localhost:8080'
$env:MoodleSettings__Token = $token
dotnet run --project backend/Uta.Middleware.WebApi/Uta.Middleware.WebApi.csproj --launch-profile http
```

El token queda en memoria en esa sesión de PowerShell y no se guarda en Git.

## 4. Respaldar y cerrar la configuración

En cada curso, usa **Más → Reutilización del curso → Copia de seguridad** (el nombre del menú puede variar ligeramente) y conserva usuarios y calificaciones. Descarga los respaldos `.mbz` en `infra/moodle/backups/`. Comprueba que se pueden restaurar en un curso de prueba. Sube al repositorio únicamente respaldos hechos con datos ficticios.

Completa este documento con la fecha de la prueba, la versión mostrada por Moodle, el resultado de las llamadas REST, las funciones disponibles y una captura sin el token.

## Comandos útiles

Ejecutar el esquema de base de datos del middleware:

```powershell
docker compose up -d sqlserver
docker exec -it edumonitor-sql bash -lc '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/001_schema.sql'
docker exec -it edumonitor-sql bash -lc '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/002_seed.sql'
```

Detener Moodle sin borrar los datos:

```powershell
docker compose --env-file .env -f infra/moodle/docker-compose.yml down
```

No uses `down -v` salvo que quieras eliminar deliberadamente la base y los cursos de laboratorio.
