import { useEffect, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import CourseAssistantPanel from '../components/CourseAssistantPanel.jsx'
import {
  buildActivityAlerts,
  loadGradeColumns,
  readActivityAlertStore,
  studentGrades,
} from '../services/activityAlerts.js'

function IconCap() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M3 10 12 6l9 4-9 4-9-4Z" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinejoin="round" />
      <path d="M7 12v4.2c0 1.2 2.2 2.8 5 2.8s5-1.6 5-2.8V12" fill="none" stroke="currentColor" strokeWidth="1.6" />
      <path d="M21 10.2V15" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" />
    </svg>
  )
}

function IconBell() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M6 16.5h12l-1.2-2V11a4.8 4.8 0 0 0-9.6 0v3.5L6 16.5Z" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinejoin="round" />
      <path d="M10 18.2a2 2 0 0 0 4 0" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" />
    </svg>
  )
}

function IconChat() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M6 16.5 4 19V7.5A2.5 2.5 0 0 1 6.5 5h11A2.5 2.5 0 0 1 20 7.5v6.2a2.5 2.5 0 0 1-2.5 2.5H6Z" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinejoin="round" />
    </svg>
  )
}

function TopbarActions({ courseId, courseName }) {
  const [open, setOpen] = useState('')
  const [gradebook, setGradebook] = useState(null)
  const rootRef = useRef(null)
  const store = readActivityAlertStore(courseId)
  const book = courseId && gradebook?.courseId === courseId ? gradebook : null
  const alerts = book
    ? buildActivityAlerts(book.columns, book.rows, store).filter((alert) => alert.estado === 'Nueva')
    : []
  const plans = book
    ? book.rows
      .map((row) => ({
        student: row.student,
        grades: studentGrades(book.columns, book.rows, row.student.moodleId),
      }))
      .filter((item) => item.grades.some((grade) => grade.score != null && grade.score < store.threshold))
    : []

  useEffect(() => {
    if (!courseId) return undefined
    let active = true
    loadGradeColumns(courseId)
      .then((book) => {
        if (active) setGradebook({ courseId, ...book })
      })
      .catch(() => {
        if (active) setGradebook({ courseId, columns: [], rows: [] })
      })
    return () => { active = false }
  }, [courseId])

  useEffect(() => {
    if (!open) return undefined
    function close(event) {
      if (!rootRef.current?.contains(event.target)) setOpen('')
    }
    document.addEventListener('mousedown', close)
    return () => document.removeEventListener('mousedown', close)
  }, [open])

  function toggle(name) {
    setOpen((current) => (current === name ? '' : name))
  }

  return (
    <div className="topbar-actions" ref={rootRef}>
      <button
        aria-expanded={open === 'plans'}
        aria-label="Enviar plan de estudio"
        className={open === 'plans' ? 'is-open' : undefined}
        onClick={() => toggle('plans')}
        type="button"
      >
        <IconCap />
      </button>
      <button
        aria-expanded={open === 'alerts'}
        aria-label="Alertas"
        className={open === 'alerts' ? 'is-open' : undefined}
        onClick={() => toggle('alerts')}
        type="button"
      >
        <IconBell />
        {alerts.length > 0 && <span className="topbar-actions__badge">{alerts.length}</span>}
      </button>
      <button
        aria-expanded={open === 'agent'}
        aria-label="Agente académico"
        className={open === 'agent' ? 'is-open' : undefined}
        onClick={() => toggle('agent')}
        type="button"
      >
        <IconChat />
      </button>

      {open === 'alerts' && (
        <section className="topbar-panel" aria-label="Alertas">
          <header>
            <strong>Alertas</strong>
            {courseId
              ? <Link to={`/cursos/${courseId}/alertas`} onClick={() => setOpen('')}>Ver todas</Link>
              : <span>Notas menores a {store.threshold}/10</span>}
          </header>
          {!courseId && <p>Abre un curso para ver sus alertas.</p>}
          {courseId && alerts.length === 0 && <p>No hay alertas nuevas.</p>}
          <ul>
            {alerts.slice(0, 6).map((alert) => (
              <li key={alert.key}>
                <Link
                  className="topbar-panel__alert"
                  to={`/cursos/${courseId}/alertas#alerta-${encodeURIComponent(alert.key)}`}
                  onClick={() => setOpen('')}
                >
                  <strong>{alert.actividad}</strong>
                  <span>{alert.estudiante}: {Number(alert.nota).toLocaleString('es-EC', { maximumFractionDigits: 2, useGrouping: false })}/10</span>
                </Link>
              </li>
            ))}
          </ul>
          {courseId && alerts.length > 6 && (
            <p className="topbar-panel__more">Hay {alerts.length - 6} alertas más en la página de alertas.</p>
          )}
        </section>
      )}

      {open === 'agent' && (
        <section className="topbar-panel topbar-panel--agent" aria-label="Agente académico">
          <header><strong>Agente académico</strong></header>
          {courseId
            ? <CourseAssistantPanel courseId={courseId} />
            : <p>Abre un curso para conversar con el agente.</p>}
        </section>
      )}

      {open === 'plans' && (
        <section className="topbar-panel" aria-label="Planes de estudio">
          <header>
            <strong>Plan de estudio</strong>
            {courseId && <Link to={`/cursos/${courseId}/planes`} onClick={() => setOpen('')}>Revisar</Link>}
          </header>
          {!courseId && <p>Abre un curso para preparar un plan.</p>}
          {courseId && plans.length === 0 && <p>Ningún estudiante tiene notas menores al corte.</p>}
          <p>El agente prepara el plan{courseName ? ` de ${courseName}` : ''}. El correo se registra cuando das el visto bueno.</p>
          <ul>
            {plans.map(({ student }) => (
              <li key={student.moodleId}>
                <Link className="topbar-panel__alert" to={`/cursos/${courseId}/planes#plan-${student.moodleId}`} onClick={() => setOpen('')}>
                  <strong>{student.nombre}</strong>
                  <span>{student.email || 'Sin correo en Moodle'}</span>
                </Link>
              </li>
            ))}
          </ul>
        </section>
      )}
    </div>
  )
}

export default TopbarActions
