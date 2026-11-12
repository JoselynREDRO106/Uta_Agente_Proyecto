# EduMonitor AI

Sistema de seguimiento y análisis del rendimiento académico integrado con Moodle. El backend ofrece autenticación JWT, sincronización y consultas académicas protegidas por rol/curso. La instancia de Moodle y SQL Server son necesarias para validar el flujo completo.

## Requisitos

- Git
- .NET 10 SDK
- Node.js 20.19+ o 22.12+, con npm
- SQL Server Express para probar la persistencia local

## Estructura

```text
backend/       Solución .NET: API, aplicación, dominio e infraestructura
frontend/      Interfaz React + Vite
database/      Scripts de SQL Server (entrega del responsable DBA)
infra/         Entorno Moodle (entrega del responsable DevOps/Moodle)
agent/         Servicio local del agente FastAPI/LangGraph y sus pruebas
docs/          Planificación, diagramas y mockups
docker-compose.yml
.env.example
```

## Configuración local

No subas `.env`, tokens ni contraseñas al repositorio. `.env` se reserva para los servicios Docker opcionales; el backend local usa `appsettings.Development.json` y variables de entorno de PowerShell.

## Base de datos del middleware (SQL Server)

Con SQL Server Express iniciado, la configuración de desarrollo usa autenticación integrada de Windows contra `localhost\SQLEXPRESS` y la base `EduMonitor`. Crea el esquema y los datos de prueba desde PowerShell en la raíz:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\001_schema.sql
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\002_seed.sql
```

`-E` usa la identidad de Windows con la que se ejecuta la terminal y la API. `-C` confía en el certificado local de SQL Server. La configuración está en `backend/Uta.Middleware.WebApi/appsettings.Development.json`; cambia el servidor si tu instancia tiene otro nombre. El endpoint `http://localhost:5000/health/database` verifica la conexión. Los scripts son idempotentes e incorporan tablas de matrícula, cuentas del sistema, asignación docente-curso, escalas y bitácora. Consulta [database/README.md](database/README.md).

Para usar SQL Server sin Docker, no ejecutes `docker compose` para SQL Server. La instancia local debe estar iniciada y el usuario de Windows debe tener permiso para conectarse y crear/usar la base `EduMonitor`.

## Conexión con Moodle

Moodle se consume mediante Web Services REST; el proyecto no se conecta directamente a la base de datos de Moodle. La migración gradual separa Identidad (`EduMonitorIdentity`), Integración Moodle (`MoodleIntegration`) y Análisis Académico (`EduMonitorAnalysis`). Integración guarda la configuración Moodle y es dueña de los snapshots de cursos, matrículas, actividades y calificaciones; publica cambios mediante un outbox transaccional y RabbitMQ. Análisis mantiene su propia proyección de lectura e ignora eventos duplicados. El frontend conserva las rutas del middleware como gateway. Las tres bases lógicas pueden compartir SQL Server local, que sigue siendo un punto único de falla.

Si ya tienes Moodle instalado localmente, inicia Apache/MariaDB con tu instalación habitual. No necesitas Docker para Moodle.

El repositorio también incluye un entorno opcional de Moodle 4.5.15 en Docker, descrito en [docs/moodle_setup.md](docs/moodle_setup.md). Úsalo solo si quieres crear un Moodle de laboratorio desde cero.

Para usar ese entorno opcional, con Docker Desktop iniciado y las contraseñas locales cambiadas en `.env`:

```powershell
docker compose --env-file .env -f infra/moodle/docker-compose.yml up -d --build
docker compose --env-file .env -f infra/moodle/docker-compose.yml ps
```

Abre `http://localhost:8080`. El primer arranque instala Moodle con las variables de `.env`; espera a que termine antes de entrar. La base de Moodle y `moodledata` se conservan en volúmenes. La guía [docs/moodle_setup.md](docs/moodle_setup.md) explica las tareas manuales para cursos, usuarios, calificaciones y Web Services REST. Nunca guardes el token real en Git.

## Backend .NET

Compila la solución:

```powershell
dotnet build backend/Uta.AcademicPerformance.Middleware.slnx
```

Inicia la API:

