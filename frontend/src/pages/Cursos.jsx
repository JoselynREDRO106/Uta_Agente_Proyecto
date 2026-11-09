import { useCallback, useEffect, useState } from 'react'
import { getCourses, getHealth, syncMoodleCourse } from '../services/api.js'

function Cursos() {
  const [health, setHealth] = useState({ status: 'loading', message: 'Comprobando conexión…' })
  const [courses, setCourses] = useState([])
  const [coursesLoading, setCoursesLoading] = useState(true)
  const [coursesError, setCoursesError] = useState('')
  const [courseId, setCourseId] = useState('')
  const [syncing, setSyncing] = useState(false)
  const [syncMessage, setSyncMessage] = useState('')

  const loadCourses = useCallback(async () => {
    setCoursesLoading(true)
    setCoursesError('')

    try {
      setCourses(await getCourses())
    } catch (error) {
      setCoursesError(error.message)
    } finally {
      setCoursesLoading(false)
    }
  }, [])

  useEffect(() => {
    let active = true

    getHealth()
      .then((result) => {
        if (active) {
          setHealth({ status: 'online', message: `Conectado · ${result.status ?? 'OK'}` })
        }
      })
      .catch((error) => {
        if (active) {
          setHealth({ status: 'offline', message: error.message })
        }
      })

    getCourses()
      .then((result) => {
        if (active) {
          setCourses(result)
        }
      })
      .catch((error) => {
        if (active) {
          setCoursesError(error.message)
        }
      })
      .finally(() => {
        if (active) {
          setCoursesLoading(false)
        }
      })

    return () => {
      active = false
    }
  }, [loadCourses])

  async function handleSync(event) {
    event.preventDefault()
    setSyncMessage('')
    setCoursesError('')
    setSyncing(true)

    try {
      const course = await syncMoodleCourse(courseId)
      setSyncMessage(`Curso "${course.name}" sincronizado correctamente.`)
      setCourseId('')
      await loadCourses()
    } catch (error) {
      setCoursesError(error.message)
    } finally {
      setSyncing(false)
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">RESUMEN ACADÉMICO</span>
          <h1>Mis cursos</h1>
          <p className="muted">Cursos sincronizados desde Moodle y guardados en EduMonitor.</p>
        </div>
        <form className="course-sync-form" onSubmit={handleSync}>
          <label htmlFor="moodle-course-id">ID del curso en Moodle</label>
          <input
            id="moodle-course-id"
            min="1"
            onChange={(event) => setCourseId(event.target.value)}
            required
            type="number"
            value={courseId}
          />
          <button className="button button--primary" disabled={syncing} type="submit">
            {syncing ? 'Sincronizando…' : 'Sincronizar curso'}
          </button>
        </form>
      </div>

      <div className={`connection-banner connection-banner--${health.status}`} role="status">
        <span className="connection-banner__dot" />
        <span className="connection-banner__label">Middleware API</span>
        <span>{health.message}</span>
      </div>

      <section aria-labelledby="courses-title" className="content-card">
        <div className="card-heading">
          <div>
            <h2 id="courses-title">Asignaturas activas</h2>
            <p className="muted">Listado guardado en la base de datos de EduMonitor</p>
          </div>
          <span className="count-pill">{courses.length} cursos</span>
        </div>
        {coursesError && <p className="course-message course-message--error" role="alert">{coursesError}</p>}
        {syncMessage && <p className="course-message course-message--success" role="status">{syncMessage}</p>}
        <div className="table-scroll">
          <table>
            <thead>
              <tr><th>Asignatura</th><th>ID en Moodle</th></tr>
            </thead>
            <tbody>
              {coursesLoading && (
                <tr><td colSpan="2">Cargando cursos…</td></tr>
              )}
              {!coursesLoading && courses.map((course) => (
                <tr key={course.id}>
                  <td>
                    <span className="course-cell">
                      <span className="course-icon course-icon--blue" aria-hidden="true">▦</span>
                      <span><strong>{course.name}</strong></span>
                    </span>
                  </td>
                  <td>{course.id}</td>
                </tr>
              ))}
              {!coursesLoading && !coursesError && courses.length === 0 && (
                <tr><td colSpan="2">No hay cursos sincronizados. Ingresa arriba el ID de un curso de Moodle.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      <p className="page-footnote">La sincronización requiere que el backend tenga configurados el acceso a Moodle y SQL Server.</p>
    </>
  )
}

export default Cursos
