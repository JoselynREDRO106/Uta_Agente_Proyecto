# Guía para levantar EduMonitor / AcademIA en local

Todos los comandos son para PowerShell y se ejecutan desde la raíz del repositorio, salvo que se indique otra cosa. No incluyas claves reales en archivos versionados.

## 1. Requisitos

| Herramienta | Uso |
|---|---|
| .NET SDK 10 | Servicios backend |
| Node.js 20+ y npm | Frontend (React + Vite) |
| SQL Server Express (`localhost\SQLEXPRESS`) y `sqlcmd` | Bases de datos |
| Docker Desktop | RabbitMQ (y, opcionalmente, agente + Ollama) |
| Moodle local (`http://localhost:8080`) | Origen de datos académicos |
| Python 3.11+ y Ollama (opcional) | Agente de IA (HU-12) |

## 2. Bases de datos

El servicio SQL Server Express debe estar iniciado. Los scripts son idempotentes: se pueden ejecutar de nuevo sin duplicar ni borrar datos. **Cada base se crea con su script**, por eso hay que ejecutarlos todos en orden.

| Script | Crea / modifica |
|---|---|
| `001_schema.sql`, `002_seed.sql` | Base `EduMonitor` (tablas y datos ficticios) |
| `003_moodle_integration.sql` | Tablas de sincronización en `EduMonitor` |
| `004_identity.sql` | Base `EduMonitorIdentity` (cuentas y asignaciones) |
| `005_academic_integration.sql` | Base `MoodleIntegration` (datos académicos y outbox) |
| `006_academic_analysis.sql` | Base `EduMonitorAnalysis` (snapshots) |
| `007_sprint2_audit.sql` | `AUDIT_LOG` en `EduMonitor` |
| `008_analysis_recommendations.sql` | Recomendaciones en `EduMonitorAnalysis` |
| `009_analysis_alerts.sql` | Alertas y reglas (HU-09) en `EduMonitorAnalysis` |
| `010_analysis_reports.sql` | Informes (HU-13) en `EduMonitorAnalysis` |

```powershell
$s = 'localhost\SQLEXPRESS'
sqlcmd -S $s -E -C -i database\001_schema.sql
sqlcmd -S $s -E -C -i database\002_seed.sql
sqlcmd -S $s -E -C -i database\003_moodle_integration.sql
sqlcmd -S $s -E -C -f 65001 -i database\004_identity.sql
sqlcmd -S $s -E -C -i database\005_academic_integration.sql
sqlcmd -S $s -E -C -i database\006_academic_analysis.sql
sqlcmd -S $s -E -C -i database\007_sprint2_audit.sql
sqlcmd -S $s -E -C -i database\008_analysis_recommendations.sql
sqlcmd -S $s -E -C -i database\009_analysis_alerts.sql
sqlcmd -S $s -E -C -i database\010_analysis_reports.sql
```

Verifica que existan las cuatro bases:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -Q "SELECT name FROM sys.databases WHERE name IN ('EduMonitor','EduMonitorIdentity','MoodleIntegration','EduMonitorAnalysis')"
```

Deben aparecer 4 filas. Si falta alguna, revisa el error del script correspondiente (el orden importa: `004` y `005` dependen de `001`–`003`; `008`–`010` dependen de `006`). Si `sqlcmd` falla por permisos, abre la terminal con un usuario de Windows que pueda crear bases.

## 3. RabbitMQ

1. Copia `.env.example` a `.env` (si aún no existe) y cambia las contraseñas de ejemplo.
2. Inicia el servicio:

```powershell
docker compose up -d rabbitmq
docker compose ps
```

Panel de administración: http://localhost:15672 (usuario y contraseña de `RABBITMQ_DEFAULT_USER` / `RABBITMQ_DEFAULT_PASS`).

## 4. Backend (4 terminales)

Abre cuatro terminales PowerShell en la raíz del repositorio. En **todas** define primero las mismas variables comunes (ajusta los valores; la clave JWT y la de Moodle deben tener al menos 32 caracteres y ser idénticas entre servicios):

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:Authentication__Jwt__SigningKey = 'CLAVE_JWT_ALEATORIA_DE_AL_MENOS_32_CARACTERES'
$env:RabbitMq__HostName = 'localhost'
$env:RabbitMq__UserName = 'edumonitor'
$env:RabbitMq__Password = 'LA_CONTRASEÑA_DE_RABBITMQ_DEL_.env'
$env:MoodleIntegration__ApiKey = 'CLAVE_MOODLE_ALEATORIA_DE_AL_MENOS_32_CARACTERES'
```

### Terminal 1 — Identity (5020)

