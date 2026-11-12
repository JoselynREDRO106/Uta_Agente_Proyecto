import { useState } from 'react'
import { askCourseAssistant } from '../services/api.js'

function AnswerList({ title, items }) {
  if (!items?.length) return null
  return <section><h4>{title}</h4><ul>{items.map((item, index) => <li key={`${index}-${item}`}>{item}</li>)}</ul></section>
}

function CourseAssistantPanel({ courseId }) {
  const [question, setQuestion] = useState('')
  const [state, setState] = useState({ loading: false, error: '', answer: null })

  async function submit(event) {
    event.preventDefault()
    const value = question.trim()
    if (!value) return

    setState({ loading: true, error: '', answer: null })
    try {
      const answer = await askCourseAssistant(courseId, value)
      setState({ loading: false, error: '', answer })
    } catch (error) {
      setState({ loading: false, error: error.message, answer: null })
    }
  }

  return (
    <section className="content-card feature-panel" aria-labelledby="assistant-heading">
      <div className="card-heading">
        <div>
          <h2 id="assistant-heading">Consultar al agente de IA</h2>
          <p className="muted">El modelo local recibe la pregunta para clasificarla; los datos del curso se limitan a indicadores agregados.</p>
        </div>
      </div>
      <form className="feature-form" onSubmit={submit}>
        <label htmlFor="assistant-question">¿Qué quieres consultar sobre este curso?</label>
        <textarea
          id="assistant-question"
          maxLength={1000}
          onChange={(event) => setQuestion(event.target.value)}
          placeholder="Ej.: ¿Qué actividades están bajo el umbral?"
          required
          rows={3}
          value={question}
        />
        <div className="feature-form__footer">
          <span className="muted">El modelo no recibe nombres, correos ni calificaciones individuales del curso.</span>
          <button className="button button--primary" disabled={state.loading || !question.trim()} type="submit">
            {state.loading ? 'Consultando…' : 'Consultar'}
          </button>
        </div>
      </form>
      {state.error && <p className="course-message course-message--error" role="alert">{state.error}</p>}
      {state.answer && (
        <div className="feature-answer" aria-live="polite">
          <h3>Respuesta</h3>
          <p>{state.answer.respuesta}</p>
          <AnswerList title="Hechos" items={state.answer.hechos} />
          <AnswerList title="Inferencias" items={state.answer.inferencias} />
          <AnswerList title="Recomendaciones propuestas" items={state.answer.recomendaciones} />
          <p><strong>Incertidumbre:</strong> {state.answer.incertidumbre}</p>
          <p className="muted">Fuente: {state.answer.fuente} · Última sincronización: {state.answer.ultimaSincronizacionUtc ?? 'No disponible'}</p>
        </div>
      )}
    </section>
  )
}

export default CourseAssistantPanel
