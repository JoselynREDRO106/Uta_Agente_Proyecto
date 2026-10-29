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

Desde la raíz del repositorio, crea el archivo local de variables y ajusta una contraseña segura para SQL Server:

```powershell
Copy-Item .env.example .env
```

No subas `.env`, tokens ni contraseñas al repositorio. El archivo raíz `.env` lo usa Docker Compose; para ejecutar el backend, configura `MoodleSettings__BaseUrl` y `MoodleSettings__Token` en el entorno del proceso si vas a probar la integración con una instancia Moodle. El backend y el endpoint de salud pueden ejecutarse sin un token configurado.

## SQL Server

```powershell
docker compose up -d sqlserver
docker compose ps
```

El servicio persiste los datos en el volumen `sqldata` y monta `database/` en `/scripts` para ejecutar los scripts DDL y seed cuando estén disponibles.

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

La solución .NET y la interfaz React están inicializadas. El Sprint 1 también contempla el esquema inicial de SQL Server y una instancia de Moodle de prueba; sus scripts y configuración corresponden a sus respectivos responsables. El consumo completo de Moodle, la persistencia, el análisis académico y la implementación del agente no forman parte de esta configuración base.

Consulta la [planificación del Sprint 1](docs/Planificacion_Sprint_1.pdf) y los [diagramas](docs/diagramas/).
