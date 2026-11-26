import { studentGrades, studyPlanText } from './gradeRules.js'

const storageKey = (courseId) => `edumonitor-study-plans-trial-${courseId}`

export function readStudyPlans(courseId) {
  try {
    const saved = JSON.parse(sessionStorage.getItem(storageKey(courseId)) || '')
    return Array.isArray(saved) ? saved : []
  } catch {
    return []
  }
}

export function writeStudyPlans(courseId, plans) {
  sessionStorage.setItem(storageKey(courseId), JSON.stringify(plans))
}

export function draftsFromGradebook({ courseName, columns, rows, threshold }) {
  return rows.flatMap((row) => {
    const grades = studentGrades(columns, rows, row.student.moodleId)
    const weak = grades.some((grade) => grade.score != null && grade.score < threshold)
    if (!weak) return []
    return [{
      id: `plan-${row.student.moodleId}`,
      estudianteId: row.student.moodleId,
      estudiante: row.student.nombre,
      email: row.student.email || '',
      estado: 'Borrador',
      texto: studyPlanText({
        studentName: row.student.nombre,
        courseName,
        grades,
        threshold,
      }),
      notificadoEn: null,
      destino: '',
      asunto: '',
    }]
  })
}

export function mergePlans(saved, drafts) {
  const previous = new Map(saved.map((plan) => [plan.id, plan]))
  return drafts.map((draft) => {
    const kept = previous.get(draft.id)
    if (!kept) return draft
    const locked = kept.estado === 'En revisión' || kept.estado === 'Notificado'
    return {
      ...draft,
      estado: kept.estado,
      texto: locked ? kept.texto : draft.texto,
      notificadoEn: kept.notificadoEn ?? null,
      destino: kept.destino || '',
      asunto: kept.asunto || '',
    }
  })
}

export function saveReview(plans, id, texto) {
  const clean = texto.trim()
  if (!clean) return { plans, error: 'El plan no puede quedar vacío.' }
  const found = plans.some((plan) => plan.id === id)
  if (!found) return { plans, error: 'No está ese plan.' }
  return {
    error: '',
    plans: plans.map((plan) => (
      plan.id === id && plan.estado !== 'Notificado'
        ? { ...plan, texto: clean, estado: 'En revisión' }
        : plan
    )),
  }
}

export function approveAndNotify(plans, id, now = new Date().toISOString()) {
  const plan = plans.find((item) => item.id === id)
  if (!plan) return { plans, error: 'No está ese plan.' }
  if (plan.estado === 'Notificado') return { plans, error: 'Ese plan ya fue notificado.' }
  if (!plan.email) return { plans, error: 'Moodle no tiene correo para este estudiante.' }
  if (!plan.texto.trim()) return { plans, error: 'El plan está vacío.' }
  const asunto = 'Plan de estudio'
  return {
    error: '',
    plans: plans.map((item) => (
      item.id === id
        ? { ...item, estado: 'Notificado', notificadoEn: now, destino: item.email, asunto }
        : item
    )),
  }
}
