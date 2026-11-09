import { useEffect, useState } from 'react'
import { getSyncHistory, syncMoodleCourse } from '../services/api.js'

function formatUtcDate(value) {
  if (!value) return 'Sin fecha'
  const utcValue = /(?:Z|[+-]\d{2}:\d{2})$/.test(value) ? value : `${value}Z`
  return new Intl.DateTimeFormat('es-EC', {
    dateStyle: 'medium',
    timeStyle: 'short',
    timeZone: 'America/Guayaquil',
  }).format(new Date(utcValue))
}

function HistorialSincronizaciones() {
  const [history, setHistory] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')
  const [retryingCourseId, setRetryingCourseId] = useState(null)

  async function loadHistory(showLoading = false) {
    if (showLoading) setLoading(true)
    try {
      setHistory(await getSyncHistory())
    } catch (loadError) {
      setError(loadError.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    let active = true
    getSyncHistory()
      .then((result) => {
        if (active) setHistory(result)
      })
      .catch((loadError) => {
        if (active) setError(loadError.message)
      })
      .finally(() => {
        if (active) setLoading(false)
      })
    return () => {
      active = false
    }
  }, [])

  async function handleRetry(courseId) {
    setRetryingCourseId(courseId)
    setError('')
    setMessage('')
    try {
      const course = await syncMoodleCourse(courseId)
      setMessage(`Se completó el reintento de "${course.name}".`)
    } catch (retryError) {
      setError(retryError.message)
    } finally {
      setRetryingCourseId(null)
      await loadHistory(true)
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ADMINISTRACIÓN</span>
          <h1>Historial de sincronizaciones</h1>
          <p className="muted">Últimas operaciones registradas. Los datos académicos guardados siguen disponibles si Moodle está fuera de línea.</p>
        </div>
        <button
          className="button button--secondary"
          disabled={loading}
          onClick={() => { setError(''); loadHistory(true) }}
          type="button"
        >
          Actualizar
        </button>
      </div>

      <section aria-labelledby="sync-history-title" className="content-card">
        <div className="card-heading">
          <div>
            <h2 id="sync-history-title">Actividad reciente</h2>
            <p className="muted">Se muestran hasta 50 operaciones, de la más reciente a la más antigua.</p>
          </div>
          <span className="count-pill">{history.length} registros</span>
        </div>
        {error && <p className="course-message course-message--error" role="alert">{error}</p>}
        {message && <p className="course-message course-message--success" role="status">{message}</p>}
        <div className="table-scroll">
          <table>
            <thead>
              <tr><th>Curso</th><th>Fecha</th><th>Estado</th><th>Detalle</th><th /></tr>
            </thead>
            <tbody>
              {loading && <tr><td colSpan="5">Cargando historial…</td></tr>}
              {!loading && history.map((entry, index) => (
                <tr key={`${entry.courseId}-${entry.completedAtUtc}-${index}`}>
                  <td><strong>{entry.courseName}</strong><br /><span className="muted">Moodle #{entry.courseId}</span></td>
                  <td>{formatUtcDate(entry.completedAtUtc)}</td>
                  <td>
                    <span className={`sync-status sync-status--${entry.status === 'Completa' ? 'complete' : 'incomplete'}`}>
                      {entry.status}
                    </span>
                  </td>
                  <td>{entry.errorSummary || 'Sin errores registrados.'}</td>
                  <td>
                    {entry.status === 'Incompleta' && (
                      <button
                        className="button button--secondary"
                        disabled={retryingCourseId !== null}
                        onClick={() => handleRetry(entry.courseId)}
                        type="button"
                      >
                        {retryingCourseId === entry.courseId ? 'Reintentando…' : 'Reintentar'}
                      </button>
                    )}
                  </td>
                </tr>
              ))}
              {!loading && history.length === 0 && !error && (
                <tr><td colSpan="5">Aún no hay operaciones de sincronización registradas.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </>
  )
}

export default HistorialSincronizaciones
