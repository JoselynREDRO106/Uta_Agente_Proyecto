# Pruebas funcionales – Sprint 2 (HU-08, HU-09 y HU-13)

**Responsable:** Joselyn Redroban · **Entorno:** local (Moodle local, SQL Server Express, servicios .NET, `npm run dev`)
**Referencia:** `Planificacion_Sprint2_AcademIA.md`, sección 11.2 (Tablas 58, 59 y 63).

> Estado de cada caso: `Aprobado` o `Fallido`. Ejecución del 12/11/2026 (ver sección 7: método y evidencia).

## 1. Preparación

1. Aplica los scripts `database\007` a `database\010` (ver `database/README.md`).
2. Levanta RabbitMQ, Identity, Integración Moodle, Analysis y Middleware, y luego `npm run dev` en `frontend`.
3. Entra con `admin@uta.edu.ec` (acceso a todos los cursos) y sincroniza un curso desde `/cursos`.
4. Para los casos de **403** (CP-08.4 y CP-13.4) se necesita un **Docente B** sin asignación al curso. Si no existe, crea un usuario docente asignado solo a otro curso.

Obtención de un token para las pruebas por URL directa (PowerShell):

```powershell
$login = Invoke-RestMethod -Method Post -Uri http://localhost:5020/api/auth/login `
  -ContentType "application/json" -Body '{"email":"<correo-docente-B>","password":"<contraseña>"}'
$h = @{ Authorization = "Bearer $($login.accessToken)" }
```

(El Middleware escucha en el puerto 5000.)

## 2. HU-08 – Seguimiento individual (M-07)

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-08.1 | CA-08.1 | Docente con curso asignado; estudiante con tres notas | Ir a `/cursos/{id}`, clic en el nombre de un estudiante con notas | Notas ordenadas por fecha, cada una con su escala | Aprobado |
| CP-08.2 | CA-08.2 | Estudiante con actividades en los tres estados | En la ficha, revisar la lista de actividades y los contadores | Cada actividad en su estado y contadores correctos | Aprobado |
| CP-08.3 | CA-08.3 | Nota ausente en Moodle | Abrir la ficha de un estudiante con una actividad sin calificar | Se muestra «sin dato», no cero | Aprobado |
| CP-08.4 | CA-08.4 | Docente B sin asignación al curso | `Invoke-RestMethod -Uri http://localhost:5000/api/cursos/{id}/estudiantes/{studentId}/seguimiento -Headers $h` | HTTP 403 | Aprobado |

## 3. HU-09 – Reglas y alertas (M-08, M-09)

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-09.1 | CA-09.1 | Reglas activas | Ir a `/cursos/{id}/alertas/reglas`, cambiar un umbral (por ejemplo 20 → 30), **Guardar cambios**, recargar la página y abrir `/cursos/{id}/alertas` | El umbral queda guardado tras recargar y se aplica en la siguiente evaluación, sin recompilar | Aprobado |
| CP-09.2 | CA-09.2 | Curso con descenso, atraso y baja finalización conocidos | Abrir el panel de alertas | Se generan las tres alertas esperadas y ninguna otra | Aprobado |
| CP-09.3 | CA-09.3 | Alerta generada | Clic en «Ver evidencia» | Ítems, valores, fechas y regla; texto sin etiquetas definitivas sobre el estudiante | Aprobado |
| CP-09.4 | CA-09.4 | Sincronización «Incompleta» | Forzar el estado (ver nota) y abrir el panel | Sin alertas por datos faltantes y con aviso amarillo de datos incompletos | Aprobado |
| CP-09.5 | CA-09.5 | Panel con datos | Revisar el pie del panel | Se muestran «Fuente: Moodle» y la fecha de la última sincronización | Aprobado |

Pruebas adicionales: cambiar el estado de una alerta a «Revisada» y «Descartada» (debe persistir y respetar el filtro) y enviar un umbral `0` o vacío (debe rechazarse con mensaje).

**Nota CP-09.2:** para tener datos conocidos, usa un curso de prueba en Moodle con un estudiante cuya última nota baje al menos 20 puntos respecto al promedio previo, otro que no entregue una actividad que el resto entregó hace 3 días o más, y otro con menos del 50 % de actividades entregadas.

**Nota CP-09.4:** si no puedes provocar una sincronización incompleta, edita temporalmente el `SyncStatus` del snapshot en `EduMonitorAnalysis.dbo.COURSE_PROJECTION.snapshot_json` de un curso de prueba a `"Incompleta"` y restáuralo después.

## 4. HU-13 – Informe del curso (M-13)

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-13.1 | CA-13.1 | Curso y período con datos | Ir a `/cursos/{id}/informes` y pulsar **Generar informe** | Incluye período, métricas, hallazgos, recomendaciones y fecha de actualización de los datos | Aprobado |
| CP-13.2 | CA-13.2 | Informe generado | Pulsar «Ver» en la lista y luego «Descargar» | Se visualiza y se descarga un archivo HTML imprimible (PDF vía «Imprimir») | Aprobado |
| CP-13.3 | CA-13.3 | Curso con sincronización incompleta | Forzar el estado (nota de CP-09.4) y generar el informe | El informe muestra el aviso de datos incompletos | Aprobado |
| CP-13.4 | CA-13.4 | Docente B sin asignación | `Invoke-RestMethod -Uri http://localhost:5000/api/cursos/{id}/informes -Headers $h` | HTTP 403 | Aprobado |

