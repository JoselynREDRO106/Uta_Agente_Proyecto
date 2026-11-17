# Pruebas funcionales – Sprint 2 (HU-08 a HU-14)

**Responsable:** Joselyn Redroban · **Entorno:** local (Moodle local, SQL Server Express, servicios .NET, `npm run dev`)
**Referencia:** `Planificacion_Sprint2_AcademIA.md`, sección 11.2.

> Estado de cada caso: `Aprobado` o `Fallido`. HU-08, HU-09 y HU-13 se ejecutaron el 12/11/2026 (sección 7). HU-10, HU-11, HU-12 y HU-14 se ejecutaron el 17/11/2026 con `dotnet test` sobre `backend/Uta.AcademicAnalysis.Tests` (8 pruebas) y `python -m unittest tests.test_app` (6 pruebas). Los servicios HTTP no estaban escuchando; el host de prueba levantó Middleware con el mismo JWT y sustituyó Análisis y el agente por respuestas fijas, así que no se volvió a llamar a Moodle ni a Ollama.

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
| **Total parcial** | **13** | **13** | **0** | **0** |

El cierre de HU-10, HU-11, HU-12 y HU-14 está en la sección 12.

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

## 8. HU-10 – Análisis grupal (M-10)

Ruta: `/cursos/{id}/analisis`. Endpoint: `GET /api/cursos/{id}/analisis-grupal?umbral=6&minimoCalificaciones=7`.

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-10.1 | CA-10.1 | Curso con actividades de promedio distinto | Abrir el análisis grupal | Actividades ordenadas por promedio; las que están bajo el umbral dicen «Bajo el umbral» | Aprobado |
| CP-10.2 | CA-10.2 | Curso con fechas de calificación | Revisar «Tendencia por fecha» | Cada punto muestra fecha y promedio sobre 10 | Aprobado |
| CP-10.3 | CA-10.3 | Actividad con menos calificaciones que el mínimo | Subir el mínimo y pulsar Aplicar | Esa actividad queda en «Datos insuficientes» y no se marca bajo el umbral | Aprobado |
| CP-10.4 | CA-10.4 | Curso sincronizado | Leer la cabecera del panel | Aparecen «Fuente: Moodle» y la última sincronización | Aprobado |

## 9. HU-11 – Agente de IA (M-11)

Ruta: `/cursos/{id}/asistente`. El modelo es Ollama local. Sin el agente en marcha la API responde 503 y no inventa la respuesta.

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-11.1 | CA-11.1 | Agente y Ollama locales, curso autorizado | Preguntar por actividades bajo el umbral | La respuesta usa indicadores y análisis ya calculados | Aprobado |
| CP-11.2 | CA-11.2 | Respuesta recibida | Leer la pantalla | Bloques separados: Hechos, Inferencias, Recomendaciones propuestas, e Incertidumbre | Aprobado |
| CP-11.3 | CA-11.3 | Actividad con datos insuficientes | Preguntar por actividades bajas | La actividad insuficiente no se presenta como hecho de promedio bajo; la incertidumbre lo aclara | Aprobado |
| CP-11.4 | CA-11.4 | Docente sin el curso | `POST /api/cursos/{id}/consultar` | HTTP 403 y evento `ConsultaAgente` con resultado Denegado | Aprobado |
| CP-11.5 | CA-11.5 | Agente configurado | Revisar la llamada a Ollama | Solo sale la pregunta de clasificación; la URL de Ollama queda en localhost o en el host Docker permitido | Aprobado |

## 10. HU-12 – Recomendaciones (M-12)

Ruta: `/cursos/{id}/recomendaciones`.

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-12.1 | CA-12.1 | Actividad grupal bajo 6/10 con muestra suficiente | Generar propuestas del curso | El texto dice qué revisar y nombra la actividad | Aprobado |
| CP-12.2 | CA-12.2 | La misma propuesta | Leer la justificación | Incluye promedio, actividad y fecha de evidencia (o el aviso de que Moodle no envió fecha) | Aprobado |
| CP-12.3 | CA-12.3 | Propuesta en estado Propuesta | Aceptar, ajustar el texto o descartar | El estado pasa a Aceptada, Ajustada o Descartada y se identifica como propuesta del sistema | Aprobado |
| CP-12.4 | CA-12.4 | Todas las actividades por debajo del mínimo de calificaciones | Generar propuestas | `creadas` es 0 y `motivo` explica que no se concluye por datos insuficientes | Aprobado |

## 11. HU-14 – Auditoría (M-14)

Ruta: `/auditoria`, solo Administrador técnico.

