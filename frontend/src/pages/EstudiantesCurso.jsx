import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getCourseStudents } from '../services/api.js'

const pageSize = 10

function statusLabel(status) {
  return ({
    calificado: 'Con calificaciones',
    pendiente: 'Pendiente de calificar',
    sin_entrega: 'Sin entrega',
    sin_datos: 'Sin datos',
  })[status] ?? status
}

function statusTone(status) {
  return ({
    calificado: 'success',
    pendiente: 'warning',
    sin_entrega: 'danger',
    sin_datos: 'neutral',
  })[status] ?? 'neutral'
}

function EstudiantesCurso() {
  const { id } = useParams()
  const [studentResult, setStudentResult] = useState(null)
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const key = JSON.stringify([id, search, status, page])
  const studentsLoading = studentResult?.key !== key
  const students = studentsLoading ? null : studentResult.data
  const error = studentsLoading ? '' : studentResult.error

  useEffect(() => {
    let active = true
    getCourseStudents(id, { search, status, page, pageSize })
      .then((data) => {
        if (active) setStudentResult({ key, data, error: '' })
      })
      .catch((requestError) => {
        if (active) setStudentResult({ key, data: null, error: requestError.message })
      })

    return () => { active = false }
  }, [id, search, status, page, key])

  function applyFilters(event) {
    event.preventDefault()
    setPage(1)
    setSearch(searchInput)
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">SEGUIMIENTO INDIVIDUAL</span>
          <h1>Estudiantes matriculados</h1>
          <p className="muted">Busca y filtra estudiantes del curso para consultar su seguimiento individual.</p>
        </div>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}

      <section className="content-card student-list">
        <div className="card-heading">
          <div>
            <h2>Listado de estudiantes</h2>
            <p className="muted">Información basada en la última sincronización con Moodle.</p>
          </div>
          <span className="count-pill">{students?.totalItems ?? 0} estudiantes</span>
        </div>
        <form className="student-filters" onSubmit={applyFilters}>
          <label htmlFor="student-search">Buscar</label>
          <input
            id="student-search"
            onChange={(event) => setSearchInput(event.target.value)}
            placeholder="Nombre o correo"
            value={searchInput}
          />
          <label htmlFor="student-status">Estado</label>
          <select
            id="student-status"
            onChange={(event) => { setStatus(event.target.value); setPage(1) }}
            value={status}
          >
            <option value="">Todos</option>
            <option value="calificado">Con calificaciones</option>
            <option value="pendiente">Pendiente de calificar</option>
            <option value="sin_entrega">Sin entrega</option>
            <option value="sin_datos">Sin datos</option>
          </select>
          <button className="button button--secondary" type="submit">Aplicar filtros</button>
        </form>
        <div className="table-scroll">
          <table>
            <thead><tr><th>Estudiante</th><th>Promedio</th><th>Estado de datos</th></tr></thead>
            <tbody>
              {studentsLoading && <tr><td colSpan="3">Cargando información…</td></tr>}
              {!studentsLoading && students?.items?.map((student) => (
                <tr key={student.moodleId}>
                  <td>
                    <span className="student-cell">
                      <span className="student-avatar">{student.nombre.split(' ').map((part) => part[0]).slice(0, 2).join('')}</span>
                      <span>
                        <strong><Link to={`/cursos/${id}/estudiantes/${student.moodleId}`}>{student.nombre}</Link></strong>
                        <small>{student.email || 'Correo no disponible'}</small>
                      </span>
                    </span>
                  </td>
                  <td>{student.promedio == null ? 'Sin datos' : `${Number(student.promedio).toFixed(2)} / 100`}</td>
                  <td><span className={`status-pill status-pill--${statusTone(student.estado)}`}>{statusLabel(student.estado)}</span></td>
                </tr>
              ))}
              {!studentsLoading && students?.items?.length === 0 && (
                <tr><td colSpan="3">No hay estudiantes que coincidan con los filtros.</td></tr>
              )}
            </tbody>
          </table>
        </div>
        <div className="pagination">
          <span className="muted">Página {students?.page ?? page} de {students?.totalPages ?? 0}</span>
          <div>
            <button className="button button--secondary" disabled={page <= 1 || studentsLoading} onClick={() => setPage((value) => value - 1)} type="button">Anterior</button>
            <button className="button button--secondary" disabled={studentsLoading || page >= (students?.totalPages ?? 0)} onClick={() => setPage((value) => value + 1)} type="button">Siguiente</button>
          </div>
        </div>
      </section>
    </>
  )
}

export default EstudiantesCurso
