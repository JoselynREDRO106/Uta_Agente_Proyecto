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
agent/         Esqueleto documental del agente, cuya implementación queda para un sprint posterior
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

Moodle se consume mediante Web Services REST; el proyecto no se conecta directamente a la base de datos de Moodle. En el primer paso de la migración gradual, `Uta.MoodleIntegration.WebApi` es un proceso independiente que guarda la URL y el token cifrado en la base lógica `MoodleIntegration`, permite verificar la conexión y ofrece un puente interno para las operaciones Moodle del middleware. El token nunca se devuelve a la interfaz. La persistencia académica de cursos, matrículas, actividades y calificaciones continúa temporalmente en `EduMonitor`; su traslado al servicio dueño de Integración es una etapa posterior.

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

#### Crear las cuentas iniciales

Después de ejecutar los scripts de SQL e iniciar la API, crea el primer administrador usando una contraseña propia de al menos 12 caracteres:

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

El middleware delega las nuevas sincronizaciones al proceso de Integración cuando `MoodleIntegration__BaseUrl` está configurado. Si se omite, conserva temporalmente el cliente Moodle legado para no romper instalaciones existentes. El puente usa `MoodleIntegration__ApiKey`; los endpoints internos no aceptan llamadas anónimas. La instancia SQL Server local aloja dos bases lógicas y sigue siendo un punto único de falla.

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