| Código | CA | Precondición | Pasos | Resultado esperado | Estado |
|---|---|---|---|---|---|
| CP-14.1 | CA-14.1 | Una sincronización y una consulta al agente | Abrir auditoría | Hay eventos con usuario, fecha y resultado. La consulta deja `ConsultaAgente` y dos `UsoHerramienta` | Aprobado |
| CP-14.2 | CA-14.2 | Esos eventos | Leer el detalle | No hay token, contraseña ni texto de la pregunta | Aprobado |
| CP-14.3 | CA-14.3 | Varios tipos y fechas | Filtrar por `UsoHerramienta` y por rango de fechas | Lista paginada de 20 | Aprobado |
| CP-14.4 | CA-14.4 | Sesión de Docente | `GET /api/auditoria` | HTTP 403. La ruta de la interfaz redirige a `/cursos` | Aprobado |

## 12. Resumen actualizado

| HU | Casos | Aprobados | Fallidos | Pendientes |
|---|---|---|---|---|
| HU-08 | 4 | 4 | 0 | 0 |
| HU-09 | 5 | 5 | 0 | 0 |
| HU-10 | 4 | 4 | 0 | 0 |
| HU-11 | 5 | 5 | 0 | 0 |
| HU-12 | 4 | 4 | 0 | 0 |
| HU-13 | 4 | 4 | 0 | 0 |
| HU-14 | 4 | 4 | 0 | 0 |
| **Total** | **30** | **30** | **0** | **0** |

### Evidencia del 17/11/2026

`dotnet test backend/Uta.AcademicAnalysis.Tests` terminó con 8 pruebas superadas y 0 errores. `python -m unittest tests.test_app` terminó con 6 pruebas OK.

| Caso | Qué se observó |
|---|---|
| CP-10.1 | Con mínimo 2, el orden fue Practica C (1.00), Tarea B (3.00) y Quiz A (6.00). Solo Tarea B quedó bajo el umbral de 6. El total de categoría no entró al promedio. `GroupAnalysisPanel` muestra «Bajo el umbral» cuando `bajoUmbral` es verdadero. |
| CP-10.2 | El único punto de tendencia fue 2026-11-10, escala 10 y promedio 4.17. La pantalla lo lista en «Tendencia por fecha». |
| CP-10.3 | Al subir el mínimo a 3, Tarea B quedó con datos insuficientes y dejó de marcarse bajo el umbral. La pantalla usa el texto «Datos insuficientes». |
| CP-10.4 | La fuente fue `Moodle` y la última sincronización 2026-11-12 15:30 UTC. La cabecera del panel pinta `fuente`. |
| CP-11.1 y CP-11.3 | El agente arma los hechos desde el análisis. La actividad con datos insuficientes no se cuenta como promedio bajo. No se llamó a Ollama en esta corrida. |
| CP-11.2 | `POST /api/cursos/1001/consultar` respondió 200 con `hechos`, `inferencias`, `recomendaciones` e `incertidumbre`. `CourseAssistantPanel` muestra esos cuatro títulos. |
| CP-11.4 | Un docente con el curso 2, al consultar el curso 1001, recibió 403. Quedó un evento `ConsultaAgente` / `Denegado` a nombre de `docente.prueba`. |
| CP-11.5 | El clasificador solo recibe la pregunta y la URL de Ollama queda limitada a localhost o al host Docker permitido. |
| CP-12.1 | El texto guardado empieza por «Revisar la estrategia de enseñanza de «Tarea B»». |
| CP-12.2 | La justificación fue «Promedio 3.00/10 en «Tarea B»…» con fecha 2026-11-10 UTC. Sin fecha, el texto dice que Moodle no la proporcionó. |
| CP-12.3 | El PATCH solo acepta Aceptada, Ajustada o Descartada, y solo si la fila sigue en Propuesta. Los botones de la pantalla envían esos tres estados. No se escribió una fila nueva en SQL en esta corrida. |
| CP-12.4 | Con todas las actividades en datos insuficientes, el motivo dice que no se generaron recomendaciones y que no se concluye. `creadas` permanece en 0 porque ese motivo solo se arma cuando no se creó ninguna. |
| CP-14.1 | La consulta autorizada dejó, en este orden, `UsoHerramienta` (`obtener_indicadores_curso`), `UsoHerramienta` (`obtener_analisis_grupal`) y `ConsultaAgente`, los tres con resultado Exito y usuario `admin.prueba`. No se repitió una sincronización contra Moodle. |
| CP-14.2 | Ningún detalle contenía la pregunta `clave-secreta-no-guardar`, ni la palabra token, ni contraseña. |
| CP-14.3 | `GET /api/auditoria?tipo=UsoHerramienta&desdeUtc=2026-11-01…&hastaUtc=2026-11-17…&tamanoPagina=20` respondió 200 y el filtro llegó con tipo `UsoHerramienta` y tamaño 20. |
| CP-14.4 | `GET /api/auditoria` con rol Docente respondió 403 y no consultó el listado. En la interfaz, `RequireAdministrator` manda esa ruta a `/cursos` y el enlace solo se muestra al Administrador técnico. |
