import { useCallback, useEffect, useState } from 'react'
import { getCourseIndicators, getCourses, getCurrentUser, getHealth, syncMoodleCourse } from '../services/api.js'
import { Link } from 'react-router-dom'

function Cursos() {
  const isAdministrator = getCurrentUser()?.rol === 'Administrador técnico'
  const isTeacher = getCurrentUser()?.rol === 'Docente'
  const [health, setHealth] = useState({ status: 'loading', message: 'Comprobando conexión…' })
  const [courses, setCourses] = useState([])
  const [indicatorResult, setIndicatorResult] = useState(null)
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

  const courseIdsKey = JSON.stringify(courses.map((course) => String(course.id)))
  const courseIndicators = indicatorResult?.key === courseIdsKey ? indicatorResult.data : {}

  useEffect(() => {
    if (!isTeacher || coursesLoading) return undefined

    let active = true
    Promise.all(courses.map(async (course) => {
      try {
        const indicators = await getCourseIndicators(course.id)
        return [String(course.id), { indicators, error: '' }]
      } catch (error) {
        return [String(course.id), { indicators: null, error: error.message }]
      }
    })).then((entries) => {
      if (active) setIndicatorResult({ key: courseIdsKey, data: Object.fromEntries(entries) })
    })

    return () => {
      active = false
    }
  }, [courses, coursesLoading, courseIdsKey, isTeacher])

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
          <h1>{isTeacher ? 'Panel docente' : 'Mis cursos'}</h1>
          <p className="muted">
            {isTeacher
              ? 'Resumen de rendimiento de tus cursos asignados.'
              : 'Cursos sincronizados desde Moodle y guardados en EduMonitor.'}
          </p>
        </div>
        {isAdministrator && <form className="course-sync-form" onSubmit={handleSync}>
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
        </form>}
      </div>

      <div className={`connection-banner connection-banner--${health.status}`} role="status">
        <span className="connection-banner__dot" />
        <span className="connection-banner__label">Middleware API</span>
        <span>{health.message}</span>
      </div>
      {coursesError && <p className="course-message course-message--error" role="alert">{coursesError}</p>}

      {isTeacher ? (
        <section aria-label="Resumen de cursos asignados" className="teacher-course-grid">
          {coursesLoading && <p className="course-message" role="status">Cargando tus cursos…</p>}
          {!coursesLoading && courses.map((course) => {
            const result = courseIndicators[String(course.id)]
            const indicators = result?.indicators
            const lastSync = indicators?.ultimaSincronizacionUtc
              ? new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' })
                .format(new Date(indicators.ultimaSincronizacionUtc))
              : 'Sin sincronización registrada'

            return (
              <article className="teacher-course-card" key={course.id}>
                <div className="teacher-course-card__heading">
                  <span className="course-icon course-icon--blue" aria-hidden="true">▦</span>
                  <span className="count-pill">Curso {course.id}</span>
                </div>
                <h2><Link to={`/cursos/${course.id}`}>{course.name}</Link></h2>
                <p className="muted">Última sincronización: {lastSync}</p>
                {result?.error && (
                  <p className="course-message course-message--error" role="alert">
                    No se pudieron cargar los indicadores: {result.error}
                  </p>
                )}
                <div aria-busy={!result} className="teacher-course-metrics">
                  <div>
                    <span>Estudiantes</span>
                    <strong>{indicators?.estudiantes ?? (result ? 'Sin datos' : '…')}</strong>
                  </div>
                  <div>
                    <span>Promedio general</span>
                    <strong>{indicators?.promedio == null ? (result ? 'Sin datos' : '…') : `${Number(indicators.promedio).toFixed(1)} / 100`}</strong>
                  </div>
                  <div>
                    <span>Progreso</span>
                    <strong>{indicators?.progresoPorcentaje == null ? (result ? 'Sin datos' : '…') : `${indicators.progresoPorcentaje}%`}</strong>
                  </div>
                </div>
                <Link className="button button--secondary teacher-course-card__action" to={`/cursos/${course.id}`}>
                  Abrir seguimiento del curso
                </Link>
              </article>
            )
          })}
          {!coursesLoading && !coursesError && courses.length === 0 && (
            <p className="course-message" role="status">No tienes cursos asignados. Solicita acceso al administrador técnico.</p>
          )}
        </section>
      ) : <section aria-labelledby="courses-title" className="content-card">
        <div className="card-heading">
          <div>
            <h2 id="courses-title">Asignaturas activas</h2>
            <p className="muted">Listado guardado en la base de datos de EduMonitor</p>
          </div>
          <span className="count-pill">{courses.length} cursos</span>
        </div>
        {syncMessage && <p className="course-message course-message--success" role="status">{syncMessage}</p>}
        <div className="table-scroll">
          <table>
            <thead>
              <tr><th>Asignatura</th><th>ID en Moodle</th><th /></tr>
            </thead>
            <tbody>
              {coursesLoading && (
                <tr><td colSpan="3">Cargando cursos…</td></tr>
              )}
              {!coursesLoading && courses.map((course) => (
                <tr key={course.id}>
                  <td>
                    <Link className="course-cell" to={`/cursos/${course.id}`}>
                      <span className="course-icon course-icon--blue" aria-hidden="true">▦</span>
                      <span><strong>{course.name}</strong></span>
                    </Link>
                  </td>
                  <td>{course.id}</td>
                  <td><Link aria-label={`Consultar ${course.name}`} className="icon-link" to={`/cursos/${course.id}`}>↗</Link></td>
                </tr>
              ))}
              {!coursesLoading && !coursesError && courses.length === 0 && (
                <tr><td colSpan="3">No hay cursos sincronizados. Ingresa arriba el ID de un curso de Moodle.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>}

      <p className="page-footnote">
        {isTeacher
          ? 'Solo se muestran los cursos que el administrador técnico asignó a tu cuenta.'
          : 'Los cursos disponibles provienen de Moodle y están limitados a los permisos de tu cuenta.'}
      </p>
    </>
  )
}

export default Cursos
