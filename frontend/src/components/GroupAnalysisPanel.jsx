import { useEffect, useState } from 'react'
import { getCourseGradebook, getCourseIndicators, getCourseStudents, getGroupAnalysis, getStudentTracking } from '../services/api.js'
import { readAnalysisSettings, writeAnalysisSettings } from '../services/analysisSettings.js'
import ActivityTrendChart from './ActivityTrendChart.jsx'

function formatUtcDate(value) {
  if (!value) return 'Sin fecha'
  return new Intl.DateTimeFormat('es-EC', {
    dateStyle: 'medium',
    timeStyle: 'short',
    timeZone: 'America/Guayaquil',
  }).format(new Date(value))
}

function buildActivitySeries(analysis, order) {
  const trend = analysis?.tendencia ?? []
  if (trend.length > 0 && trend.every((point) => point.actividad)) {
    return trend.map((point, index) => ({
      key: point.itemId ?? `${point.actividad}-${index}`,
      nombre: point.actividad,
      promedio: point.promedioCurso == null ? null : Number(point.promedioCurso),
      numCalificaciones: point.numCalificaciones,
      datosInsuficientes: Boolean(point.datosInsuficientes),
      delta: point.variacion == null ? null : Number(point.variacion),
    }))
  }

  const activities = analysis?.actividades ?? []
  const byId = new Map(activities.map((item) => [item.itemId, item]))
  const seen = new Set()
  const ordered = []
  for (const entry of order) {
    const item = byId.get(entry.itemId)
    if (!item || seen.has(item.itemId)) continue
    seen.add(item.itemId)
    ordered.push(item)
  }
  for (const item of activities) {
    if (seen.has(item.itemId)) continue
    seen.add(item.itemId)
    ordered.push(item)
  }

  let previous = null
  return ordered.map((item) => {
    const promedio = item.promedio == null ? null : Number(item.promedio)
    const delta = promedio != null && previous != null ? promedio - previous : null
    if (promedio != null) previous = promedio
    return {
      key: item.itemId,
      nombre: item.nombre,
      promedio,
      numCalificaciones: item.numCalificaciones,
      datosInsuficientes: Boolean(item.datosInsuficientes),
      delta,
    }
  })
}

async function loadActivityOrder(courseId) {
  try {
    const book = await getCourseGradebook(courseId)
    if (Array.isArray(book?.actividades) && book.actividades.length > 0) {
      return book.actividades.map((activity) => ({ itemId: activity.itemId }))
    }
  } catch {
    // El libro único entra cuando se reinician Análisis y Middleware.
  }

  const page = await getCourseStudents(courseId, { page: 1, pageSize: 1 })
  const student = page?.items?.[0]
  if (!student) return []
  const tracking = await getStudentTracking(courseId, student.moodleId)
  return (tracking.actividades ?? []).map((activity) => ({ itemId: activity.itemId }))
}

