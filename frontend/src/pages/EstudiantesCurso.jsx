import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getAllCourseStudents, getCourseGradebook, getStudentTracking } from '../services/api.js'
import { readAnalysisSettings } from '../services/analysisSettings.js'

function formatGrade(value) {
  return Number(value).toLocaleString('es-EC', { maximumFractionDigits: 2, useGrouping: false })
}

function formatScale(min, max) {
  return `${formatGrade(min)}–${formatGrade(max)}`
}

function scoreOverTen(note, column) {
  if (note?.nota == null) return null
  const min = Number(column.escalaMinima)
  const max = Number(column.escalaMaxima)
  const value = Number(note.nota)
  if (!Number.isFinite(value) || !Number.isFinite(min) || !Number.isFinite(max) || max === min) return null
  return (value - min) * 10 / (max - min)
}

function gradedCount(columns, notes) {
  return columns.filter((column) => notes.get(column.itemId)?.nota != null).length
}

function csvCell(value) {
  const text = String(value ?? '')
  return /[;"\n]/.test(text) ? `"${text.replaceAll('"', '""')}"` : text
}

function downloadGradebook(courseId, columns, rows) {
  const header = ['Estudiante', 'Correo', ...columns.map((column) => `${column.item} (${formatScale(column.escalaMinima, column.escalaMaxima)})`), 'Con nota']
  const body = rows.map(({ student, notes }) => [
    student.nombre,
    student.email || '',
    ...columns.map((column) => {
      const note = notes.get(column.itemId)
      return note?.nota == null ? '' : formatGrade(note.nota)
    }),
    `${gradedCount(columns, notes)}/${columns.length}`,
  ])
  const csv = [header, ...body].map((line) => line.map(csvCell).join(';')).join('\r\n')
  const url = URL.createObjectURL(new Blob([`\uFEFF${csv}`], { type: 'text/csv;charset=utf-8' }))
  const link = document.createElement('a')
  link.href = url
  link.download = `calificaciones-curso-${courseId}.csv`
  link.click()
  URL.revokeObjectURL(url)
}

function EstudiantesCurso() {
  const { id } = useParams()
  const [gradebook, setGradebook] = useState(null)
  const [query, setQuery] = useState('')
  const gradebookLoading = gradebook?.courseId !== id
  const gradeColumns = gradebookLoading ? [] : gradebook.columns
  const gradeRows = gradebookLoading ? [] : gradebook.rows
  const gradebookError = gradebookLoading ? '' : gradebook.error

  useEffect(() => {
    let active = true

    async function loadFromTracking() {
      const enrolled = await getAllCourseStudents(id)
      const trackings = await Promise.all(enrolled.map(async (student) => ({
        student,
        tracking: await getStudentTracking(id, student.moodleId),
      })))
      const columns = []
      const seen = new Set()
      for (const { tracking } of trackings) {
        for (const note of tracking.notas ?? []) {
          if (seen.has(note.itemId)) continue
          seen.add(note.itemId)
          columns.push(note)
        }
      }
      columns.sort((left, right) => left.itemId - right.itemId)
      return {
        courseId: id,
        error: '',
        columns,
        rows: trackings.map(({ student, tracking }) => ({
          student,
          notes: new Map((tracking.notas ?? []).map((note) => [note.itemId, note])),
        })),
      }
    }

    getCourseGradebook(id)
      .then((book) => {
        if (!active) return
        setGradebook({
          courseId: id,
          error: '',
          columns: (book.actividades ?? []).map((activity) => ({
            itemId: activity.itemId,
            item: activity.nombre,
            escalaMinima: activity.escalaMinima,
            escalaMaxima: activity.escalaMaxima,
          })),
          rows: (book.estudiantes ?? []).map((student) => ({
            student: { moodleId: student.moodleId, nombre: student.nombre, email: student.email },
            notes: new Map((student.notas ?? []).map((note) => [note.itemId, note])),
          })),
        })
      })
      .catch(async (requestError) => {
        try {
          const fallback = await loadFromTracking()
          if (active) setGradebook(fallback)
        } catch {
          if (active) setGradebook({ courseId: id, error: requestError.message, columns: [], rows: [] })
        }
      })

    return () => { active = false }
  }, [id])

  useEffect(() => {
    if (gradebookLoading) return undefined
    const hash = window.location.hash
    if (!hash.startsWith('#actividad-')) return undefined
    const name = decodeURIComponent(hash.slice('#actividad-'.length))
    const header = document.querySelector(`[data-activity="${CSS.escape(name)}"]`)
    header?.scrollIntoView({ inline: 'center', block: 'center' })
    return undefined
  }, [gradebookLoading, gradeColumns, id])

  const threshold = readAnalysisSettings(id)?.umbral ?? 6
  const normalizedQuery = query.trim().toLowerCase()
  const visibleRows = normalizedQuery
    ? gradeRows.filter((row) => row.student.nombre.toLowerCase().includes(normalizedQuery))
    : gradeRows
  const columnCount = Math.max(gradeColumns.length + 2, 3)

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">SEGUIMIENTO INDIVIDUAL</span>
          <h1>Estudiantes matriculados</h1>
          <p className="muted">Nota de cada actividad en la escala de Moodle. «—» significa que no hay nota registrada; no es un cero. Las notas bajo el umbral del análisis ({threshold.toFixed(2)}/10) se marcan en color.</p>
        </div>
        <button
          className="button button--secondary"
          disabled={gradebookLoading || gradeRows.length === 0}
          onClick={() => downloadGradebook(id, gradeColumns, gradeRows)}
          type="button"
        >
          Descargar CSV
        </button>
      </div>

      {gradebookError && <p className="course-message course-message--error" role="alert">{gradebookError}</p>}

      <section className="content-card">
        <div className="card-heading card-heading--tools">
          <label>
            Buscar estudiante
            <input
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Nombre"
              type="search"
              value={query}
            />
          </label>
          <span className="count-pill">{gradebookLoading ? '…' : `${visibleRows.length} estudiantes`}</span>
        </div>
        <div className="table-scroll">
          <table className="gradebook">
            <thead>
              <tr>
                <th>Estudiante</th>
                {gradeColumns.map((column) => (
                  <th data-activity={column.item} key={column.itemId}>
                    {column.item}
                    <small>{formatScale(column.escalaMinima, column.escalaMaxima)}</small>
                  </th>
                ))}
                <th>Con nota</th>
              </tr>
            </thead>
            <tbody>
              {gradebookLoading && <tr><td colSpan={columnCount}>Cargando calificaciones…</td></tr>}
              {!gradebookLoading && visibleRows.map(({ student, notes }) => (
                <tr key={student.moodleId}>
                  <td>
                    <span className="student-cell">
                      <span>
                        <strong><Link to={`/cursos/${id}/estudiantes/${student.moodleId}`}>{student.nombre}</Link></strong>
                        <small>{student.email || 'Correo no disponible'}</small>
                      </span>
                    </span>
                  </td>
                  {gradeColumns.map((column) => {
                    const note = notes.get(column.itemId)
                    const score = scoreOverTen(note, column)
                    const belowThreshold = score != null && score < threshold
                    return (
                      <td className={belowThreshold ? 'gradebook__low' : undefined} key={column.itemId}>
                        {note?.nota == null ? '—' : formatGrade(note.nota)}
                      </td>
                    )
                  })}
                  <td>{gradedCount(gradeColumns, notes)}/{gradeColumns.length}</td>
                </tr>
              ))}
              {!gradebookLoading && !gradebookError && gradeRows.length === 0 && (
                <tr><td colSpan={columnCount}>No hay estudiantes matriculados en la última sincronización.</td></tr>
              )}
              {!gradebookLoading && gradeRows.length > 0 && visibleRows.length === 0 && (
                <tr><td colSpan={columnCount}>Ningún estudiante coincide con la búsqueda.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </>
  )
}

export default EstudiantesCurso
