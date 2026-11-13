import { useEffect, useState } from 'react'
import {
  getAllCourseStudents,
  generateCourseRecommendations,
  getCourseRecommendations,
  updateRecommendation,
} from '../services/api.js'

function formatEvidenceDate(value) {
  return value
    ? new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeZone: 'America/Guayaquil' }).format(new Date(value))
    : 'fecha no proporcionada por Moodle'
}

function RecommendationsPanel({ courseId }) {
  const [state, setState] = useState({ courseId: null, loading: true, error: '', items: [] })
  const [students, setStudents] = useState({ courseId: null, loading: true, error: '', items: [] })
  const [busy, setBusy] = useState(false)
  const [message, setMessage] = useState('')
  const [drafts, setDrafts] = useState({})
  const [scope, setScope] = useState('course')
  const [studentId, setStudentId] = useState('')
  const current = state.courseId === courseId ? state : { loading: true, error: '', items: [] }
  const currentStudents = students.courseId === courseId
    ? students
    : { loading: true, error: '', items: [] }
  const hasSelectedStudent = currentStudents.items.some(
    (student) => String(student.moodleId) === studentId,
  )

  async function load() {
    try {
      const items = await getCourseRecommendations(courseId)
      setState({ courseId, loading: false, error: '', items })
    } catch (error) {
      setState({ courseId, loading: false, error: error.message, items: [] })
    }
  }

  useEffect(() => {
    let active = true
    getCourseRecommendations(courseId)
      .then((items) => {
        if (active) setState({ courseId, loading: false, error: '', items })
      })
      .catch((error) => {
        if (active) setState({ courseId, loading: false, error: error.message, items: [] })
      })
    return () => { active = false }
  }, [courseId])

  useEffect(() => {
    let active = true
    getAllCourseStudents(courseId)
      .then((items) => {
        if (active) setStudents({ courseId, loading: false, error: '', items })
      })
      .catch((error) => {
        if (active) setStudents({ courseId, loading: false, error: error.message, items: [] })
      })
    return () => { active = false }
  }, [courseId])

  async function generate() {
    setBusy(true)
    setMessage('')
    setState((previous) => ({ ...previous, error: '' }))
    try {
      const result = await generateCourseRecommendations(courseId, {
        estudianteId: scope === 'student' ? Number(studentId) : null,
      })
      setMessage(result.creadas
        ? `Se generaron ${result.creadas} propuestas justificadas${scope === 'student' ? ' para el estudiante' : ''}.`
        : scope === 'student'
          ? 'No hay calificaciones bajo el umbral para este estudiante.'
          : 'No hay actividades con muestra suficiente bajo el umbral.')
      await load()
    } catch (error) {
      setState((previous) => ({ ...previous, error: error.message }))
    } finally {
      setBusy(false)
    }
  }

  async function update(item, estado) {
    setBusy(true)
    setMessage('')
    setState((previous) => ({ ...previous, error: '' }))
    try {
      await updateRecommendation(
        courseId,
        item.id,
        estado,
        estado === 'Ajustada' ? drafts[item.id] : undefined,
      )
      setMessage(`La propuesta quedó ${estado.toLowerCase()}.`)
      await load()
    } catch (error) {
      setState((previous) => ({ ...previous, error: error.message }))
    } finally {
      setBusy(false)
    }
  }

  return (
    <section className="content-card feature-panel">
      <div className="card-heading">
        <div>
          <h2>Recomendaciones pedagógicas</h2>
          <p className="muted">Genera propuestas para el curso (promedio grupal) o para un estudiante con calificaciones bajo 6/10.</p>
        </div>
      </div>
      <div className="recommendation-target">
        <label>
          Alcance
          <select onChange={(event) => setScope(event.target.value)} value={scope}>
            <option value="course">Curso completo</option>
            <option value="student">Estudiante específico</option>
          </select>
        </label>
        {scope === 'student' && (
          <label>
            Estudiante
            <select
              disabled={currentStudents.loading || currentStudents.items.length === 0}
              onChange={(event) => setStudentId(event.target.value)}
              value={hasSelectedStudent ? studentId : ''}
            >
              <option value="">Selecciona un estudiante</option>
              {currentStudents.items.map((student) => (
                <option key={student.moodleId} value={student.moodleId}>{student.nombre}</option>
              ))}
            </select>
          </label>
        )}
        {scope === 'student' && currentStudents.loading && <span className="muted">Cargando estudiantes…</span>}
        {scope === 'student' && !currentStudents.loading && !currentStudents.error && currentStudents.items.length === 0 && (
          <span className="muted">No hay estudiantes disponibles en este curso.</span>
        )}
        {scope === 'student' && currentStudents.error && (
          <p className="course-message course-message--error" role="alert">{currentStudents.error}</p>
        )}
        <button
          className="button button--secondary"
          disabled={busy || current.loading || (scope === 'student' && (!hasSelectedStudent || currentStudents.loading))}
          onClick={generate}
          type="button"
        >
          {busy ? 'Procesando…' : 'Generar propuestas'}
        </button>
      </div>
      {current.loading && <p className="feature-message">Cargando recomendaciones…</p>}
      {current.error && <p className="course-message course-message--error" role="alert">{current.error}</p>}
      {message && <p className="course-message course-message--success" role="status">{message}</p>}
      {!current.loading && current.items.length === 0 && !current.error && (
        <p className="feature-message">Aún no hay propuestas para este curso.</p>
      )}
      {current.items.map((item) => (
        <article className="recommendation-item" key={item.id}>
          <div className="card-heading">
            <div>
              {item.estudiante && <p><strong>Estudiante: {item.estudiante}</strong></p>}
              <h3>{item.actividad}</h3>
              <p>{item.texto}</p>
              <p className="muted">{item.justificacion}</p>
              {item.estudiante && (
                <p className="muted">Fecha de evidencia: {formatEvidenceDate(item.fechaEvidenciaUtc)}</p>
              )}
            </div>
            <span className="count-pill">{item.estado}</span>
          </div>
          {item.estado === 'Propuesta' && (
            <div className="recommendation-actions">
              <label>
                Ajustar texto
                <input
                  maxLength={1000}
                  onChange={(event) => setDrafts((value) => ({ ...value, [item.id]: event.target.value }))}
                  value={drafts[item.id] ?? item.texto}
                />
              </label>
              <button className="button button--secondary" disabled={busy} onClick={() => update(item, 'Ajustada')} type="button">Guardar ajuste</button>
              <button className="button button--secondary" disabled={busy} onClick={() => update(item, 'Aceptada')} type="button">Aceptar</button>
              <button className="button button--secondary" disabled={busy} onClick={() => update(item, 'Descartada')} type="button">Descartar</button>
            </div>
          )}
        </article>
      ))}
    </section>
  )
}

export default RecommendationsPanel
