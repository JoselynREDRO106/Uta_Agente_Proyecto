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
  const current = result.courseId === courseId ? result : { loading: true, error: '', data: null }

  useEffect(() => {
    let active = true
    getGroupAnalysis(courseId)
      .then((data) => {
        if (active) setResult({ courseId, loading: false, error: '', data })
      })
      .catch((error) => {
        if (active) setResult({ courseId, loading: false, error: error.message, data: null })
      })
    return () => { active = false }
  }, [courseId])

  return (
    <section className="content-card feature-panel">
      <div className="card-heading">
        <div>
          <h2>Análisis grupal</h2>
          <p className="muted">Promedios normalizados a 10; el umbral es 6/10 y la muestra mínima, 7 calificaciones.</p>
        </div>
        {current.data && <span className="count-pill">Fuente: {current.data.fuente}</span>}
      </div>
      {current.loading && <p className="feature-message">Calculando análisis…</p>}
      {current.error && <p className="course-message course-message--error" role="alert">{current.error}</p>}
      {current.data && (
        <>
          <p className="feature-meta">Última sincronización: {formatUtcDate(current.data.ultimaSincronizacionUtc)}</p>
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
          <div className="trend-list">
            <h3>Tendencia por fecha</h3>
            {current.data.tendencia.length === 0
              ? <p className="muted">Moodle no proporcionó fechas de calificación.</p>
              : current.data.tendencia.map((point) => (
                <p key={point.fecha}>
                  <strong>{point.fecha}</strong>
                  {' — '}
                  {point.datosInsuficientes
                    ? `Muestra insuficiente (${point.numCalificaciones} calificaciones)`
                    : `${Number(point.promedioCurso).toFixed(2)} / 10`}
                </p>
              ))}
          </div>
        </>
      )}
    </section>
  )
}

export default GroupAnalysisPanel
