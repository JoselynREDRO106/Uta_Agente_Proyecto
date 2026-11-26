const keyFor = (courseId) => `edumonitor-analysis-${courseId}`

export function readAnalysisSettings(courseId) {
  try {
    const saved = JSON.parse(sessionStorage.getItem(keyFor(courseId)) || '')
    const umbral = Number(saved.umbral)
    const minimoCalificaciones = Number(saved.minimoCalificaciones)
    if (umbral > 0 && umbral <= 10 && Number.isInteger(minimoCalificaciones) && minimoCalificaciones >= 1) {
      return { umbral, minimoCalificaciones }
    }
  } catch {
    // La página usa el umbral por defecto cuando no hay una elección guardada.
  }
  return null
}

export function writeAnalysisSettings(courseId, settings) {
  sessionStorage.setItem(keyFor(courseId), JSON.stringify({
    umbral: settings.umbral,
    minimoCalificaciones: settings.minimoCalificaciones,
  }))
}
