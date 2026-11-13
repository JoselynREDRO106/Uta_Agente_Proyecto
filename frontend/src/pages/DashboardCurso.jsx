import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import MetricCard from '../components/MetricCard.jsx'
import { getCourseIndicators, getCourses } from '../services/api.js'

function formatValue(value, suffix = '') {
  return value == null ? 'Sin datos' : `${value}${suffix}`
}

function DashboardCurso() {
  const { id } = useParams()
  const [courseResult, setCourseResult] = useState(null)
  const courseLoading = courseResult?.id !== id
  const course = courseLoading ? null : courseResult.course
  const indicators = courseLoading ? null : courseResult.indicators
  const error = courseLoading ? '' : courseResult.error

  useEffect(() => {
    let active = true
    Promise.all([getCourses(), getCourseIndicators(id)])
      .then(([courses, result]) => {
        if (active) {
          setCourseResult({
            id,
            course: courses.find((item) => String(item.id) === id) ?? null,
            indicators: result,
            error: '',
          })
        }
      })
      .catch((requestError) => {
        if (active) {
          setCourseResult({ id, course: null, indicators: null, error: requestError.message })
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
          detail={average == null ? 'No existen calificaciones con escala disponible' : `Escala normalizada de 0 a ${indicators.notaMaxima}`}
          label="Promedio general"
          tone="blue"
          value={formatValue(average == null ? null : Number(average).toFixed(2), average == null ? '' : ' / 100')}
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
            <p className="muted">Porcentaje normalizado sobre 100; solo incluye notas calificadas.</p>
          </div>
          <span className="count-pill">{indicators?.actividadesEvaluadas ?? 0} / {indicators?.actividadesTotales ?? 0} actividades evaluadas</span>
        </div>
        <div className="distribution-grid">
          {(indicators?.distribucion ?? []).map((band) => (
            <div className="distribution-item" key={band.rango}>
              <span>{band.rango}</span>
              <strong>{band.cantidad}</strong>
              <small>calificaciones</small>
            </div>
          ))}
          {!indicators?.distribucion?.length && <p className="muted">Sin datos para mostrar.</p>}
        </div>
      </section>

      <div className="course-dashboard-shortcuts">
        <Link className="button button--secondary" to={`/cursos/${id}/estudiantes`}>Ver estudiantes</Link>
        <Link className="button button--secondary" to={`/cursos/${id}/alertas`}>Revisar alertas</Link>
        <Link className="button button--secondary" to={`/cursos/${id}/informes`}>Consultar informes</Link>
      </div>
    </>
  )
}

export default DashboardCurso
