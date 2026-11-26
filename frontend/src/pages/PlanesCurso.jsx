import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { loadGradeColumns, readActivityAlertStore } from '../services/activityAlerts.js'
import { getCourses } from '../services/api.js'
import {
  approveAndNotify,
  draftsFromGradebook,
  mergePlans,
  readStudyPlans,
  saveReview,
  writeStudyPlans,
} from '../services/studyPlans.js'

function formatWhen(value) {
  return value ? new Date(value).toLocaleString('es-EC') : ''
}

function PlanesCurso() {
  const { id } = useParams()
  const [plans, setPlans] = useState([])
  const [drafts, setDrafts] = useState({})
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let active = true
    Promise.all([
      getCourses().catch(() => []),
      loadGradeColumns(id),
    ]).then(([courses, book]) => {
      if (!active) return
      const course = courses.find((item) => String(item.id) === String(id))
      const threshold = readActivityAlertStore(id).threshold
      const next = mergePlans(readStudyPlans(id), draftsFromGradebook({
        courseName: course?.name || '',
        columns: book.columns,
        rows: book.rows,
        threshold,
      }))
      writeStudyPlans(id, next)
      setPlans(next)
      setDrafts(Object.fromEntries(next.map((plan) => [plan.id, plan.texto])))
    }).catch((loadError) => {
      if (active) setError(loadError.message)
    }).finally(() => {
      if (active)       setLoading(false)
    })
    return () => { active = false }
  }, [id])

  useEffect(() => {
    if (loading || !window.location.hash) return
    document.getElementById(window.location.hash.slice(1))?.scrollIntoView({ block: 'center' })
  }, [loading, plans])

  function persist(next, notice) {
    writeStudyPlans(id, next)
    setPlans(next)
    setMessage(notice)
    setError('')
  }

  function review(plan) {
    const result = saveReview(plans, plan.id, drafts[plan.id] ?? plan.texto)
    if (result.error) {
      setError(result.error)
      return
    }
    persist(result.plans, 'La revisión quedó guardada en esta prueba. El estudiante todavía no recibe el aviso.')
  }

  function approve(plan) {
    const reviewed = saveReview(plans, plan.id, drafts[plan.id] ?? plan.texto)
    const source = reviewed.error ? plans : reviewed.plans
    const result = approveAndNotify(source, plan.id)
    if (result.error) {
      setError(result.error)
      return
    }
    const sent = result.plans.find((item) => item.id === plan.id)
    persist(result.plans, `Visto bueno registrado. El agente dejó el aviso de prueba para ${sent.destino}.`)
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">PLAN DE ESTUDIO</span>
          <h1>Revisión del docente</h1>
          <p className="muted">
            El agente arma cada plan con las notas sincronizadas de Moodle. Tú lo revisas y, al dar el visto bueno, el agente registra la notificación al correo del estudiante.
            Esta prueba vive solo en el navegador: no entra a la base de datos ni sale por un servidor de correo.
          </p>
        </div>
      </div>
      {error && <p className="course-message course-message--error" role="alert">{error}</p>}
      {message && <p className="course-message course-message--success" role="status">{message}</p>}
      {loading && <p className="feature-message">El agente está leyendo las calificaciones…</p>}
      {!loading && plans.length === 0 && !error && (
        <p className="feature-message">Ningún estudiante tiene notas menores al corte.</p>
      )}
      <div className="plan-list">
        {plans.map((plan) => (
          <article className="content-card study-plan-card" id={plan.id} key={plan.id}>
            <div className="card-heading">
              <div>
                <h2>{plan.estudiante}</h2>
                <p className="muted">{plan.email || 'Moodle no tiene correo para este estudiante.'}</p>
              </div>
              <span className="count-pill">{plan.estado}</span>
            </div>
            <label>
              Propuesta del agente
              <textarea
                disabled={plan.estado === 'Notificado'}
                onChange={(event) => setDrafts((current) => ({ ...current, [plan.id]: event.target.value }))}
                rows={10}
                value={drafts[plan.id] ?? plan.texto}
              />
            </label>
            {plan.estado !== 'Notificado' && (
              <div className="recommendation-actions">
                <button className="button button--secondary" onClick={() => review(plan)} type="button">Guardar revisión</button>
                <button className="button button--primary" onClick={() => approve(plan)} type="button">Dar visto bueno y notificar</button>
              </div>
            )}
            {plan.estado === 'Notificado' && (
              <div className="notice-preview">
                <p><strong>Aviso registrado en la prueba</strong></p>
                <p>Para: {plan.destino}</p>
                <p>Asunto: {plan.asunto}</p>
                <p>Cuando: {formatWhen(plan.notificadoEn)}</p>
                <pre>{plan.texto}</pre>
              </div>
            )}
          </article>
        ))}
      </div>
    </>
  )
}

export default PlanesCurso
