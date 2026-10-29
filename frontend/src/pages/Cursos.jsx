import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getHealth } from '../services/api.js'

const courses = [
  { id: '1', name: 'Servicios Web', code: 'SW-4B', students: 28, average: '82.4', progress: 68, color: 'violet' },
  { id: '2', name: 'Bases de Datos', code: 'BD-4B', students: 25, average: '76.8', progress: 54, color: 'blue' },
  { id: '3', name: 'Ingeniería de Software', code: 'IS-4B', students: 31, average: '88.1', progress: 81, color: 'green' },
]

function Cursos() {
  const [health, setHealth] = useState({ status: 'loading', message: 'Comprobando conexión…' })

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

    return () => {
      active = false
    }
  }, [])

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">RESUMEN ACADÉMICO</span>
          <h1>Mis cursos</h1>
          <p className="muted">Monitorea el rendimiento y el avance de tus asignaturas.</p>
        </div>
        <button className="button button--primary" type="button" onClick={() => window.location.reload()}>
          <span aria-hidden="true">↻</span> Sincronizar
        </button>
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
            <p className="muted">Periodo académico actual</p>
          </div>
          <span className="count-pill">{courses.length} cursos</span>
        </div>
        <div className="table-scroll">
          <table>
            <thead>
              <tr><th>Asignatura</th><th>Estudiantes</th><th>Promedio</th><th>Finalización</th><th /></tr>
            </thead>
            <tbody>
              {courses.map((course) => (
                <tr key={course.id}>
                  <td>
                    <Link className="course-cell" to={`/cursos/${course.id}`}>
                      <span className={`course-icon course-icon--${course.color}`} aria-hidden="true">▦</span>
                      <span><strong>{course.name}</strong><small>{course.code}</small></span>
                    </Link>
                  </td>
                  <td>{course.students} <span className="muted">estudiantes</span></td>
                  <td><span className="grade-value">{course.average}</span><span className="muted"> / 100</span></td>
                  <td>
                    <div className="progress-cell"><span className="progress-track"><span style={{ width: `${course.progress}%` }} /></span><small>{course.progress}%</small></div>
                  </td>
                  <td><Link aria-label={`Ver ${course.name}`} className="icon-link" to={`/cursos/${course.id}`}>↗</Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <p className="page-footnote">Los datos de cursos mostrados son de ejemplo para la interfaz inicial.</p>
    </>
  )
}

export default Cursos
