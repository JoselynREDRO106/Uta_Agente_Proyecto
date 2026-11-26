import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getHealth, syncMoodleCourse } from '../services/api.js'

function SincronizarCurso() {
  const [health, setHealth] = useState({ status: 'loading', message: 'Comprobando conexión…' })
  const [courseId, setCourseId] = useState('')
  const [syncing, setSyncing] = useState(false)
  const [error, setError] = useState('')
  const [syncedCourse, setSyncedCourse] = useState(null)

  useEffect(() => {
    let active = true
    getHealth()
      .then((result) => {
        if (active) setHealth({ status: 'online', message: `Conectado · ${result.status ?? 'OK'}` })
      })
      .catch((requestError) => {
        if (active) setHealth({ status: 'offline', message: requestError.message })
      })
    return () => { active = false }
  }, [])

  async function handleSync(event) {
    event.preventDefault()
    setError('')
    setSyncedCourse(null)
    setSyncing(true)
    try {
      const course = await syncMoodleCourse(courseId)
      setSyncedCourse(course)
      setCourseId('')
    } catch (requestError) {
      setError(requestError.message)
    } finally {
      setSyncing(false)
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ADMINISTRACIÓN</span>
          <h1>Sincronizar curso</h1>
          <p className="muted">Trae estudiantes, docentes y calificaciones de un curso de Moodle.</p>
        </div>
        <Link className="button button--secondary" to="/admin/sincronizaciones">Ver historial</Link>
      </div>

      <section className="content-card admin-panel">
        <div className={`connection-banner connection-banner--inset connection-banner--${health.status}`} role="status">
          <span className="connection-banner__dot" />
          <span className="connection-banner__label">Middleware API</span>
          <span>{health.message}</span>
        </div>
        <div className="card-heading">
          <div>
            <h2>Curso en Moodle</h2>
            <p className="muted">Usa el identificador numérico del curso, no el nombre corto.</p>
          </div>
        </div>
        <form className="feature-form admin-form" onSubmit={handleSync}>
          <label htmlFor="moodle-course-id">ID del curso</label>
          <input
            id="moodle-course-id"
            min="1"
            onChange={(event) => setCourseId(event.target.value)}
            placeholder="Por ejemplo, 42"
            required
            type="number"
            value={courseId}
          />
          <div className="feature-form__footer">
            <button className="button button--primary" disabled={syncing} type="submit">
              {syncing ? 'Sincronizando…' : 'Sincronizar curso'}
            </button>
          </div>
          {error && <p className="course-message course-message--error" role="alert">{error}</p>}
          {syncedCourse && (
            <p className="course-message course-message--success" role="status">
              Curso “{syncedCourse.name}” sincronizado.{' '}
              <Link to={`/cursos/${syncedCourse.id}`}>Abrir el curso</Link>
            </p>
          )}
        </form>
      </section>
    </>
  )
}

export default SincronizarCurso
