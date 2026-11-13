import { useEffect, useState } from 'react'
import { getGroupAnalysis } from '../services/api.js'

function formatUtcDate(value) {
  if (!value) return 'Sin fecha'
  return new Intl.DateTimeFormat('es-EC', {
    dateStyle: 'medium',
    timeStyle: 'short',
    timeZone: 'America/Guayaquil',
  }).format(new Date(value))
}

function GroupAnalysisPanel({ courseId }) {
  const [result, setResult] = useState({ courseId: null, loading: true, error: '', data: null })
  const [umbral, setUmbral] = useState('6')
  const [minimoCalificaciones, setMinimoCalificaciones] = useState('7')
  const [filters, setFilters] = useState({ umbral: 6, minimoCalificaciones: 7 })
  const [filterError, setFilterError] = useState('')
  const current = result.courseId === courseId
    && result.umbral === filters.umbral
    && result.minimoCalificaciones === filters.minimoCalificaciones
    ? result
    : { loading: true, error: '', data: null }

  useEffect(() => {
    let active = true
    getGroupAnalysis(courseId, filters)
      .then((data) => {
        if (active) setResult({
          courseId,
          umbral: filters.umbral,
          minimoCalificaciones: filters.minimoCalificaciones,
          loading: false,
          error: '',
          data,
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
    setFilters({ umbral: threshold, minimoCalificaciones: minimum })
  }

  return (
    <section className="content-card feature-panel">
      <div className="card-heading">
        <div>
          <h2>Análisis grupal</h2>
          <p className="muted">Promedios normalizados a 10; configura el umbral y la muestra mínima.</p>
        </div>
        {current.data && <span className="count-pill">Fuente: {current.data.fuente}</span>}
      </div>
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
                  <tr key={activity.itemId}>
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
            <h3 id="group-trend-heading">Tendencia por fecha</h3>
            {current.data.tendencia.length === 0
              ? <p className="muted">Moodle no proporcionó fechas de calificación.</p>
              : (
                <ol className="trend-chart">
                  {current.data.tendencia.map((point) => {
                    const hasAverage = !point.datosInsuficientes && point.promedioCurso != null
                    const percentage = hasAverage
                      ? Math.min(100, Math.max(0, Number(point.promedioCurso) * 10))
                      : 0
                    return (
                      <li aria-label={`${point.fecha}: ${hasAverage
                        ? `${Number(point.promedioCurso).toFixed(2)} de 10`
                        : `muestra insuficiente, ${point.numCalificaciones} calificaciones`}`}
                      key={point.fecha}>
                        <span className="trend-chart__column" aria-hidden="true">
                          <span className="trend-chart__bar" style={{ height: `${percentage}%` }} />
                        </span>
                        <strong>{point.fecha}</strong>
                        <span>{hasAverage
                          ? `${Number(point.promedioCurso).toFixed(2)} / 10`
                          : 'Datos insuficientes'}</span>
                      </li>
                    )
                  })}
                </ol>
              )}
          </section>
        </>
      )}
    </section>
  )
}

export default GroupAnalysisPanel
