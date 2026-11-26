import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getCourseIndicators, getCourses, getCurrentUser } from '../services/api.js'

function Cursos() {
  const isAdministrator = getCurrentUser()?.rol === 'Administrador técnico'
  const isTeacher = getCurrentUser()?.rol === 'Docente'
  const [courses, setCourses] = useState([])
  const [indicatorResult, setIndicatorResult] = useState(null)
  const [coursesLoading, setCoursesLoading] = useState(true)
  const [coursesError, setCoursesError] = useState('')

  useEffect(() => {
    let active = true

    getCourses()
      .then((result) => {
        if (active) setCourses(result)
      })
      .catch((error) => {
        if (active) setCoursesError(error.message)
      })
      .finally(() => {
        if (active) setCoursesLoading(false)
      })

    return () => {
      active = false
    }
  }, [])

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

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">RESUMEN ACADÉMICO</span>
          <h1 id="courses-title">{isTeacher ? 'Panel docente' : 'Mis cursos'}</h1>
          <p className="muted">
            {isTeacher
              ? 'Resumen de rendimiento de tus cursos asignados.'
              : 'Cursos sincronizados desde Moodle y guardados en EduMonitor.'}
          </p>
        </div>
        {isAdministrator && (
          <div className="page-heading__actions">
            <Link className="button button--secondary" to="/admin/docentes">Docentes</Link>
            <Link className="button button--primary" to="/admin/sincronizar">Sincronizar curso</Link>
          </div>
        )}
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
                    <strong>{indicators?.promedio == null ? (result ? 'Sin datos' : '…') : `${(Number(indicators.promedio) / 10).toFixed(1)} / 10`}</strong>
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
      ) : (
        <section aria-labelledby="courses-title" className="content-card">
          <div className="card-heading card-heading--tools">
            <span className="count-pill">{courses.length} cursos</span>
          </div>
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
                  <tr>
                    <td colSpan="3">
                      No hay cursos sincronizados.{' '}
                      {isAdministrator
                        ? <Link to="/admin/sincronizar">Sincroniza un curso desde Moodle</Link>
                        : 'Pide al administrador técnico que sincronice un curso.'}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </section>
      )}

      <p className="page-footnote">
        {isTeacher
          ? 'Solo se muestran los cursos que el administrador técnico asignó a tu cuenta.'
          : 'Los cursos disponibles provienen de Moodle y están limitados a los permisos de tu cuenta.'}
      </p>
    </>
  )
}

export default Cursos
