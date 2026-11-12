# Contexto de trabajo — Sprint 2 (Frontend)

Documento de contexto para continuar el desarrollo del Sprint 2 de EduMonitor (AcademIA). Resume el alcance, el flujo de trabajo, el estado actual y cómo ejecutar el sistema en local.

## 1. Proyecto y rol

- **Proyecto:** EduMonitor — sistema de seguimiento del rendimiento académico integrado con Moodle.
- **Metodología:** Scrum, 3 sprints. El Sprint 1 está cerrado ([Informe_Sprint1_AcademIA.md](./Informe_Sprint1_AcademIA.md)).
- **Sprint 2:** planificado como «Especificación y diseño» ([Planificacion_Sprint2_AcademIA.md](./Planificacion_Sprint2_AcademIA.md)). El equipo decidió además implementar el código, probarlo e informar los resultados.
- **Integrante:** Joselyn Redroban — Frontend React, UX/UI, mockups M-07 a M-14 y consumo de endpoints.
- **Stack:** .NET 10 (Clean Architecture, microservicios), React + Vite, SQL Server, RabbitMQ, Moodle 4.5, agente de IA (FastAPI/LangGraph + Ollama).

## 2. Fases de trabajo

1. **Fase 1 — Codificación:** implementar en React las interfaces y el consumo de API de cada historia.
2. **Fase 2 — Pruebas funcionales:** ejecutar y documentar los casos CP-xx.x definidos en la planificación (sección 11).
3. **Fase 3 — Informe del Sprint 2:** misma estructura y formalidad que el informe del Sprint 1, con resultados, diagramas, métricas y pruebas del Sprint 2.

### Flujo Git (obligatorio por historia)

1. `git checkout develop; git pull` y crear la rama: `git checkout -b feature/joselyn-hu0X-<nombre>`.
2. Programar con commits pequeños (`feat:`, `fix:`, `docs:`). **Sin co-author de Copilot.**
3. Verificar en local (`npm run lint`, `npm run build`, `dotnet build` y prueba manual).
4. Push y Pull Request hacia `develop`.
5. Con el PR hecho, pasar a las pruebas de esa historia y luego a la siguiente.

## 3. Historias de usuario del Sprint 2