```powershell
$env:Authentication__BootstrapKey = 'CLAVE_BOOTSTRAP_DE_UN_SOLO_USO'
dotnet run --project backend\Uta.Identity.WebApi --urls http://localhost:5020 --no-launch-profile
```

### Terminal 2 — Integración Moodle (5010)

Configura además el acceso a Moodle (URL y token de un servicio web habilitado):

```powershell
$env:MoodleSettings__BaseUrl = 'http://localhost:8080'
$env:MoodleSettings__Token = 'TOKEN_DE_MOODLE'
dotnet run --project backend\Uta.MoodleIntegration.WebApi --urls http://localhost:5010 --no-launch-profile
```

### Terminal 3 — Analysis (5030)

```powershell
dotnet run --project backend\Uta.AcademicAnalysis.WebApi --urls http://localhost:5030 --no-launch-profile
```

### Terminal 4 — Middleware (5000)

```powershell
$env:MoodleIntegration__BaseUrl = 'http://localhost:5010'
$env:AcademicAnalysis__BaseUrl = 'http://localhost:5030'
dotnet run --project backend\Uta.Middleware.WebApi --urls http://localhost:5000 --no-launch-profile
```

Para habilitar el agente de IA, añade en esta terminal `AcademicAgent__BaseUrl = 'http://localhost:8010'` y `AcademicAgent__ApiKey` con la misma clave del agente.

Orden recomendado: Identity, Integración, Analysis y, al final, Middleware.

### Primer administrador

Si la tabla de cuentas está vacía, crea el administrador una sola vez (usa la `BootstrapKey` de la Terminal 1):

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5020/api/auth/bootstrap-admin `
  -Headers @{ 'X-Bootstrap-Key' = 'CLAVE_BOOTSTRAP_DE_UN_SOLO_USO' } `
  -ContentType 'application/json' `
  -Body '{"email":"admin@example.test","password":"CONTRASEÑA_SEGURA"}'
```

Si ya existe un administrador (por ejemplo `admin@uta.edu.ec`), omite este paso y usa esa cuenta.

## 5. Agente de IA (opcional, HU-12)

```powershell
cd agent
python -m venv .venv
.\.venv\Scripts\Activate.ps1
python -m pip install -r requirements.txt
$env:AGENT_API_KEY = 'CLAVE_ALEATORIA_DE_AL_MENOS_32_BYTES'
$env:OLLAMA_BASE_URL = 'http://127.0.0.1:11434'
$env:OLLAMA_ALLOWED_HOST = 'ollama'
$env:OLLAMA_MODEL = 'llama3.2'
uvicorn app:app --host 127.0.0.1 --port 8010
```

Ollama debe estar iniciado y con el modelo descargado (`ollama pull llama3.2`). Sin el agente, el resto del sistema funciona.

## 6. Frontend (5173)

`frontend\.env` debe contener:

```
VITE_API_URL=http://localhost:5000
VITE_IDENTITY_API_URL=http://localhost:5020
VITE_MOODLE_INTEGRATION_URL=http://localhost:5010
```

```powershell
cd frontend
npm install
npm run dev
```

Abre http://localhost:5173 e inicia sesión con el administrador. Sincroniza un curso desde el panel y espera a que el snapshot se proyecte en Analysis; luego se habilitan Dashboard, Ficha del estudiante, Alertas, Informes y Auditoría.

## 7. Verificación rápida

| Qué | Cómo |
|---|---|
| Servicios arriba | Cada terminal muestra `Now listening on: http://localhost:<puerto>` |
| Bases | Consulta de la sección 2 devuelve 4 filas |
| RabbitMQ | http://localhost:15672 accesible |
| Compilación | `dotnet build` y `npm run build` sin errores |

## 8. Problemas frecuentes

| Síntoma | Causa y solución |
|---|---|
| `Cannot open database ...` | Falta ejecutar el script de esa base (sección 2). |
| Error de clave JWT o API key | Las claves deben tener ≥ 32 caracteres y coincidir entre servicios. |
| Login funciona pero no hay cursos | Falta la sincronización con Moodle, o el token/URL de Moodle es incorrecto. |
| Dashboard vacío tras sincronizar | RabbitMQ apagado o credenciales distintas entre Integración y Analysis. |
| Auditoría vacía | Falta `007_sprint2_audit.sql`. |
| Alertas o Informes devuelven error 500 | Faltan `009_analysis_alerts.sql` o `010_analysis_reports.sql`. |
| Caracteres raros en `004_identity.sql` | Ejecutarlo con `-f 65001`. |
| Puerto ocupado | Cierra el proceso previo o cambia `--urls` y las variables `*__BaseUrl`/`VITE_*` correspondientes. |
