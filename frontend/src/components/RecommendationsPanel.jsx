import { useEffect, useState } from 'react'
import {
  generateCourseRecommendations,
  getCourseRecommendations,
  updateRecommendation,
} from '../services/api.js'

function RecommendationsPanel({ courseId }) {
  const [state, setState] = useState({ courseId: null, loading: true, error: '', items: [] })
  const [busy, setBusy] = useState(false)
  const [message, setMessage] = useState('')
  const [drafts, setDrafts] = useState({})
  const current = state.courseId === courseId ? state : { loading: true, error: '', items: [] }

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

  async function generate() {
    setBusy(true)
    setMessage('')
    setState((previous) => ({ ...previous, error: '' }))
    try {
      const result = await generateCourseRecommendations(courseId)
      setMessage(result.creadas
        ? `Se generaron ${result.creadas} propuestas justificadas.`
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
          <p className="muted">Cada propuesta se genera solo con una actividad bajo 6/10 y al menos 7 calificaciones.</p>
        </div>
        <button className="button button--secondary" disabled={busy || current.loading} onClick={generate} type="button">
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
              <h3>{item.actividad}</h3>
              <p>{item.texto}</p>
              <p className="muted">{item.justificacion}</p>
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
