import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  downloadCourseReport,
  generateCourseReport,
  getCourseReport,
  getCourseReports,
} from '../services/api.js'

function formatDate(value) {
  return value ? new Date(value).toLocaleString('es-EC') : 'No disponible'
}

function InformesCurso() {
  const { id } = useParams()
  const [reports, setReports] = useState([])
  const [selected, setSelected] = useState(null)
  const [period, setPeriod] = useState('')
  const [loading, setLoading] = useState(true)
  const [generating, setGenerating] = useState(false)
  const [message, setMessage] = useState(null)

  useEffect(() => {
    let active = true
    getCourseReports(id)
      .then((data) => active && setReports(data))
      .catch((error) => active && setMessage({ type: 'error', text: error.message }))
      .finally(() => active && setLoading(false))
    return () => { active = false }
  }, [id])

  const generate = async (event) => {
    event.preventDefault()
    setGenerating(true)
    setMessage(null)
    try {
      const report = await generateCourseReport(id, period.trim())
      setReports(await getCourseReports(id))
      setSelected(report)
      setMessage({ type: 'success', text: 'Informe generado correctamente.' })
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    } finally {
      setGenerating(false)
    }
  }

  const view = async (reportId) => {
    setMessage(null)
    try {
      setSelected(await getCourseReport(id, reportId))
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    }
  }

  const download = async (reportId) => {
    setMessage(null)
    try {
      await downloadCourseReport(id, reportId)
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    }
  }

  const content = selected?.contenido

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to={`/cursos/${id}`}>← Volver al curso</Link>
          <span className="eyebrow">INFORMES</span>
          <h1>Informe académico del curso</h1>
          <p className="muted">Genere un informe con las métricas y hallazgos de la última sincronización.</p>
        </div>
      </div>

      {message && (
        <p className={`course-message course-message--${message.type}`} role={message.type === 'error' ? 'alert' : 'status'}>
          {message.text}
        </p>
      )}

      <form className="content-card student-filters" onSubmit={generate}>
        <label htmlFor="report-period">Período (opcional)</label>
        <input
          id="report-period"
          maxLength={150}
          onChange={(event) => setPeriod(event.target.value)}
          placeholder="Por defecto: período activo del curso"
          value={period}
        />
        <button className="button button--primary" disabled={generating} type="submit">
          {generating ? 'Generando…' : 'Generar informe'}
        </button>
      </form>

      {content && (
        <section className="content-card">
          <div className="card-heading">
            <div>
              <h2>{content.curso}</h2>
              <p className="muted">
                Período: {selected.periodo} · Generado: {formatDate(selected.fechaGeneracionUtc)} ·
                Datos actualizados al: {formatDate(selected.fechaActualizacionDatosUtc)}
              </p>
            </div>
            <button className="button button--secondary" onClick={() => download(selected.id)} type="button">Descargar</button>
          </div>
          {content.aviso && <p className="course-message course-message--warning" role="status">{content.aviso}</p>}
          <h3>Métricas</h3>
          <ul>
            <li>Estudiantes: {content.indicadores.estudiantes}</li>
            <li>Promedio: {content.indicadores.promedio == null ? 'Sin datos' : Number(content.indicadores.promedio).toFixed(2)}</li>
            <li>Actividades evaluadas: {content.indicadores.actividadesEvaluadas} de {content.indicadores.actividadesTotales}</li>
          </ul>
          <h3>Hallazgos</h3>
          <ul>{content.hallazgos.map((item) => <li key={item}>{item}</li>)}</ul>
          <h3>Actividades bajo el umbral</h3>
          {content.actividadesBajoUmbral.length === 0
            ? <p className="muted">Ninguna.</p>
            : <ul>{content.actividadesBajoUmbral.map((a) => <li key={a.itemId}>{a.nombre}: {Number(a.promedio).toFixed(2)} ({a.numCalificaciones} calificaciones)</li>)}</ul>}
          <h3>Recomendaciones</h3>
          {content.recomendaciones.length === 0
            ? <p className="muted">Sin recomendaciones registradas.</p>
            : <ul>{content.recomendaciones.map((item) => <li key={item}>{item}</li>)}</ul>}
        </section>
      )}

      <section className="content-card">
        <div className="card-heading"><div><h2>Informes generados</h2></div></div>
        <div className="table-scroll">
          <table>
            <thead><tr><th>Fecha</th><th>Período</th><th>Datos</th><th>Acciones</th></tr></thead>
            <tbody>
              {loading && <tr><td colSpan="4">Cargando informes…</td></tr>}
              {!loading && reports.length === 0 && <tr><td colSpan="4">Aún no hay informes generados.</td></tr>}
              {reports.map((report) => (
                <tr key={report.id}>
                  <td>{formatDate(report.fechaGeneracionUtc)}</td>
                  <td>{report.periodo}</td>
                  <td>{report.datosIncompletos ? 'Incompletos' : 'Completos'}</td>
                  <td>
                    <button className="button button--secondary" onClick={() => view(report.id)} type="button">Ver</button>{' '}
                    <button className="button button--secondary" onClick={() => download(report.id)} type="button">Descargar</button>
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

export default InformesCurso
