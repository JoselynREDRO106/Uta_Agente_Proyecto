import { useEffect, useRef, useState } from 'react'
import { askCourseAssistant, getCourseIndicators, getGroupAnalysis } from '../services/api.js'
import { composeAcademicReply } from '../services/academicReply.js'
import { readAnalysisSettings } from '../services/analysisSettings.js'

const greeting = {
  id: 'greeting',
  role: 'assistant',
  text: 'Hola. Puedo responder sobre el promedio, el progreso, las actividades y la tendencia de este curso.',
}

const suggestedQuestions = [
  '¿Cuál es el promedio del curso?',
  '¿Cuál es la actividad más baja?',
  '¿Cómo es la tendencia por actividad?',
]

function threadKey(courseId) {
  return `edumonitor-agent-${courseId}`
}

function readThread(courseId) {
  try {
    const saved = JSON.parse(sessionStorage.getItem(threadKey(courseId)) || '')
    if (Array.isArray(saved) && saved.length > 0) return saved
  } catch {
    // Si la conversación guardada no se puede leer, el chat vuelve al saludo.
  }
  return [greeting]
}

function CourseAssistantPanel({ courseId }) {
  const [messages, setMessages] = useState(() => readThread(courseId))
  const [draft, setDraft] = useState('')
  const [sending, setSending] = useState(false)
  const threadRef = useRef(null)
  const storedCourse = useRef(courseId)

  useEffect(() => {
    if (storedCourse.current !== courseId) {
      storedCourse.current = courseId
      setMessages(readThread(courseId))
      setDraft('')
      return
    }
    sessionStorage.setItem(threadKey(courseId), JSON.stringify(messages))
  }, [courseId, messages])

  useEffect(() => {
    const thread = threadRef.current
    if (thread) thread.scrollTop = thread.scrollHeight
  }, [messages, sending])

  async function ask(question) {
    const value = question.trim()
    if (!value || sending) return

    setMessages((current) => [...current, { id: crypto.randomUUID(), role: 'user', text: value }])
    setDraft('')
    setSending(true)
    try {
      let answer
      let source = 'agent'
      try {
        answer = await askCourseAssistant(courseId, value)
      } catch {
        source = 'local'
        const indicators = await getCourseIndicators(courseId)
        let groupAnalysis = null
        try {
          const settings = readAnalysisSettings(courseId) ?? { umbral: 6, minimoCalificaciones: 1 }
          groupAnalysis = await getGroupAnalysis(courseId, settings)
        } catch {
          groupAnalysis = null
        }
        answer = composeAcademicReply(value, indicators, groupAnalysis)
      }
      setMessages((current) => [...current, {
        id: crypto.randomUUID(),
        role: 'assistant',
        source,
        text: answer.respuesta,
        hechos: answer.hechos ?? [],
        recomendaciones: answer.recomendaciones ?? [],
        incertidumbre: answer.incertidumbre,
      }])
    } catch (error) {
      setMessages((current) => [...current, {
        id: crypto.randomUUID(),
        role: 'assistant',
        text: error.message,
        error: true,
      }])
    } finally {
      setSending(false)
    }
  }

  return (
    <section aria-labelledby="assistant-heading" className="content-card feature-panel assistant-chat">
      <div aria-live="polite" className="assistant-chat__thread" ref={threadRef}>
        {messages.map((message) => (
          <article className={`assistant-chat__message assistant-chat__message--${message.role}${message.error ? ' is-error' : ''}`} key={message.id}>
            <span className="assistant-chat__author">{message.role === 'user' ? 'Tú' : 'IA académica'}</span>
            <p>{message.text}</p>
            {message.role === 'assistant' && message.source && (
              <small className="assistant-chat__source">{message.source === 'agent' ? 'Agente' : 'Resumen local'}</small>
            )}
            {message.hechos?.length > 0 && (
              <details>
                <summary>Datos usados</summary>
                <ul>{message.hechos.map((item) => <li key={item}>{item}</li>)}</ul>
                {message.recomendaciones?.length > 0 && (
                  <p><strong>Propuesta: </strong>{message.recomendaciones.join(' ')}</p>
                )}
                {message.incertidumbre && <p>{message.incertidumbre}</p>}
              </details>
            )}
          </article>
        ))}
        {sending && (
          <article className="assistant-chat__message assistant-chat__message--assistant">
            <span className="assistant-chat__author">IA académica</span>
            <p>Revisando los indicadores del curso…</p>
          </article>
        )}
      </div>
      <div className="assistant-chat__prompts">
        {suggestedQuestions.map((question) => (
          <button disabled={sending} key={question} onClick={() => ask(question)} type="button">{question}</button>
        ))}
      </div>
      <form className="assistant-chat__composer" onSubmit={(event) => { event.preventDefault(); ask(draft) }}>
        <label className="visually-hidden" htmlFor="assistant-question">Pregunta para la IA académica</label>
        <input
          id="assistant-question"
          maxLength={1000}
          onChange={(event) => setDraft(event.target.value)}
          placeholder="Escribe una pregunta sobre el curso"
          value={draft}
        />
        <button className="button button--primary" disabled={sending || !draft.trim()} type="submit">
          Enviar
        </button>
      </form>
    </section>
  )
}

export default CourseAssistantPanel
