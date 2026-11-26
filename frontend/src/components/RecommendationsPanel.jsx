import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getAllCourseStudents,
  generateCourseRecommendations,
  getCourseRecommendations,
  updateRecommendation,
} from '../services/api.js'
import { readAnalysisSettings } from '../services/analysisSettings.js'
import {
  loadGradeColumns,
  readActivityAlertStore,
  studentGrades,
  studyPlanText,
} from '../services/activityAlerts.js'

const recommendationStates = ['Propuesta', 'Aceptada', 'Ajustada', 'Descartada']

function groupByActivity(items) {
  const groups = new Map()
  for (const item of items) {
    const name = item.actividad || 'Actividad'
    const group = groups.get(name) ?? []
    group.push(item)
    groups.set(name, group)
  }
  return [...groups]
}

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
  const [statusFilter, setStatusFilter] = useState('Propuesta')
  const [gradebook, setGradebook] = useState(null)
  const [planStudent, setPlanStudent] = useState({})
  const settings = readAnalysisSettings(courseId) ?? { umbral: 6, minimoCalificaciones: 7 }
  const alertStore = readActivityAlertStore(courseId)
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

  useEffect(() => {
    let active = true
    loadGradeColumns(courseId)
      .then((book) => {
        if (active) setGradebook(book)
      })
      .catch(() => {
        if (active) setGradebook({ columns: [], rows: [] })
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
        umbral: settings.umbral,
        minimoCalificaciones: settings.minimoCalificaciones,
      })
      setMessage(result.creadas
        ? `Se generaron ${result.creadas} propuestas justificadas${scope === 'student' ? ' para el estudiante' : ''}.`
        : result.motivo || 'No se generaron recomendaciones.')
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
      <p className="feature-meta">Umbral del análisis grupal: {Number(settings.umbral).toFixed(2)}/10</p>
      <div className="alert-counts" role="group" aria-label="Filtrar por estado">
        {recommendationStates.map((estado) => (
          <button
            className={statusFilter === estado ? 'is-selected' : undefined}
            key={estado}
            onClick={() => setStatusFilter(estado)}
            type="button"
          >
            {estado}
            <strong>{current.items.filter((item) => item.estado === estado).length}</strong>
          </button>
        ))}
      </div>
      {!current.loading && current.items.length === 0 && !current.error && (
        <p className="feature-message">Aún no hay propuestas para este curso.</p>
      )}
      {!current.loading && current.items.length > 0 && current.items.every((item) => item.estado !== statusFilter) && (
        <p className="feature-message">No hay propuestas en estado {statusFilter}.</p>
      )}
      {groupByActivity(current.items.filter((item) => item.estado === statusFilter)).map(([name, items]) => (
        <section className="recommendation-group" key={name}>
          <h3>{name}</h3>
          {items.map((item) => (
        <article className="recommendation-item" key={item.id}>
          <div className="card-heading">
            <div>
              {item.estudiante && <p><strong>Estudiante: {item.estudiante}</strong></p>}
              <p>{item.texto}</p>
              <p className="muted">{item.justificacion}</p>
              <p className="muted">Fecha de evidencia: {formatEvidenceDate(item.fechaEvidenciaUtc)}</p>
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
          {(item.estado === 'Aceptada' || item.estado === 'Ajustada') && (
            <StudyPlan
              courseId={courseId}
              gradebook={gradebook}
              item={item}
              onStudent={(value) => setPlanStudent((current) => ({ ...current, [item.id]: value }))}
              selectedId={planStudent[item.id] || ''}
              students={currentStudents.items}
              threshold={alertStore.threshold}
            />
          )}
        </article>
          ))}
        </section>
      ))}
    </section>
  )
}

function StudyPlan({ courseId, gradebook, item, students, selectedId, onStudent, threshold }) {
  const matched = students.find((student) => item.estudianteId
    ? String(student.moodleId) === String(item.estudianteId)
    : student.nombre === item.estudiante)
  const chosen = matched
    || students.find((student) => String(student.moodleId) === String(selectedId))
  const grades = chosen && gradebook
    ? studentGrades(gradebook.columns, gradebook.rows, chosen.moodleId)
    : []
  const plan = chosen
    ? studyPlanText({
      studentName: chosen.nombre,
      courseName: 'tu curso',
      focusActivity: item.actividad,
      grades,
      threshold,
    })
    : ''

  return (
    <div className="study-plan">
      <h4>Plan de estudio</h4>
      {!matched && (
        <label>
          Estudiante
          <select onChange={(event) => onStudent(event.target.value)} value={chosen ? chosen.moodleId : ''}>
            <option value="">Selecciona un estudiante</option>
            {students.map((student) => (
              <option key={student.moodleId} value={student.moodleId}>{student.nombre}</option>
            ))}
          </select>
        </label>
      )}
      {chosen && <p>{plan}</p>}
      {chosen && (
        <Link className="button button--secondary" to={`/cursos/${courseId}/planes#plan-${chosen.moodleId}`}>
          Revisar y dar visto bueno
        </Link>
      )}
    </div>
  )
}

export default RecommendationsPanel
