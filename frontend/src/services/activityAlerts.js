import { getAllCourseStudents, getCourseGradebook, getStudentTracking } from './api.js'

export {
  buildActivityAlerts,
  mailtoPlan,
  scoreOverTen,
  studentGrades,
  studyPlanText,
} from './gradeRules.js'

const storageKey = (courseId) => `edumonitor-activity-alerts-${courseId}`

export function readActivityAlertStore(courseId) {
  try {
    const saved = JSON.parse(sessionStorage.getItem(storageKey(courseId)) || '')
    const threshold = Number(saved.threshold)
    return {
      threshold: threshold > 0 && threshold <= 10 ? threshold : 7,
      states: saved.states && typeof saved.states === 'object' ? saved.states : {},
    }
  } catch {
    return { threshold: 7, states: {} }
  }
}

export function writeActivityAlertStore(courseId, store) {
  sessionStorage.setItem(storageKey(courseId), JSON.stringify({
    threshold: store.threshold,
    states: store.states,
  }))
}

export async function loadGradeColumns(courseId) {
  try {
    const book = await getCourseGradebook(courseId)
    if (Array.isArray(book?.actividades) && Array.isArray(book?.estudiantes)) {
      return {
        columns: book.actividades.map((activity) => ({
          itemId: activity.itemId,
          item: activity.nombre,
          escalaMinima: activity.escalaMinima,
          escalaMaxima: activity.escalaMaxima,
        })),
        rows: book.estudiantes.map((student) => ({
          student: { moodleId: student.moodleId, nombre: student.nombre, email: student.email || '' },
          notes: new Map((student.notas ?? []).map((note) => [note.itemId, note])),
        })),
      }
    }
  } catch {
    // El libro único entra al reiniciar Análisis y Middleware.
  }

  const enrolled = await getAllCourseStudents(courseId)
  const trackings = await Promise.all(enrolled.map(async (student) => ({
    student,
    tracking: await getStudentTracking(courseId, student.moodleId),
  })))
  const columns = []
  const seen = new Set()
  for (const { tracking } of trackings) {
    for (const note of tracking.notas ?? []) {
      if (seen.has(note.itemId)) continue
      seen.add(note.itemId)
      columns.push({
        itemId: note.itemId,
        item: note.item,
        escalaMinima: note.escalaMinima,
        escalaMaxima: note.escalaMaxima,
      })
    }
  }
  columns.sort((left, right) => left.itemId - right.itemId)
  return {
    columns,
    rows: trackings.map(({ student, tracking }) => ({
      student: { moodleId: student.moodleId, nombre: student.nombre, email: student.email || '' },
      notes: new Map((tracking.notas ?? []).map((note) => [note.itemId, note])),
    })),
  }
}
