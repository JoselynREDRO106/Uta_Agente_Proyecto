import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import MetricCard from '../components/MetricCard.jsx'
import { getCourseIndicators, getCourses, getGroupAnalysis } from '../services/api.js'
import { buildActivityAlerts, loadGradeColumns, readActivityAlertStore } from '../services/activityAlerts.js'

function formatValue(value, suffix = '') {
  return value == null ? 'Sin datos' : `${value}${suffix}`
}

function DashboardCurso() {
  const { id } = useParams()
  const [courseResult, setCourseResult] = useState(null)
  const courseLoading = courseResult?.id !== id
  const course = courseLoading ? null : courseResult.course
  const indicators = courseLoading ? null : courseResult.indicators
  const lowestActivity = courseLoading ? null : courseResult.lowestActivity
  const newAlerts = courseLoading ? null : courseResult.newAlerts
  const error = courseLoading ? '' : courseResult.error

  useEffect(() => {
    let active = true
    Promise.all([
      getCourses(),
      getCourseIndicators(id),
      getGroupAnalysis(id, { umbral: 6, minimoCalificaciones: 1 }).catch(() => null),
      loadGradeColumns(id).catch(() => null),
    ])
      .then(([courses, result, analysis, gradebook]) => {
        if (!active) return
        const lowest = (analysis?.actividades ?? []).find((item) => item.promedio != null) ?? null
        const nuevas = gradebook
          ? buildActivityAlerts(gradebook.columns, gradebook.rows, readActivityAlertStore(id))
            .filter((item) => item.estado === 'Nueva').length
          : null
        setCourseResult({
          id,
          course: courses.find((item) => String(item.id) === id) ?? null,
          indicators: result,
          lowestActivity: lowest,
          newAlerts: nuevas,
          error: '',
        })
      })
      .catch((requestError) => {
        if (active) {
          setCourseResult({
            id,
            course: null,
            indicators: null,
            lowestActivity: null,
            newAlerts: null,
            error: requestError.message,
          })
        }
      })

    return () => { active = false }
  }, [id])

  const average = indicators?.promedio
  const progress = indicators?.progresoPorcentaje
  const lastSync = indicators?.ultimaSincronizacionUtc
    ? new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeStyle: 'short' })
      .format(new Date(indicators.ultimaSincronizacionUtc))
    : 'Sin sincronización registrada'

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">RESUMEN DEL CURSO</span>
          <h1>{course?.name ?? (courseLoading ? 'Cargando curso…' : 'Curso')}</h1>
          <p className="muted">Fuente: Moodle · Última sincronización: {lastSync}</p>
        </div>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}

      <section aria-label="Indicadores del curso" className="metric-grid">
        <MetricCard
          detail={average == null ? 'No existen calificaciones con escala disponible' : 'Escala de 0 a 10'}
          label="Promedio general"
          tone="blue"
          value={formatValue(average == null ? null : (Number(average) / 10).toFixed(2), average == null ? '' : ' / 10')}
        />
        <MetricCard
          detail="Matrículas sincronizadas"
          label="Estudiantes"
          tone="violet"
          value={formatValue(indicators?.estudiantes)}
        />
        <MetricCard
          detail="Notas calificadas sobre el total de registros posibles"
          label="Progreso"
          tone="green"
          value={formatValue(progress, progress == null ? '' : '%')}
        />
      </section>

      {indicators && !indicators.datosDisponibles && (
        <p className="course-message course-message--warning" role="status">
          No hay datos suficientes con escala de calificación para calcular el promedio. No se asigna cero a notas ausentes.
        </p>
      )}
      {indicators?.estadoSincronizacion === 'Incompleta' && (
        <p className="course-message course-message--warning" role="status">
          La última sincronización quedó incompleta. Algunos indicadores pueden no estar disponibles.
        </p>
      )}

      <section className="content-card indicator-distribution">
        <div className="card-heading">
          <div>
            <h2>Distribución de calificaciones</h2>
            <p className="muted">Nota normalizada sobre 10. Cada número cuenta calificaciones registradas, no estudiantes.</p>
          </div>
          <span className="count-pill">{indicators?.actividadesEvaluadas ?? 0} / {indicators?.actividadesTotales ?? 0} actividades evaluadas</span>
        </div>
        <div className="distribution-grid">
          {(indicators?.distribucion ?? []).map((band) => (
            <div className="distribution-item" key={band.rango}>
              <span>{({
                '0–59': '0–5.9',
                '60–69': '6–6.9',
                '70–84': '7–8.4',
                '85–100': '8.5–10',
                '>100': '>10',
              })[band.rango] ?? band.rango}</span>
              <strong>{band.cantidad}</strong>
              <small>calificaciones</small>
            </div>
          ))}
          {!indicators?.distribucion?.length && <p className="muted">Sin datos para mostrar.</p>}
        </div>
      </section>

      <section aria-label="Lectura del curso" className="course-reading">
        <article className="content-card">
          <h2>Actividad más baja</h2>
          {lowestActivity ? (
            <>
              <strong>{lowestActivity.nombre}</strong>
              <p>{Number(lowestActivity.promedio).toFixed(2)} / 10{lowestActivity.datosInsuficientes ? ' · muestra corta' : ''}</p>
            </>
          ) : (
            <p className="muted">{courseLoading ? 'Calculando…' : 'No hay actividades con promedio.'}</p>
          )}
          <Link to="analisis">Ver análisis grupal</Link>
        </article>
        <article className="content-card">
          <h2>Alertas nuevas</h2>
          <strong>{newAlerts == null ? (courseLoading ? '…' : 'Sin datos') : newAlerts}</strong>
          <p className="muted">Notas de una actividad menores al corte, en estado Nueva.</p>
          <Link to="alertas">Ver alertas</Link>
        </article>
      </section>
    </>
  )
}

export default DashboardCurso
