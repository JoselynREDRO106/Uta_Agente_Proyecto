import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { loadGradeColumns, readActivityAlertStore } from '../services/activityAlerts.js'
import { formatReport, runLogicChecks } from '../services/logicChecks.js'
import { draftsFromGradebook } from '../services/studyPlans.js'

function downloadReport(text) {
  const blob = new Blob([text], { type: 'text/plain;charset=utf-8' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = 'informe-pruebas.txt'
  link.click()
  URL.revokeObjectURL(url)
}

async function runLiveChecks(courseId) {
  const started = performance.now()
  try {
    const book = await loadGradeColumns(courseId)
    const threshold = readActivityAlertStore(courseId).threshold
    const drafts = draftsFromGradebook({
      courseName: '',
      columns: book.columns,
      rows: book.rows,
      threshold,
    })
    const elapsed = Math.round(performance.now() - started)
    return [
      {
        name: 'Las calificaciones sincronizadas responden',
        passed: book.columns.length > 0 && book.rows.length > 0,
        detail: `${book.rows.length} estudiantes y ${book.columns.length} actividades, en ${elapsed} ms.`,
      },
      {
        name: 'El agente puede preparar borradores con esas notas',
        passed: drafts.every((plan) => plan.estado === 'Borrador' && plan.texto.includes('Plan de estudio')),
        detail: drafts.length === 0
          ? 'No hay estudiantes bajo el corte, así que no hay borradores. La regla se cumplió.'
          : `${drafts.length} borradores listos para revisión. Ninguno quedó notificado por esta prueba.`,
      },
    ]
  } catch (error) {
    return [{
      name: 'Las calificaciones sincronizadas responden',
      passed: false,
      detail: error.message,
    }]
  }
}

function PruebasCurso() {
  const { id } = useParams()
  const [results, setResults] = useState(() => runLogicChecks())
  const [running, setRunning] = useState(false)
  const [generatedAt, setGeneratedAt] = useState(() => new Date().toLocaleString('es-EC'))

  async function run() {
    setRunning(true)
    const logic = runLogicChecks()
    const live = await runLiveChecks(id)
    setResults([...logic, ...live])
    setGeneratedAt(new Date().toLocaleString('es-EC'))
    setRunning(false)
  }

  const report = formatReport(results, {
    title: `Informe de pruebas automáticas del curso ${id}`,
    generatedAt,
  })
  const failed = results.filter((item) => !item.passed).length

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">PRUEBAS AUTOMÁTICAS</span>
          <h1>Informe de pruebas</h1>
          <p className="muted">
            Cada fila es una comprobación. Pasó significa que el resultado fue el esperado. Falló incluye el motivo, para corregirlo antes de subir la rama.
          </p>
        </div>
        <button className="button button--primary" disabled={running} onClick={run} type="button">
          {running ? 'Ejecutando…' : 'Ejecutar pruebas'}
        </button>
      </div>
      <p className="alert-counts" role="status">
        <span>Total <strong>{results.length}</strong></span>
        <span>Pasaron <strong>{results.length - failed}</strong></span>
        <span>Fallaron <strong>{failed}</strong></span>
      </p>
      <section className="content-card">
        <ul className="check-report">
          {results.map((item) => (
            <li className={item.passed ? 'check-report__pass' : 'check-report__fail'} key={item.name}>
              <strong>{item.passed ? 'Pasó' : 'Falló'}</strong>
              <span>{item.name}</span>
              <p>{item.detail}</p>
            </li>
          ))}
        </ul>
        <div className="recommendation-actions">
          <button className="button button--secondary" onClick={() => navigator.clipboard.writeText(report)} type="button">Copiar informe</button>
          <button className="button button--secondary" onClick={() => downloadReport(`${report}\n`)} type="button">Descargar informe</button>
        </div>
      </section>
    </>
  )
}

export default PruebasCurso
