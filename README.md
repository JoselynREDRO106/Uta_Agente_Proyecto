# EduMonitor AI

Sistema de seguimiento y análisis del rendimiento académico integrado con Moodle. El Sprint 1 deja preparada la base técnica del proyecto; las pantallas actuales de React usan datos de demostración y el acceso es simulado.

## Requisitos

- Git
- .NET 10 SDK
- Node.js 20.19+ o 22.12+, con npm
- Docker Desktop con Docker Compose, para levantar SQL Server

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

Desde la raíz del repositorio, copia el archivo de ejemplo. Antes de iniciar los contenedores, cambia las contraseñas de ejemplo por valores locales fuertes y únicos:

```powershell
Copy-Item .env.example .env
```

No subas `.env`, tokens ni contraseñas al repositorio. El archivo raíz `.env` lo usan Docker Compose y la configuración local del middleware.

## Base de datos del middleware (SQL Server)

```powershell
docker compose up -d sqlserver
docker compose ps
```

El servicio persiste los datos en el volumen `sqldata` y monta `database/` en `/scripts`. Ejecuta el esquema y los datos de prueba; ambos scripts se pueden volver a ejecutar:

```powershell
docker exec -it edumonitor-sql bash -lc '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/001_schema.sql'
docker exec -it edumonitor-sql bash -lc '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/002_seed.sql'
```

Consulta [database/README.md](database/README.md) para verificar las tablas y los estados de muestra.

## Moodle 4.5.15+

El entorno local descarga la distribución oficial Moodle 4.5.15 durante la construcción de la imagen y utiliza MariaDB 10.11. Con Docker Desktop iniciado y las contraseñas locales cambiadas en `.env`:

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