```powershell
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$jwtBytes = New-Object byte[] 48
$bootstrapBytes = New-Object byte[] 32
$rng.GetBytes($jwtBytes)
$rng.GetBytes($bootstrapBytes)
$env:Authentication__Jwt__SigningKey = [Convert]::ToBase64String($jwtBytes)
$env:Authentication__BootstrapKey = [Convert]::ToBase64String($bootstrapBytes)
$rng.Dispose()
dotnet run --project backend/Uta.Middleware.WebApi/Uta.Middleware.WebApi.csproj --launch-profile http
```

Las dos variables son obligatorias/secretas y deben generarse nuevamente para un entorno propio; no las guardes en el repositorio. La clave de firma debe mantenerse estable entre reinicios para que los tokens emitidos sigan siendo válidos. `Authentication__BootstrapKey` habilita una sola creación inicial de administrador; retírala del entorno después del alta.

Comprueba el endpoint de salud en otra terminal:

```powershell
Invoke-RestMethod http://localhost:5000/health
```

La respuesta indica `status: ok` y el servicio `EduMonitor.Middleware`. El middleware habilita CORS para el origen local del frontend (`http://localhost:5173`).

#### Crear las cuentas iniciales (ruta heredada)

El endpoint de autenticación del middleware se conserva solo para compatibilidad con instalaciones anteriores. En una instalación nueva usa el servicio independiente de Identidad descrito abajo y evita crear cuentas por ambas rutas.

```powershell
$bootstrapHeaders = @{ 'X-Bootstrap-Key' = $env:Authentication__BootstrapKey }
$bootstrapBody = @{ email = 'admin@uta.edu.ec'; password = 'USA_UNA_CLAVE_SEGURA_DE_12_CARACTERES_O_MAS' } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5000/api/auth/bootstrap-admin -Headers $bootstrapHeaders -ContentType 'application/json' -Body $bootstrapBody
Remove-Item Env:Authentication__BootstrapKey
```

Inicia sesión desde `/login`. El administrador puede crear docentes y asignarles los IDs Moodle de cursos ya sincronizados mediante `POST /api/auth/usuarios` con `Authorization: Bearer <token>` y un cuerpo como:

```json
{
  "nombre": "Docente de prueba",
  "email": "docente@uta.edu.ec",
  "password": "USA_UNA_CLAVE_SEGURA_DE_12_CARACTERES_O_MAS",
  "rol": "Docente",
  "cursosMoodle": [1001]
}
```

El bootstrap devuelve conflicto una vez que ya existe un administrador. La API exige un JWT en todos los endpoints salvo login, bootstrap inicial, salud y OpenAPI de desarrollo. El endpoint de sincronización Moodle requiere el rol `Administrador técnico`; un docente solo puede consultar cursos asignados. El cierre de sesión elimina el token del almacenamiento de sesión del navegador.

### Servicio independiente de Identidad

El incremento de Identidad usa el proceso `Uta.Identity.WebApi` y la base lógica `EduMonitorIdentity`. Ejecuta el script después de crear `EduMonitor` con `001_schema.sql`; copia las cuentas y asignaciones sin borrar las tablas originales:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -f 65001 -i database\004_identity.sql
```

Inicia Identidad en `http://localhost:5020` con la misma clave JWT que utiliza el middleware académico. También configura una clave temporal de bootstrap y una cadena de conexión propia:

```powershell
$env:Authentication__Jwt__SigningKey = 'MISMA_CLAVE_ALEATORIA_DE_48_BYTES_QUE_USA_MIDDLEWARE'
$env:Authentication__BootstrapKey = 'CLAVE_TEMPORAL_ALEATORIA'
$env:ConnectionStrings__Identity = 'Server=localhost\SQLEXPRESS;Database=EduMonitorIdentity;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True'
dotnet run --project backend\Uta.Identity.WebApi\Uta.Identity.WebApi.csproj --launch-profile http
```

Si la cuenta administrador ya fue migrada desde `EduMonitor`, no es necesario volver a ejecutar el bootstrap. Para una base Identity vacía, usa `POST http://localhost:5020/api/auth/bootstrap-admin` con el encabezado `X-Bootstrap-Key` y retira la clave después del alta. El endpoint `POST /api/auth/usuarios` de Identidad crea docentes, roles y asignaciones por ID de curso Moodle.

