import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getCourses, getStudentTracking } from '../services/api.js'

const dateFormat = new Intl.DateTimeFormat('es-EC', { dateStyle: 'short' })
const dateTimeFormat = new Intl.DateTimeFormat('es-EC', { dateStyle: 'short', timeStyle: 'short' })

const activityStates = {
  SinEntrega: { label: 'Sin entrega', tone: 'danger' },
  PendienteCalificar: { label: 'Pendiente de calificar', tone: 'warning' },
  Completada: { label: 'Completada', tone: 'success' },
  NoDisponible: { label: 'No disponible', tone: 'neutral' },
}

function formatNumber(value) {
  return Number(value).toLocaleString('es-EC', { maximumFractionDigits: 2 })
}

function formatScale(min, max) {
  return `${formatNumber(min)}–${formatNumber(max)}`
}

function errorMessage(error) {
  if (/HTTP 403/.test(error.message)) {
    return 'No tienes autorización para consultar este curso.'
  }
  if (/HTTP 404/.test(error.message)) {
    return 'No se encontró el estudiante en este curso.'
  }
  return error.message
}

function FichaEstudiante() {
  const { id: courseId, studentId } = useParams()
  const key = `${courseId}/${studentId}`
  const [result, setResult] = useState(null)
  const [courseName, setCourseName] = useState('')
  const loading = result?.key !== key
  const tracking = loading ? null : result.tracking
  const error = loading ? '' : result.error

  useEffect(() => {
    let active = true
    getStudentTracking(courseId, studentId)
      .then((data) => { if (active) setResult({ key, tracking: data, error: '' }) })
      .catch((requestError) => {
        if (active) setResult({ key, tracking: null, error: errorMessage(requestError) })
      })
    getCourses()
      .then((courses) => {
        if (active) setCourseName(courses.find((course) => String(course.id) === courseId)?.name ?? '')
      })
      .catch(() => {})

    return () => { active = false }
  }, [courseId, studentId, key])

  const notes = tracking?.notas ?? []
  const activities = tracking?.actividades ?? []
  const counts = Object.keys(activityStates).map((state) => ({
    state,
    total: activities.filter((activity) => activity.estado === state).length,
  }))
  const hasMissingNotes = notes.some((note) => note.nota == null)
  const lastSync = tracking
    ? dateTimeFormat.format(new Date(tracking.ultimaSincronizacionUtc))
    : null
  const scale = notes.length > 0
    ? formatScale(notes[0].escalaMinima, notes[0].escalaMaxima)
    : null
  const mixedScales = new Set(notes.map((note) => formatScale(note.escalaMinima, note.escalaMaxima))).size > 1

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to={`/cursos/${courseId}`}>← Volver al curso</Link>
          <span className="eyebrow">SEGUIMIENTO INDIVIDUAL</span>
          <h1>{tracking?.nombre ?? (loading ? 'Cargando estudiante…' : 'Estudiante')}</h1>
          <p className="muted">Curso: {courseName || `#${courseId}`}</p>
        </div>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}

      {tracking?.estadoSincronizacion === 'Incompleta' && (
        <p className="course-message course-message--warning" role="status">
          La última sincronización quedó incompleta. Algunos datos pueden no estar disponibles.
        </p>
      )}

      {tracking && (
        <>
          <section className="content-card">
            <div className="card-heading">
              <div>
                <h2>Evolución de notas{scale && !mixedScales ? ` (escala: ${scale})` : ''}</h2>
                {hasMissingNotes && (
                  <p className="muted">«Sin dato» indica que Moodle no registra nota; no equivale a cero.</p>
                )}
              </div>
            </div>
            <div className="table-scroll">
              <table>
                <thead><tr><th>Ítem</th><th>Fecha</th><th>Nota</th><th>Escala</th></tr></thead>
                <tbody>
                  {notes.map((note) => (
                    <tr key={note.itemId}>
                      <td>{note.item}</td>
                      <td>{note.fechaUtc && new Date(note.fechaUtc).getFullYear() > 2000 ? dateFormat.format(new Date(note.fechaUtc)) : '—'}</td>
                      <td>
                        {note.nota == null
                          ? <span className="status-pill status-pill--neutral">sin dato</span>
                          : formatNumber(note.nota)}
                      </td>
                      <td>{formatScale(note.escalaMinima, note.escalaMaxima)}</td>
                    </tr>
                  ))}
                  {notes.length === 0 && <tr><td colSpan="4">No hay ítems calificables en este curso.</td></tr>}
                </tbody>
              </table>
            </div>
          </section>

          <section className="content-card">
            <div className="card-heading"><h2>Actividades</h2></div>
            <div className="tracking-summary">
              {counts.filter((item) => item.state !== 'NoDisponible' || item.total > 0).map((item) => (
                <span className={`status-pill status-pill--${activityStates[item.state].tone}`} key={item.state}>
                  {activityStates[item.state].label}: {item.total}
                </span>
              ))}
            </div>
            <div className="table-scroll">
              <table>
                <thead><tr><th>Actividad</th><th>Estado</th></tr></thead>
                <tbody>
                  {activities.map((activity) => (
                    <tr key={activity.itemId}>
                      <td>{activity.actividad}</td>
                      <td>{activityStates[activity.estado]?.label ?? activity.estado}</td>
                    </tr>
                  ))}
                  {activities.length === 0 && <tr><td colSpan="2">No hay actividades para mostrar.</td></tr>}
                </tbody>
              </table>
            </div>
          </section>

          <p className="muted">Fuente: {tracking.fuente} · Última sincronización: {lastSync}</p>
        </>
      )}
    </>
  )
}

export default FichaEstudiante