function GroupAnalysisPanel({ courseId }) {
  const [result, setResult] = useState({ courseId: null, loading: true, error: '', data: null })
  const [umbral, setUmbral] = useState('6')
  const [minimoCalificaciones, setMinimoCalificaciones] = useState('1')
  const [filters, setFilters] = useState(null)
  const [filterError, setFilterError] = useState('')
  const current = filters
    && result.courseId === courseId
    && result.umbral === filters.umbral
    && result.minimoCalificaciones === filters.minimoCalificaciones
    ? result
    : { loading: true, error: '', data: null }

  useEffect(() => {
    let active = true
    const saved = readAnalysisSettings(courseId)
    if (saved) {
      setUmbral(String(saved.umbral))
      setMinimoCalificaciones(String(saved.minimoCalificaciones))
      setFilters(saved)
      return () => { active = false }
    }

    getCourseIndicators(courseId)
      .then((indicators) => {
        const enrolled = Math.max(1, Math.min(1000, Number(indicators?.estudiantes) || 1))
        const next = { umbral: 6, minimoCalificaciones: enrolled }
        if (!active) return
        writeAnalysisSettings(courseId, next)
        setUmbral('6')
        setMinimoCalificaciones(String(enrolled))
        setFilters(next)
      })
      .catch(() => {
        const next = { umbral: 6, minimoCalificaciones: 1 }
        if (!active) return
        setUmbral('6')
        setMinimoCalificaciones('1')
        setFilters(next)
      })
    return () => { active = false }
  }, [courseId])

  useEffect(() => {
    if (!filters) return undefined
    let active = true
    Promise.all([
      getGroupAnalysis(courseId, filters),
      loadActivityOrder(courseId).catch(() => []),
    ])
      .then(([data, order]) => {
        if (active) setResult({
          courseId,
          umbral: filters.umbral,
          minimoCalificaciones: filters.minimoCalificaciones,
          loading: false,
          error: '',
          data,
          series: buildActivitySeries(data, order),
        })
      })
      .catch((error) => {
        if (active) setResult({
          courseId,
          umbral: filters.umbral,
          minimoCalificaciones: filters.minimoCalificaciones,
          loading: false,
          error: error.message,
          data: null,
        })
      })
    return () => { active = false }
  }, [courseId, filters])

  function applyFilters(event) {
    event.preventDefault()
    const threshold = Number(umbral)
    const minimum = Number(minimoCalificaciones)
    if (!Number.isFinite(threshold) || threshold <= 0 || threshold > 10) {
      setFilterError('El umbral debe ser mayor que 0 y no superar 10.')
      return
    }
    if (!Number.isInteger(minimum) || minimum < 1 || minimum > 1000) {
      setFilterError('La muestra mínima debe ser un entero entre 1 y 1000.')
      return
    }

    setFilterError('')
    const next = { umbral: threshold, minimoCalificaciones: minimum }
    writeAnalysisSettings(courseId, next)
    setFilters(next)
  }

  return (
    <section className="content-card feature-panel">
      {current.data && (
        <div className="card-heading card-heading--tools">
          <span className="count-pill">Fuente: {current.data.fuente}</span>
        </div>
      )}
      <form className="analysis-controls" onSubmit={applyFilters}>
        <label>
          Umbral bajo (0–10)
          <input
            max="10"
            min="0.01"
            onChange={(event) => setUmbral(event.target.value)}
            required
            step="0.01"
            type="number"
            value={umbral}
          />
        </label>
        <label>
          Mínimo de calificaciones
          <input
            max="1000"
            min="1"
            onChange={(event) => setMinimoCalificaciones(event.target.value)}
            required
            step="1"
            type="number"
            value={minimoCalificaciones}
          />
        </label>
        <button className="button button--primary" type="submit">Aplicar</button>
      </form>
      {filterError && <p className="course-message course-message--error" role="alert">{filterError}</p>}
      {current.loading && <p className="feature-message">Calculando análisis…</p>}
      {current.error && <p className="course-message course-message--error" role="alert">{current.error}</p>}
      {current.data && (
        <>
          <p className="feature-meta">
            Umbral: {Number(current.data.umbralBajo).toFixed(2)}/10 · Mínimo: {current.data.minimoCalificaciones} calificaciones ·
            Última sincronización: {formatUtcDate(current.data.ultimaSincronizacionUtc)}
          </p>
          <div className="table-scroll">
            <table>
              <thead><tr><th>Actividad</th><th>Promedio / 10</th><th>Calificaciones</th><th>Resultado</th></tr></thead>
              <tbody>
                {current.data.actividades.map((activity) => (
                  <tr className={!activity.datosInsuficientes && activity.bajoUmbral ? 'analysis-row--low' : undefined} key={activity.itemId}>
                    <td>{activity.nombre}</td>
                    <td>{activity.promedio == null ? 'Sin datos' : Number(activity.promedio).toFixed(2)}</td>
                    <td>{activity.numCalificaciones}</td>
                    <td>
                      {activity.datosInsuficientes
                        ? 'Datos insuficientes'
                        : activity.bajoUmbral ? 'Bajo el umbral' : 'Suficiente'}
                    </td>
                  </tr>
                ))}
                {current.data.actividades.length === 0 && <tr><td colSpan="4">No hay actividades calificables.</td></tr>}
              </tbody>
            </table>
          </div>
          <section className="trend-list" aria-labelledby="group-trend-heading">
            <h3 id="group-trend-heading">Tendencia por actividad</h3>
            <p className="muted">Cada punto es una actividad, en el orden del libro de calificaciones. El número junto al punto es la diferencia con la actividad anterior.</p>
            <ActivityTrendChart points={current.series ?? []} threshold={current.data.umbralBajo} />
          </section>
        </>
      )}
    </section>
  )
}

export default GroupAnalysisPanel
