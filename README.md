# EduMonitor AI

Sistema de seguimiento y análisis del rendimiento académico integrado con Moodle. El Sprint 1 deja preparada la base técnica del proyecto; las pantallas actuales de React usan datos de demostración y el acceso es simulado.

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

`-E` usa la identidad de Windows con la que se ejecuta la terminal y la API. `-C` confía en el certificado local de SQL Server. La configuración está en `backend/Uta.Middleware.WebApi/appsettings.Development.json`; cambia el servidor si tu instancia tiene otro nombre. El endpoint `http://localhost:5000/health/database` verifica la conexión. Consulta [database/README.md](database/README.md) para verificar las tablas y los estados de muestra.

Para usar SQL Server sin Docker, no ejecutes `docker compose` para SQL Server. La instancia local debe estar iniciada y el usuario de Windows debe tener permiso para conectarse y crear/usar la base `EduMonitor`.

## Conexión con Moodle

El backend consume Moodle mediante Web Services REST y un token; no se conecta a la base de datos de Moodle. Si ya tienes Moodle instalado localmente, inicia Apache/MariaDB con tu instalación habitual y configura `MoodleSettings__BaseUrl` con la URL que abre en tu navegador. No necesitas usar Docker para ello.

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
dotnet run --project backend/Uta.Middleware.WebApi/Uta.Middleware.WebApi.csproj --launch-profile http
```

Comprueba el endpoint de salud en otra terminal:

```powershell
Invoke-RestMethod http://localhost:5000/health
```

La respuesta indica `status: ok` y el servicio `EduMonitor.Middleware`. El middleware habilita CORS para el origen local del frontend (`http://localhost:5173`).

Para consultar los servicios Moodle desde el backend, asegúrate de que Moodle esté iniciado y define el token local para el proceso antes de ejecutar .NET:

```powershell
$env:MoodleSettings__BaseUrl = 'http://localhost:8080'
$env:MoodleSettings__Token = 'PEGA_AQUI_EL_TOKEN_LOCAL'
dotnet run --project backend/Uta.Middleware.WebApi/Uta.Middleware.WebApi.csproj --launch-profile http
```

Las rutas de sincronización consultan Moodle y guardan cursos, usuarios, actividades, calificaciones y mapeos temáticos en SQL Server. El token debe tener acceso a `core_course_get_courses`, `core_enrol_get_enrolled_users`, `core_grades_get_grades` y `core_course_get_contents`.

## Frontend React

En una terminal nueva:

```powershell
Set-Location frontend
Copy-Item .env.example .env
npm ci
npm run dev
```

Abre `http://localhost:5173`. La página de cursos consulta `/health` y muestra el estado de conexión. También puedes verificar el paquete de producción con `npm run build` y las reglas estáticas con `npm run lint`.

Rutas de demostración:

- `/login`
- `/cursos`
- `/cursos/:id`
- `/estudiantes/:id`

## Trabajo en equipo

- `main` contiene la versión estable y `develop` integra el trabajo del sprint.
- Cada integrante trabaja en una rama `feature/...`; los cambios llegan a `develop` mediante Pull Request revisado.
- Antes de abrir un Pull Request, actualiza la rama con `git fetch origin` y `git merge origin/develop`.
- Usa prefijos de commit como `feat:`, `fix:`, `docs:` y `chore:`.
- No subas secretos, `.env`, `node_modules/`, `bin/` ni `obj/`.

## Alcance del Sprint 1

Esta base incluye la solución .NET y la interfaz React, el esquema inicial de SQL Server, y un entorno Moodle de laboratorio con instrucciones para habilitar Web Services y cargar los datos de prueba. El consumo completo con persistencia, el análisis académico y la implementación del agente quedan fuera del Sprint 1.

Consulta la [planificación del Sprint 1](docs/Planificacion_Sprint_1.pdf), el [diccionario de datos](docs/diccionario_datos.md) y los [diagramas](docs/diagramas/).