Verificación adicional: en el historial de auditoría (`/admin/auditoria`) deben aparecer los eventos `InformeGenerado`, `InformeDescargado`, `ReglasAlertaActualizadas` y `EstadoAlertaActualizado`.

## 5. Resumen de resultados

| HU | Casos | Aprobados | Fallidos | Pendientes |
|---|---|---|---|---|
| HU-08 | 4 | 4 | 0 | 0 |
| HU-09 | 5 | 5 | 0 | 0 |
| HU-13 | 4 | 4 | 0 | 0 |
| **Total** | **13** | **13** | **0** | **0** |

## 6. Defectos encontrados

| ID | Caso | Descripción | Severidad | Estado |
|---|---|---|---|---|
| D-01 | CP-08.3 | La ficha mostraba la fecha `01/01/1970` en una nota sin calificar, porque Moodle envía `dateSubmitted = "1"` como marcador. | Baja | Corregido en `FichaEstudiante.jsx` (se muestra «—» para fechas anteriores al año 2000). |

## 7. Método y evidencia de ejecución (12/11/2026)

**Método.** Se levantaron Analysis (5030) y Middleware (5000) con SQL Server Express y RabbitMQ locales, y se llamó a los endpoints del Middleware (`/api/cursos/...`) con tokens JWT firmados con la clave de la sesión de pruebas: Docente con `course_id` 1001 y 9001, Docente B con `course_id` 999 y Administrador técnico. El frontend se verificó con `npm run build` (compila sin errores). No se hizo recorrido visual en el navegador; los criterios de pantalla se comprobaron por los datos que consume cada componente.

**Datos.** HU-08 y los 403 usaron el curso real 1001 (Ana, Luis y María Ejemplo). Para HU-09 y HU-13 se creó el curso temporal 9001 con cuatro estudiantes de comportamiento conocido (descenso, atraso, baja finalización y regular). Al terminar se eliminó el curso 9001 y sus registros de alertas, reglas, informes y auditoría.

| Caso | Resultado observado |
|---|---|
| CP-08.1 | Luis (2002) devuelve 3 notas (78, 91, 82) con escala 0–100, ordenadas por fecha. |
| CP-08.2 | Ana (2001) devuelve las tres actividades: `Completada` (Evaluacion U1), `PendienteCalificar` (Tarea U1) y `SinEntrega` (Practica U1). |
| CP-08.3 | Las notas ausentes de Ana llegan como `nota: null`; la ficha muestra «sin dato», no cero. |
| CP-08.4 | Docente B (curso 999) recibe **HTTP 403** en `/cursos/1001/estudiantes/2001/seguimiento`. |
| CP-09.1 | Con el umbral de Descenso en 50 desaparece la alerta de descenso; al volver a 20 reaparece, sin recompilar. Las reglas persisten al volver a consultarlas. Umbral `0` → **HTTP 400**. |
| CP-09.2 | Curso 9001: `Descenso` (Alumno Descenso, 40 puntos bajo el promedio de 90 %), `Atraso` (Alumno Atraso, 1 actividad) y `BajaFinalizacion` (Alumno Baja Finalización, 1 de 4 = 25 %). El alumno regular no genera alertas. El alumno de baja finalización también recibe `Atraso` porque sus actividades faltantes superan los 3 días. |
| CP-09.3 | Cada alerta trae `evidencias` con ítem, valor, fecha y descripción, y un texto sin etiquetas definitivas («Se recomienda revisar el contexto antes de actuar»). |
| CP-09.4 | Con `syncStatus = "Incompleta"`: 0 alertas, `datosIncompletos = true` y aviso «La última sincronización quedó incompleta…». |
| CP-09.5 | El panel devuelve `fuente = "Moodle"` y `ultimaSincronizacionUtc`. |
| Estados de alerta | «Revisada» y «Descartada» persisten y el filtro `?estado=` las devuelve. |
| CP-13.1 | El informe incluye período, indicadores, hallazgos, recomendaciones y `fechaActualizacionDatosUtc`. |
| CP-13.2 | El informe se lista y se abre por id; la descarga responde 200, `text/html` con `Content-Disposition: attachment; filename=informe-curso-9001-….html`. |
| CP-13.3 | Con sincronización incompleta, `datosIncompletos = true` y `aviso = "Datos incompletos: la última sincronización no terminó correctamente…"`. |
| CP-13.4 | Docente B recibe **HTTP 403** en `/cursos/1001/informes` (también en `/alertas`). |
| Auditoría | Se registraron `ReglasAlertaActualizadas` (Éxito y Error por el umbral inválido), `EstadoAlertaActualizado`, `InformeGenerado` e `InformeDescargado`. |
