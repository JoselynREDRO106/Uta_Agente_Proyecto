const rangeLabels = new Map([
  ['0–59', '0–5.9'],
  ['0-59', '0–5.9'],
  ['60–69', '6–6.9'],
  ['60-69', '6–6.9'],
  ['70–84', '7–8.4'],
  ['70-84', '7–8.4'],
  ['85–100', '8.5–10'],
  ['85-100', '8.5–10'],
  ['>100', '>10'],
])

function classify(question) {
  const text = question.toLowerCase()
  if (['correo', 'nombre del estudiante', 'predice', 'predicción', 'prediccion', 'por qué', 'porque'].some((word) => text.includes(word))) {
    return 'unsupported'
  }

  const rules = [
    ['low_activities', ['bajo el umbral', 'bajo umbral', 'más baja', 'mas baja', 'más bajas', 'mas bajas', 'reprob']],
    ['trend', ['tendencia', 'evolución', 'evolucion', 'por actividad', 'por fecha']],
    ['distribution', ['distribución', 'distribucion', 'rango', 'banda']],
    ['progress', ['progreso', 'avance']],
    ['sync_status', ['sincroniz']],
    ['average', ['promedio', 'media', 'nota general']],
    ['student_count', ['estudiantes', 'estudiante']],
    ['activities', ['actividad', 'tarea', 'evaluación', 'evaluacion']],
    ['summary', ['resumen', 'cómo va', 'como va']],
  ]
  return rules.find(([, words]) => words.some((word) => text.includes(word)))?.[0] ?? 'summary'
}

function scoreOverTen(value, maximum = 100) {
  const number = Number(value)
  const scale = Number(maximum)
  if (!Number.isFinite(number)) return null
  const divisor = Number.isFinite(scale) && scale !== 0 ? scale : 100
  return number * 10 / divisor
}

function formatScore(value) {
  return Number(value).toFixed(2)
}

function formatDelta(value) {
  const number = Number(value)
  const text = Math.abs(number).toFixed(2)
  if (number > 0) return `+${text}`
  if (number < 0) return `−${text}`
  return '0.00'
}

export function composeAcademicReply(question, indicators, groupAnalysis) {
  const intent = classify(question)
  const average = indicators?.datosDisponibles && indicators.promedio != null
    ? scoreOverTen(indicators.promedio, indicators.notaMaxima ?? 100)
    : null
  const activities = groupAnalysis?.actividades ?? []
  const trend = groupAnalysis?.tendencia ?? []
  const facts = []

  facts.push(average == null
    ? 'No hay calificaciones con escala válida para calcular el promedio.'
    : `Promedio del curso: ${formatScore(average)} sobre 10.`)
  facts.push(`Estudiantes registrados: ${indicators?.estudiantes ?? 0}.`)
  facts.push(`Actividades con calificaciones: ${indicators?.actividadesEvaluadas ?? 0} de ${indicators?.actividadesTotales ?? 0}.`)
  if (indicators?.progresoPorcentaje != null) {
    facts.push(`Progreso registrado: ${Number(indicators.progresoPorcentaje)}%.`)
  }
  if (indicators?.estadoSincronizacion) {
    facts.push(`Estado de sincronización: ${indicators.estadoSincronizacion}.`)
  }
  for (const band of indicators?.distribucion ?? []) {
    const label = rangeLabels.get(band.rango) ?? band.rango
    facts.push(`La banda ${label} contiene ${band.cantidad} registros de calificación.`)
  }

  const sufficientLow = activities.filter((item) => item.bajoUmbral && !item.datosInsuficientes)
  let respuesta
  if (intent === 'average') respuesta = facts[0]
  else if (intent === 'progress') respuesta = facts.find((item) => item.startsWith('Progreso')) ?? 'El progreso no está disponible.'
  else if (intent === 'activities') respuesta = `Se analizaron ${activities.length} actividades; la escala de comparación es de 0 a 10.`
  else if (intent === 'distribution') {
    const bands = facts.filter((item) => item.startsWith('La banda'))
    respuesta = bands.length > 0 ? bands.join(' ') : 'No hay distribución disponible.'
  } else if (intent === 'student_count') respuesta = facts[1]
  else if (intent === 'sync_status') {
    respuesta = indicators?.estadoSincronizacion
      ? `Estado de sincronización: ${indicators.estadoSincronizacion}.`
      : 'No hay estado de sincronización registrado.'
  } else if (intent === 'low_activities' && /m[aá]s baja(?!s)/i.test(question)) {
    const ranked = activities
      .filter((item) => item.promedio != null)
      .sort((left, right) => Number(left.promedio) - Number(right.promedio))
    respuesta = ranked.length > 0
      ? `La actividad con el promedio más bajo es ${ranked[0].nombre} (${formatScore(ranked[0].promedio)}/10)${ranked[0].datosInsuficientes ? ', con muestra corta' : ''}.`
      : 'No hay actividades con promedio para comparar.'
  } else if (intent === 'low_activities') {
    respuesta = sufficientLow.length > 0
      ? `Actividades con promedio bajo y muestra suficiente: ${sufficientLow.map((item) => `${item.nombre} (${formatScore(item.promedio)}/10)`).join(', ')}.`
      : 'No se identificaron actividades bajo el umbral con una muestra suficiente.'
  } else if (intent === 'trend') {
    const available = trend.filter((point) => point.promedioCurso != null)
    respuesta = available.length > 0
      ? `Tendencia por actividad: ${available.map((point) => {
        const name = point.actividad || point.fecha || 'actividad'
        const change = point.variacion == null ? '' : ` (${formatDelta(point.variacion)})`
        const sample = point.datosInsuficientes ? ', muestra corta' : ''
        return `${name}: ${formatScore(point.promedioCurso)}/10${change}${sample}`
      }).join('; ')}.`
      : 'No hay actividades con calificaciones para describir la tendencia.'
  } else if (intent === 'summary') respuesta = facts.slice(0, 5).join(' ')
  else {
    respuesta = 'Puedo informar indicadores agregados del curso, actividades con resultados bajos y la tendencia por actividad. No puedo consultar datos individuales, causas ni predicciones.'
  }

  const recomendaciones = []
  if (sufficientLow.length > 0 && (intent === 'low_activities' || intent === 'summary')) {
    recomendaciones.push('Considera revisar las actividades señaladas y ofrecer retroalimentación; los datos no determinan por sí solos la causa del resultado.')
  }
  if (indicators?.estadoSincronizacion === 'Incompleta') {
    recomendaciones.push('Verifica la sincronización incompleta antes de tomar decisiones.')
  }

  let incertidumbre = indicators?.estadoSincronizacion === 'Incompleta'
    ? 'La sincronización quedó incompleta; los datos pueden estar parciales.'
    : 'Los resultados son agregados; no muestran datos individuales ni explican causas.'
  if (!trend.some((point) => point.promedioCurso != null)) {
    incertidumbre += ' La tendencia no incluye actividades con calificaciones.'
  } else if (trend.some((point) => point.datosInsuficientes && point.promedioCurso != null)) {
    incertidumbre += ' Algunos puntos tienen una muestra menor al mínimo configurado.'
  }

  return { respuesta, hechos: facts.slice(0, 12), recomendaciones, incertidumbre }
}