Configura `VITE_IDENTITY_API_URL=http://localhost:5020` en `frontend/.env`; el inicio de sesión usará Identidad y las demás operaciones seguirán usando `VITE_API_URL`. Ambos procesos deben validar JWT con la misma clave e issuer/audience. Identidad firma en el token la lista de cursos asignados, así el middleware valida el acceso localmente, sin llamar a Identidad por petición. Los cambios de asignación entran en vigor al emitir un nuevo token; mientras tanto, los JWT anteriores conservan su lista hasta expirar. Las rutas de autenticación del middleware se mantienen temporalmente para compatibilidad; no las uses para crear cuentas nuevas después de habilitar Identidad.

### Servicio independiente de Integración Moodle

Ejecuta una vez el script de esquema de la base de configuración, además de los scripts de `EduMonitor`:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -i database\003_moodle_integration.sql
```

La API de Integración corre en `http://localhost:5010` y la API académica conserva `http://localhost:5000`. Para desarrollo, configura una clave JWT estable compartida por ambas APIs y una clave interna aleatoria de al menos 32 bytes. No publiques estos valores ni la carpeta local de claves Data Protection; conserva una copia protegida de esta última, necesaria para descifrar el token guardado.

```powershell
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$jwtBytes = New-Object byte[] 48
$integrationBytes = New-Object byte[] 32
$rng.GetBytes($jwtBytes)
$rng.GetBytes($integrationBytes)
$env:Authentication__Jwt__SigningKey = [Convert]::ToBase64String($jwtBytes)
$env:MoodleIntegration__ApiKey = [Convert]::ToBase64String($integrationBytes)
$env:MoodleIntegration__DataProtectionKeysPath = Join-Path $env:LOCALAPPDATA 'EduMonitor\MoodleIntegration\keys'
$rng.Dispose()

# Inicia Integración en un proceso hijo que hereda las claves del proceso actual.
$integrationProcess = Start-Process dotnet -ArgumentList @(
  'run', '--project', 'backend\Uta.MoodleIntegration.WebApi\Uta.MoodleIntegration.WebApi.csproj',
  '--launch-profile', 'http'
) -WorkingDirectory $PWD -PassThru
```

Apunta la API académica al servicio de Integración y reiníciala si ya estaba ejecutándose:

```powershell
$env:MoodleIntegration__BaseUrl = 'http://localhost:5010'
dotnet run --project backend\Uta.Middleware.WebApi\Uta.Middleware.WebApi.csproj --launch-profile http
```

Abre `/admin/moodle` con una cuenta `Administrador técnico`, guarda la URL y el token y usa **Verificar conexión**. El campo del token es write-only: vacío conserva el valor previamente guardado. Para Moodle remoto se exige HTTPS; HTTP solo se permite para una instancia local. El token debe habilitar `core_webservice_get_site_info`, `core_course_get_courses`, `core_enrol_get_enrolled_users`, `core_course_get_contents` y `gradereport_user_get_grade_items`.

El middleware delega las sincronizaciones al proceso de Integración cuando se configuran `MoodleIntegration__BaseUrl` y `MoodleIntegration__ApiKey`. El puente usa una clave compartida de al menos 32 bytes; los endpoints internos la validan y no aceptan llamadas anónimas. Mientras no se configure, se conserva temporalmente el cliente Moodle legado para compatibilidad.

### Análisis Académico y RabbitMQ

Ejecuta `database\005_academic_integration.sql` para crear las tablas de snapshots y outbox y migrar los cursos locales existentes a Integración sin borrar `EduMonitor`. Luego ejecuta `database\006_academic_analysis.sql` para crear `EduMonitorAnalysis`. Los eventos existentes quedan pendientes en el outbox hasta que RabbitMQ esté disponible.

El `docker-compose.yml` incluye RabbitMQ con volúmenes persistentes y puertos ligados a `localhost`. Para iniciarlo, copia `.env.example` a `.env`, reemplaza las contraseñas de ejemplo y ejecuta:

```powershell
docker compose up -d rabbitmq
docker compose ps rabbitmq
```

