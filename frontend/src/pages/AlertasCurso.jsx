import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import {
  buildActivityAlerts,
  loadGradeColumns,
  readActivityAlertStore,
  writeActivityAlertStore,
} from '../services/activityAlerts.js'

const stateTone = { Nueva: 'warning', Revisada: 'success', Descartada: 'neutral' }
const stateLabel = { Nueva: 'nuevas', Revisada: 'revisadas', Descartada: 'descartadas' }

function formatGrade(value) {
  return Number(value).toLocaleString('es-EC', { maximumFractionDigits: 2, useGrouping: false })
}

function AlertasCurso() {
  const { id } = useParams()
  const location = useLocation()
  const [filter, setFilter] = useState('Nueva')
  const [gradebook, setGradebook] = useState(null)
  const [store, setStore] = useState(() => readActivityAlertStore(id))
  const [draft, setDraft] = useState(() => String(readActivityAlertStore(id).threshold))
  const [thresholdError, setThresholdError] = useState('')
  const loading = gradebook?.courseId !== id
  const error = loading ? '' : gradebook.error
  const alerts = loading ? [] : buildActivityAlerts(gradebook.columns, gradebook.rows, store)

  useEffect(() => {
    const next = readActivityAlertStore(id)
    setStore(next)
    setDraft(String(next.threshold))
  }, [id])

  useEffect(() => {
    let active = true
    loadGradeColumns(id)
      .then((book) => {
        if (active) setGradebook({ courseId: id, error: '', ...book })
      })
      .catch((loadError) => {
        if (active) setGradebook({ courseId: id, error: loadError.message, columns: [], rows: [] })
      })
    return () => { active = false }
  }, [id])

  useEffect(() => {
    if (loading || !location.hash.startsWith('#alerta-')) return undefined
    const key = decodeURIComponent(location.hash.slice('#alerta-'.length))
    const match = alerts.find((alert) => alert.key === key)
    if (match && match.estado !== filter) setFilter(match.estado)
    const row = document.getElementById(`alerta-${key}`)
    row?.scrollIntoView({ block: 'center' })
    return undefined
  }, [alerts, filter, loading, location.hash])

  function applyThreshold(event) {
    event.preventDefault()
    const threshold = Number(draft)
    if (!Number.isFinite(threshold) || threshold <= 0 || threshold > 10) {
      setThresholdError('El corte debe ser mayor que 0 y no superar 10.')
      return
    }
    const next = { ...store, threshold }
    writeActivityAlertStore(id, next)
    setStore(next)
    setThresholdError('')
  }

  function change(alert, estado) {
    const next = { ...store, states: { ...store.states, [alert.key]: estado } }
    writeActivityAlertStore(id, next)
    setStore(next)
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ALERTAS ACADÉMICAS</span>
          <h1>Alertas por actividad</h1>
          <p className="muted">
            Sale una alerta cuando la nota de un estudiante en una actividad es menor a {Number(store.threshold).toFixed(2)}/10.
            Una calificación de 4 entra. Si no hay nota, no se genera alerta.
          </p>
        </div>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}
      <form className="analysis-controls content-card" onSubmit={applyThreshold}>
        <label>
          Alertar si la nota es menor a
          <input
            max="10"
            min="0.01"
            onChange={(event) => setDraft(event.target.value)}
            step="0.01"
            type="number"
            value={draft}
          />
        </label>
        <button className="button button--primary" type="submit">Aplicar</button>
        {thresholdError && <p className="course-message course-message--error" role="alert">{thresholdError}</p>}
      </form>

      <div className="alert-counts" role="group" aria-label="Filtrar alertas por estado">
        {['Nueva', 'Revisada', 'Descartada'].map((estado) => (
          <button
            className={filter === estado ? 'is-selected' : undefined}
            key={estado}
            onClick={() => setFilter(estado)}
            type="button"
          >
            {estado === 'Nueva' ? 'Nuevas' : estado === 'Revisada' ? 'Revisadas' : 'Descartadas'}
            <strong>{alerts.filter((alert) => alert.estado === estado).length}</strong>
          </button>
        ))}
      </div>

      <section className="content-card">
        <div className="table-scroll">
          <table>
            <thead><tr><th>Actividad</th><th>Estudiante</th><th>Nota</th><th>Estado</th><th>Acciones</th></tr></thead>
            <tbody>
              {loading && <tr><td colSpan="5">Cargando calificaciones…</td></tr>}
              {!loading && alerts.filter((alert) => alert.estado === filter).length === 0 && (
                <tr><td colSpan="5">No hay alertas {stateLabel[filter]} con nota menor a {Number(store.threshold).toFixed(2)}/10.</td></tr>
              )}
              {alerts.filter((alert) => alert.estado === filter).map((alert) => (
                <tr className={location.hash === `#alerta-${encodeURIComponent(alert.key)}` ? 'is-focused' : undefined} id={`alerta-${alert.key}`} key={alert.key}>
                  <td>
                    <Link to={`/cursos/${id}/estudiantes#actividad-${encodeURIComponent(alert.actividad)}`}>{alert.actividad}</Link>
                  </td>
                  <td><Link to={`/cursos/${id}/estudiantes/${alert.estudianteId}`}>{alert.estudiante}</Link></td>
                  <td className="gradebook__low">{formatGrade(alert.nota)}</td>
                  <td><span className={`status-pill status-pill--${stateTone[alert.estado] ?? 'neutral'}`}>{alert.estado}</span></td>
                  <td>
                    {alert.estado !== 'Revisada' && (
                      <button className="button button--secondary" onClick={() => change(alert, 'Revisada')} type="button">Marcar revisada</button>
                    )}{' '}
                    {alert.estado !== 'Descartada' && (
                      <button className="button button--secondary" onClick={() => change(alert, 'Descartada')} type="button">Descartar</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </>
  )
}

export default AlertasCurso