| HU | Historia | Mockup | Responsable del código | Estado |
|---|---|---|---|---|
| HU-08 | Seguimiento individual de un estudiante | M-07 | Joselyn (frontend y endpoint mínimo) | Implementada en rama; pendiente verificación y PR |
| HU-09 | Reglas y alertas académicas | M-08, M-09 | Pendiente | Sin implementar (sin backend) |
| HU-10 | Análisis grupal del curso | M-10 | Diego | Fusionada en `develop` (PR #13) |
| HU-11 | Consulta al agente de IA | M-11 | Diego | Fusionada en `develop` (PR #13) |
| HU-12 | Recomendaciones pedagógicas | M-12 | Diego | Fusionada en `develop` (PR #13) |
| HU-13 | Informe del curso | M-13 | Pendiente | Sin implementar |
| HU-14 | Registro de auditoría | M-14 | Diego | Fusionada en `develop` (PR #13) |

Mockups en [docs/mockups/](./mockups).

## 4. Estado de HU-08 (rama `feature/joselyn-hu08-seguimiento-individual`)

**Backend**

- Analysis: `GET /api/analysis/courses/{cursoId}/students/{estudianteId}/tracking` (403 sin acceso al curso; 404 si el estudiante no está matriculado).
- Middleware: `GET /api/cursos/{cursoId}/estudiantes/{estudianteId}/seguimiento` (valida el curso y reenvía a Analysis; 503 si Analysis no está configurado).
- DTO `SeguimientoEstudianteDto`: notas (con escala mínima y máxima), actividades y última sincronización.
- Estados de actividad: `SinEntrega`, `PendienteCalificar`, `Completada`, `NoDisponible`.
- Una nota ausente se devuelve como `null` (nunca 0).

**Frontend**

- `frontend/src/pages/FichaEstudiante.jsx` (vista M-07), ruta `/cursos/:courseId/estudiantes/:studentId`.
- `getStudentTracking` en `frontend/src/services/api.js`.
- El nombre del estudiante en `DashboardCurso.jsx` enlaza a la ficha.

**Criterios cubiertos:** CA-08.1 (notas por ítem con escala), CA-08.2 (estados sin mezclar), CA-08.3 («sin dato», no cero), CA-08.4 (403 con mensaje claro).

## 5. Revisión del PR #13 (Diego)

Revisado antes de fusionar: `dotnet build` sin errores ni advertencias, `npm ci`/`lint`/`build` correctos, 6 pruebas del agente (pytest) aprobadas, controladores con `[Authorize]` y `Forbid()`, y clave del agente comparada con `hmac.compare_digest` sin secretos reales en el repositorio.

## 6. Cómo ejecutar el sistema en local

### Requisitos

Git, .NET 10 SDK, Node.js 20.19+ o 22.12+, SQL Server Express (`localhost\SQLEXPRESS`), Docker Desktop (RabbitMQ y, opcionalmente, Moodle/agente de IA) y `sqlcmd`.

### Orden de arranque

1. **Base de datos** (una sola vez; los scripts son idempotentes):

   ```powershell
   foreach ($f in '001_schema','002_seed','003_moodle_integration','005_academic_integration','006_academic_analysis','007_sprint2_audit','008_analysis_recommendations') {
     sqlcmd -S 'localhost\SQLEXPRESS' -E -C -f 65001 -i "database\$f.sql"
   }
   sqlcmd -S 'localhost\SQLEXPRESS' -E -C -f 65001 -i database\004_identity.sql
   ```

2. **RabbitMQ y Moodle (Docker):** copiar `.env.example` a `.env` y cambiar las contraseñas.

   ```powershell
   docker compose up -d rabbitmq
   docker compose --env-file .env -f infra/moodle/docker-compose.yml up -d --build
   ```

   Moodle queda en `http://localhost:8080`. Si ya tienes Moodle instalado localmente, usa ese. Sigue [moodle_setup.md](./moodle_setup.md) para cursos, usuarios, calificaciones y el token de Web Services.

3. **Variables compartidas** (en cada terminal que arranque un servicio; las claves deben ser las mismas en todos):

   ```powershell
   $env:Authentication__Jwt__SigningKey = '<clave aleatoria de 48 bytes en base64>'
   $env:MoodleIntegration__ApiKey = '<clave aleatoria de 32 bytes en base64>'
   $env:RabbitMq__HostName = 'localhost'; $env:RabbitMq__Port = '5672'
   $env:RabbitMq__UserName = '<RABBITMQ_DEFAULT_USER>'; $env:RabbitMq__Password = '<RABBITMQ_DEFAULT_PASS>'
   ```

4. **Servicios .NET** (una terminal cada uno, desde la raíz del repositorio):

   | Servicio | Puerto | Comando |
   |---|---|---|
   | Identity | 5020 | `dotnet run --project backend\Uta.Identity.WebApi\Uta.Identity.WebApi.csproj --launch-profile http` |
   | Integración Moodle | 5010 | `dotnet run --project backend\Uta.MoodleIntegration.WebApi\Uta.MoodleIntegration.WebApi.csproj --launch-profile http` |
   | Analysis | 5030 | `dotnet run --project backend\Uta.AcademicAnalysis.WebApi\Uta.AcademicAnalysis.WebApi.csproj --launch-profile http` |
   | Middleware (gateway) | 5000 | `dotnet run --project backend\Uta.Middleware.WebApi\Uta.Middleware.WebApi.csproj --launch-profile http` |

   Identity necesita además `Authentication__BootstrapKey` (solo para el alta del primer administrador) y `ConnectionStrings__Identity` (ver el README). Middleware necesita `MoodleIntegration__BaseUrl=http://localhost:5010` y `AcademicAnalysis__BaseUrl=http://localhost:5030`.

5. **Frontend:**

   ```powershell
   Set-Location frontend
   Copy-Item .env.example .env   # VITE_API_URL=http://localhost:5000 y VITE_IDENTITY_API_URL=http://localhost:5020
   npm ci
   npm run dev
   ```

   Abrir `http://localhost:5173`.

6. **Datos de prueba:** inicia sesión como administrador, configura Moodle en `/admin/moodle` (URL y token), sincroniza un curso y crea un docente con ese curso asignado (`POST http://localhost:5020/api/auth/usuarios`). Espera a que el outbox publique el evento para que Analysis tenga la proyección del curso.

### Verificación rápida

```powershell
Invoke-RestMethod http://localhost:5000/health
Invoke-RestMethod http://localhost:5000/health/database
```

## 7. Comandos de validación

```powershell
dotnet build backend\Uta.AcademicPerformance.Middleware.slnx
Set-Location frontend; npm run lint; npm run build
```
