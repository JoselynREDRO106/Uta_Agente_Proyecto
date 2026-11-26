export function scoreOverTen(nota, min, max) {
  if (nota == null || nota === '') return null
  const value = Number(nota)
  const minimum = Number(min)
  const maximum = Number(max)
  if (!Number.isFinite(value) || !Number.isFinite(minimum) || !Number.isFinite(maximum) || maximum === minimum) {
    return null
  }
  return (value - minimum) * 10 / (maximum - minimum)
}

export function buildActivityAlerts(columns, rows, store) {
  const alerts = []
  for (const column of columns) {
    for (const row of rows) {
      const note = row.notes.get(column.itemId)
      const score = scoreOverTen(note?.nota, column.escalaMinima, column.escalaMaxima)
      if (score == null || score >= store.threshold) continue
      const key = `${row.student.moodleId}-${column.itemId}`
      alerts.push({
        key,
        estudianteId: row.student.moodleId,
        estudiante: row.student.nombre,
        itemId: column.itemId,
        actividad: column.item,
        nota: note.nota,
        score,
        estado: store.states[key] || 'Nueva',
      })
    }
  }
  return alerts.sort((left, right) => left.actividad.localeCompare(right.actividad, 'es')
    || left.estudiante.localeCompare(right.estudiante, 'es'))
}

export function mailtoPlan(email, subject, body) {
  return `mailto:${email}?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`
}

export function studentGrades(columns, rows, studentId) {
  const row = rows.find((item) => String(item.student.moodleId) === String(studentId))
  if (!row) return []
  return columns.map((column) => {
    const note = row.notes.get(column.itemId)
    return {
      actividad: column.item,
      nota: note?.nota ?? null,
      score: scoreOverTen(note?.nota, column.escalaMinima, column.escalaMaxima),
    }
  })
}

export function studyPlanText({ studentName, courseName, focusActivity, grades, threshold = 7 }) {
  const weak = grades.filter((grade) => grade.score != null && grade.score < threshold)
  const focus = weak.find((grade) => grade.actividad === focusActivity) ?? weak[0]
  const format = (value) => Number(value).toLocaleString('es-EC', { maximumFractionDigits: 2, useGrouping: false })
  const steps = focus
    ? [
      `1. Abre en Moodle la actividad «${focus.actividad}» y revisa el enunciado y la retroalimentación.`,
      `2. Tu nota actual es ${format(focus.nota)} sobre 10. El corte para reforzar es ${threshold}/10.`,
      '3. Vuelve a resolver los ejercicios de esa unidad sin mirar la solución.',
      '4. Compara el resultado con la rúbrica y anota la parte que falló.',
      '5. Si la duda continúa, pregunta en el foro del curso o en la siguiente clase.',
    ]
    : ['No hay actividades con nota menor al corte para armar el plan.']
  const others = weak
    .filter((grade) => grade !== focus)
    .slice(0, 4)
    .map((grade) => `- ${grade.actividad}: ${format(grade.nota)}/10`)
  return [
    `Hola ${studentName}.`,
    courseName ? `Curso: ${courseName}.` : '',
    focus ? `Conviene mejorar «${focus.actividad}».` : '',
    'Plan de estudio:',
    ...steps,
    others.length > 0 ? 'Otras actividades por debajo del corte:' : '',
    ...others,
  ].filter(Boolean).join('\n')
}
