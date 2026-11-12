import { Fragment, useCallback, useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getCourseAlerts, updateAlertState } from '../services/api.js'

const stateTone = { Nueva: 'warning', Revisada: 'success', Descartada: 'neutral' }

function formatDate(value) {
  return value ? new Date(value).toLocaleString('es-EC') : 'No disponible'
}

function AlertasCurso() {
  const { id } = useParams()
  const [filter, setFilter] = useState('')
  const [panel, setPanel] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [expanded, setExpanded] = useState(null)
  const [busy, setBusy] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      setPanel(await getCourseAlerts(id, filter))
    } catch (loadError) {
      setPanel(null)
      setError(loadError.message)
    } finally {
      setLoading(false)
    }
  }, [id, filter])

  useEffect(() => { load() }, [load])

  const change = async (alert, estado) => {
    const key = `${alert.estudianteId}-${alert.tipo}`
    setBusy(key)
    setError('')
    try {
      await updateAlertState(id, alert.estudianteId, alert.tipo, estado)
      await load()
    } catch (changeError) {
      setError(changeError.message)
    } finally {
      setBusy('')
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to={`/cursos/${id}`}>← Volver al curso</Link>
          <span className="eyebrow">ALERTAS ACADÉMICAS</span>
          <h1>Alertas del curso</h1>
          <p className="muted">Situaciones que conviene revisar según las reglas configuradas.</p>
        </div>
        <Link className="button button--secondary" to={`/cursos/${id}/alertas/reglas`}>Configurar reglas</Link>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}
      {panel?.datosIncompletos && (
        <p className="course-message course-message--warning" role="status">
          {panel.aviso || 'Datos incompletos: la sincronización no terminó correctamente, no se generan alertas.'}
        </p>
      )}

      <section className="content-card">
        <div className="card-heading">
          <div><h2>Alertas generadas</h2></div>
          <label>
            Estado{' '}
            <select onChange={(event) => setFilter(event.target.value)} value={filter}>
              <option value="">Todas</option>
              <option value="Nueva">Nueva</option>
              <option value="Revisada">Revisada</option>
              <option value="Descartada">Descartada</option>
            </select>
          </label>
        </div>
        <div className="table-scroll">
          <table>
            <thead><tr><th>Estudiante</th><th>Regla aplicada</th><th>Fecha</th><th>Estado</th><th>Acciones</th></tr></thead>
            <tbody>
              {loading && <tr><td colSpan="5">Cargando alertas…</td></tr>}
              {!loading && panel && panel.alertas.length === 0 && (
                <tr><td colSpan="5">No hay alertas para mostrar.</td></tr>
              )}
              {panel?.alertas.map((alert) => {
                const key = `${alert.estudianteId}-${alert.tipo}`
                return (
                  <Fragment key={key}>
                    <tr>
                      <td><Link to={`/cursos/${id}/estudiantes/${alert.estudianteId}`}>{alert.estudiante}</Link></td>
                      <td>{alert.regla}</td>
                      <td>{formatDate(alert.fechaUtc)}</td>
                      <td><span className={`status-pill status-pill--${stateTone[alert.estado] ?? 'neutral'}`}>{alert.estado}</span></td>
                      <td>
                        <button
                          className="button button--secondary"
                          onClick={() => setExpanded(expanded === key ? null : key)}
                          type="button"
                        >
                          {expanded === key ? 'Ocultar evidencia' : 'Ver evidencia'}
                        </button>{' '}
                        {alert.estado !== 'Revisada' && (
                          <button className="button button--secondary" disabled={busy === key} onClick={() => change(alert, 'Revisada')} type="button">Marcar revisada</button>
                        )}{' '}
                        {alert.estado !== 'Descartada' && (
                          <button className="button button--secondary" disabled={busy === key} onClick={() => change(alert, 'Descartada')} type="button">Descartar</button>
                        )}
                      </td>
                    </tr>
                    {expanded === key && (
                      <tr>
                        <td colSpan="5">
                          <p>{alert.texto}</p>
                          <ul>
                            {alert.evidencias.map((evidence, index) => (
                              <li key={`${evidence.item}-${index}`}>
                                <strong>{evidence.item}</strong>: {evidence.valor} — {evidence.descripcion}
                                {evidence.fechaUtc ? ` (${formatDate(evidence.fechaUtc)})` : ''}
                              </li>
                            ))}
                          </ul>
                        </td>
                      </tr>
                    )}
                  </Fragment>
                )
              })}
            </tbody>
          </table>
        </div>
        {panel && (
          <p className="muted">Fuente: {panel.fuente} · Última sincronización: {formatDate(panel.ultimaSincronizacionUtc)}</p>
        )}
      </section>
    </>
  )
}

export default AlertasCurso
