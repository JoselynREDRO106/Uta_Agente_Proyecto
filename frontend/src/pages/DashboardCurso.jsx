import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import CourseAssistantPanel from '../components/CourseAssistantPanel.jsx'
import GroupAnalysisPanel from '../components/GroupAnalysisPanel.jsx'
import MetricCard from '../components/MetricCard.jsx'
import RecommendationsPanel from '../components/RecommendationsPanel.jsx'
import { getCourseIndicators, getCourseStudents, getCourses } from '../services/api.js'

const pageSize = 10

function formatValue(value, suffix = '') {
  return value == null ? 'Sin datos' : `${value}${suffix}`
}

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

function DashboardCurso() {
  const { id } = useParams()
  const [courseResult, setCourseResult] = useState(null)
  const [studentResult, setStudentResult] = useState(null)
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const courseLoading = courseResult?.id !== id
  const studentsLoading = studentResult?.key !== JSON.stringify([id, search, status, page])
  const course = courseLoading ? null : courseResult.course
  const indicators = courseLoading ? null : courseResult.indicators
  const students = studentsLoading ? null : studentResult.data
  const error = (courseLoading ? '' : courseResult.error)
    || (studentsLoading ? '' : studentResult.error)

  useEffect(() => {
    let active = true
    Promise.all([getCourses(), getCourseIndicators(id)])
      .then(([courses, result]) => {
        if (active) {
          setCourseResult({
            id,
            course: courses.find((item) => String(item.id) === id) ?? null,
            indicators: result,
            error: '',
          })
        }
      })
      .catch((requestError) => {
        if (active) {
          setCourseResult({
            id,
            course: null,
            indicators: null,
            error: requestError.message,
          })
        }
      })

    return () => { active = false }
  }, [id])

  useEffect(() => {
    let active = true
    const key = JSON.stringify([id, search, status, page])
    getCourseStudents(id, { search, status, page, pageSize })
      .then((data) => {
        if (active) setStudentResult({ key, data, error: '' })
      })
      .catch((requestError) => {
        if (active) setStudentResult({ key, data: null, error: requestError.message })
      })

    return () => { active = false }
  }, [id, search, status, page])

  function applyFilters(event) {
    event.preventDefault()
    setPage(1)
    setSearch(searchInput)
  }

  const average = indicators?.promedio
  const progress = indicators?.progresoPorcentaje
  const lastSync = indicators?.ultimaSincronizacionUtc
    ? new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeStyle: 'short' })
      .format(new Date(indicators.ultimaSincronizacionUtc))
    : 'Sin sincronización registrada'

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to="/cursos">← Volver a cursos</Link>
          <span className="eyebrow">INDICADORES DEL CURSO</span>
          <h1>{course?.name ?? (courseLoading ? 'Cargando curso…' : 'Curso')}</h1>
          <p className="muted">Fuente: Moodle · Última sincronización: {lastSync}</p>
        </div>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}

      <section aria-label="Indicadores del curso" className="metric-grid">
        <MetricCard
          detail={average == null ? 'No existen calificaciones con escala disponible' : `Escala normalizada de 0 a ${indicators.notaMaxima}`}
          label="Promedio general"
          tone="blue"
          value={formatValue(average == null ? null : Number(average).toFixed(2), average == null ? '' : ' / 100')}
        />
        <MetricCard
          detail="Matrículas sincronizadas"
          label="Estudiantes"
          tone="violet"
          value={formatValue(indicators?.estudiantes)}
        />
        <MetricCard
          detail="Notas calificadas sobre el total de registros posibles"
          label="Progreso"
          tone="green"
          value={formatValue(progress, progress == null ? '' : '%')}
        />
      </section>

      {indicators && !indicators.datosDisponibles && (
        <p className="course-message course-message--warning" role="status">
          No hay datos suficientes con escala de calificación para calcular el promedio. No se asigna cero a notas ausentes.
        </p>
      )}
      {indicators?.estadoSincronizacion === 'Incompleta' && (
        <p className="course-message course-message--warning" role="status">
          La última sincronización quedó incompleta. Algunos indicadores pueden no estar disponibles.
        </p>
      )}

      <GroupAnalysisPanel courseId={id} />
      <CourseAssistantPanel courseId={id} />
      <RecommendationsPanel courseId={id} />

      <section className="content-card indicator-distribution">
        <div className="card-heading">
          <div><h2>Distribución de calificaciones</h2><p className="muted">Porcentaje normalizado sobre 100; solo incluye notas calificadas.</p></div>
          <span className="count-pill">{indicators?.actividadesEvaluadas ?? 0} / {indicators?.actividadesTotales ?? 0} actividades evaluadas</span>
        </div>
        <div className="distribution-grid">
          {(indicators?.distribucion ?? []).map((band) => (
            <div className="distribution-item" key={band.rango}>
              <span>{band.rango}</span>
              <strong>{band.cantidad}</strong>
              <small>calificaciones</small>
            </div>
          ))}
          {!indicators?.distribucion?.length && <p className="muted">Sin datos para mostrar.</p>}
        </div>
      </section>

      <section className="content-card student-list">
        <div className="card-heading">
          <div><h2>Estudiantes matriculados</h2><p className="muted">Listado paginado con filtros basados en la sincronización.</p></div>
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
                  <td><span className="student-cell"><span className="student-avatar">{student.nombre.split(' ').map((part) => part[0]).slice(0, 2).join('')}</span><span><strong><Link to={`/cursos/${id}/estudiantes/${student.moodleId}`}>{student.nombre}</Link></strong><small>{student.email || 'Correo no disponible'}</small></span></span></td>
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

export default DashboardCurso