Inicia `Uta.AcademicAnalysis.WebApi` en `localhost:5030` y configura en los procesos Integration y Analysis las mismas variables `RabbitMq__HostName=localhost`, `RabbitMq__Port=5672`, `RabbitMq__UserName` y `RabbitMq__Password`. El publisher crea un exchange y una cola durables, confirma la publicación y reintenta el mensaje fallido por ID; el consumidor aplica cada evento de forma idempotente. Middleware usa `AcademicAnalysis__BaseUrl` para sus consultas; las rutas existentes de cursos, estudiantes e indicadores delegan en Análisis para los JWT con asignaciones administradas.

Si RabbitMQ no está configurado, la sincronización todavía guarda datos en Integración y el evento permanece en el outbox, pero la proyección de Análisis no se actualiza. Una sesión JWT válida permite consultas durante una caída de Identidad; no permite iniciar nuevas sesiones mientras Identidad esté fuera de servicio.

### Análisis grupal, agente y recomendaciones (Sprint 2)

Aplica `database\007_sprint2_audit.sql` y `database\008_analysis_recommendations.sql` después de crear las bases. El dashboard del curso usa estas rutas protegidas por JWT y autorización del curso:

- `GET /api/cursos/{cursoId}/analisis-grupal?umbral=6&minimoCalificaciones=7`
- `POST /api/cursos/{cursoId}/consultar`
- `GET /api/cursos/{cursoId}/recomendaciones`
- `POST /api/cursos/{cursoId}/recomendaciones/generar`
- `PATCH /api/cursos/{cursoId}/recomendaciones/{id}/estado`
- `GET /api/auditoria` (solo Administrador técnico, con filtros y paginación)

El análisis normaliza las notas a escala 0–10; no aplica el umbral cuando hay menos de siete calificaciones. Para ejecutar agente y Ollama en Docker, genera un secreto para `ACADEMIC_AGENT_API_KEY` en `.env`, luego inicia ambos servicios:

```powershell
docker compose --profile ai up -d --build ollama academic-agent
docker compose exec ollama ollama pull llama3.2
```

Configura Middleware con `AcademicAgent__BaseUrl=http://localhost:8010` y la misma `AcademicAgent__ApiKey` definida en `.env`. El agente envía únicamente la pregunta al modelo local para clasificar la intención; construye la respuesta con indicadores agregados obtenidos por la API .NET. No envía nombres, correos ni calificaciones individuales.

## Frontend React

En una terminal nueva:

```powershell
Set-Location frontend
Copy-Item .env.example .env
npm ci
npm run dev
```

Abre `http://localhost:5173`. La página de cursos consulta `/health` y muestra el estado de conexión. También puedes verificar el paquete de producción con `npm run build` y las reglas estáticas con `npm run lint`.

Rutas de la aplicación:

- `/login`
- `/cursos`
- `/cursos/:id`
- `/estudiantes/:id` (el seguimiento individual detallado permanece fuera del Sprint 1)

Las rutas académicas requieren una sesión válida. El listado de cursos y la vista de curso consultan la API; el dashboard presenta indicadores normalizados a escala 0–100 y estados de matrícula. No se asigna cero a una calificación ausente.

## Trabajo en equipo

- `main` contiene la versión estable y `develop` integra el trabajo del sprint.
- Cada integrante trabaja en una rama `feature/...`; los cambios llegan a `develop` mediante Pull Request revisado.
- Antes de abrir un Pull Request, actualiza la rama con `git fetch origin` y `git merge origin/develop`.
- Usa prefijos de commit como `feat:`, `fix:`, `docs:` y `chore:`.
- No subas secretos, `.env`, `node_modules/`, `bin/` ni `obj/`.

## Alcance del Sprint 1

Este informe conserva el alcance histórico del Sprint 1. En la rama de avance actual ya se integraron el consumo de Moodle, la persistencia SQL, la consulta de estudiantes y el panel básico de indicadores; la implementación del agente queda para un sprint posterior.

Consulta la [planificación del Sprint 1](docs/Planificacion_Sprint_1.pdf), el [diccionario de datos](docs/diccionario_datos.md) y los [diagramas](docs/diagramas/).
