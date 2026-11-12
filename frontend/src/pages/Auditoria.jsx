import { useEffect, useState } from 'react'
import { getAuditLogs } from '../services/api.js'

const pageSize = 20

function formatUtcDate(value) {
  if (!value) return 'Sin fecha'
  return new Intl.DateTimeFormat('es-EC', {
    dateStyle: 'medium',
    timeStyle: 'short',
    timeZone: 'America/Guayaquil',
  }).format(new Date(value))
}

function toUtcStart(date) {
  return date ? `${date}T00:00:00-05:00` : ''
}

function toUtcEnd(date) {
  return date ? `${date}T23:59:59-05:00` : ''
}

function Auditoria() {
  const [typeInput, setTypeInput] = useState('')
  const [fromInput, setFromInput] = useState('')
  const [toInput, setToInput] = useState('')
  const [filter, setFilter] = useState({ tipo: '', desdeUtc: '', hastaUtc: '' })
  const [page, setPage] = useState(1)
  const [result, setResult] = useState({ key: '', data: null, error: '' })
  const requestKey = JSON.stringify([filter, page])
  const loading = result.key !== requestKey
  const error = loading ? '' : result.error
  const data = loading ? null : result.data

  useEffect(() => {
    let active = true
    getAuditLogs({ ...filter, pagina: page, tamanoPagina: pageSize })
      .then((data) => {
        if (active) setResult({ key: requestKey, data, error: '' })
      })
      .catch((requestError) => {
        if (active) setResult({ key: requestKey, data: null, error: requestError.message })
      })
    return () => { active = false }
  }, [filter, page, requestKey])

  function applyFilters(event) {
    event.preventDefault()
    setPage(1)
    setFilter({
      tipo: typeInput,
      desdeUtc: toUtcStart(fromInput),
      hastaUtc: toUtcEnd(toInput),
    })
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ADMINISTRACIÓN</span>
          <h1>Auditoría</h1>
          <p className="muted">Consultas, sincronizaciones y cambios de recomendaciones. No se almacenan secretos ni texto de preguntas.</p>
        </div>
      </div>
      <section className="content-card">
        <form className="audit-filters" onSubmit={applyFilters}>
          <label htmlFor="audit-type">Tipo</label>
          <select id="audit-type" onChange={(event) => setTypeInput(event.target.value)} value={typeInput}>
            <option value="">Todos</option>
            <option value="Sincronizacion">Sincronización</option>
            <option value="ConsultaAgente">Consulta al agente</option>
            <option value="AnalisisGrupal">Análisis grupal</option>
            <option value="RecomendacionGenerada">Recomendación generada</option>
            <option value="EstadoRecomendacionActualizado">Estado de recomendación</option>
          </select>
          <label htmlFor="audit-from">Desde</label>
          <input id="audit-from" onChange={(event) => setFromInput(event.target.value)} type="date" value={fromInput} />
          <label htmlFor="audit-to">Hasta</label>
          <input id="audit-to" onChange={(event) => setToInput(event.target.value)} type="date" value={toInput} />
          <button className="button button--secondary" type="submit">Filtrar</button>
        </form>
        {error && <p className="course-message course-message--error" role="alert">{error}</p>}
        <div className="table-scroll">
          <table>
            <thead>
              <tr><th>Fecha</th><th>Usuario</th><th>Rol</th><th>Tipo</th><th>Curso</th><th>Resultado</th><th>Detalle</th></tr>
            </thead>
            <tbody>
              {loading && <tr><td colSpan="7">Cargando registros…</td></tr>}
              {!loading && data?.items?.map((item) => (
                <tr key={item.id}>
                  <td>{formatUtcDate(item.fechaUtc)}</td>
                  <td>{item.usuario}</td>
                  <td>{item.rol}</td>
                  <td>{item.tipo}</td>
                  <td>{item.cursoId ?? '—'}</td>
                  <td>{item.resultado}</td>
                  <td>{item.detalle}</td>
                </tr>
              ))}
              {!loading && !error && data?.items?.length === 0 && (
                <tr><td colSpan="7">No hay registros para los filtros seleccionados.</td></tr>
              )}
            </tbody>
          </table>
        </div>
        <div className="pagination">
          <span className="muted">{data?.totalItems ?? 0} registros · página {data?.page ?? page} de {data?.totalPages ?? 0}</span>
          <div>
            <button className="button button--secondary" disabled={loading || page <= 1} onClick={() => setPage((value) => value - 1)} type="button">Anterior</button>
            <button className="button button--secondary" disabled={loading || page >= (data?.totalPages ?? 0)} onClick={() => setPage((value) => value + 1)} type="button">Siguiente</button>
          </div>
        </div>
      </section>
    </>
  )
}

export default Auditoria
