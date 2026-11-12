const apiUrl = import.meta.env.VITE_API_URL
const identityUrl = import.meta.env.VITE_IDENTITY_API_URL
const moodleIntegrationUrl = import.meta.env.VITE_MOODLE_INTEGRATION_URL || 'http://localhost:5010'
const accessTokenKey = 'edumonitor.accessToken'
const userKey = 'edumonitor.user'

function getApiUrl() {
  if (!apiUrl) {
    throw new Error('Configura VITE_API_URL en frontend/.env para conectar con la API.')
  }

  return apiUrl.replace(/\/$/, '')
}

async function requestJson(path, options, baseUrl = getApiUrl()) {
  const headers = new Headers(options?.headers)
  const accessToken = sessionStorage.getItem(accessTokenKey)
  if (accessToken) {
    headers.set('Authorization', `Bearer ${accessToken}`)
  }

  const response = await fetch(`${baseUrl.replace(/\/$/, '')}${path}`, { ...options, headers })
  const body = await response.text()
  let result = null

  if (body) {
    try {
      result = JSON.parse(body)
    } catch {
      if (response.ok) {
        throw new Error('La API devolvió una respuesta JSON inválida.')
      }

      result = body
    }
  }

  if (!response.ok) {
    const message = typeof result === 'string'
      ? result
      : result?.detail ?? result?.title ?? `La API respondió con HTTP ${response.status}.`
    throw new Error(message)
  }

  return result
}

export async function getHealth() {
  return requestJson('/health')
}

export async function getCourses() {
  const courses = await requestJson('/api/cursos')

  if (!Array.isArray(courses)) {
    throw new Error('La API devolvió un listado de cursos inválido.')
  }

  return courses
}

export async function syncMoodleCourse(courseId) {
  return requestJson(`/api/cursos/sincronizar/${encodeURIComponent(courseId)}`, {
    method: 'POST',
  })
}

export async function getSyncHistory(limit = 50) {
  const history = await requestJson(`/api/cursos/sincronizaciones?limit=${encodeURIComponent(limit)}`)
  if (!Array.isArray(history)) {
    throw new Error('La API devolvió un historial de sincronización inválido.')
  }

  return history
}

export async function login(email, password) {
  const result = await requestJson('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  }, identityUrl || getApiUrl())

  sessionStorage.setItem(accessTokenKey, result.accessToken)
  sessionStorage.setItem(userKey, JSON.stringify({ nombre: result.nombre, rol: result.rol }))
  return result
}

export function logout() {
  sessionStorage.removeItem(accessTokenKey)
  sessionStorage.removeItem(userKey)
}

export function getCurrentUser() {
  const value = sessionStorage.getItem(userKey)
  return value ? JSON.parse(value) : null
}

export function hasSession() {
  return Boolean(sessionStorage.getItem(accessTokenKey))
}

export async function getCourseStudents(courseId, { search = '', status = '', page = 1, pageSize = 10 } = {}) {
  const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
  if (search.trim()) query.set('search', search.trim())
  if (status) query.set('status', status)
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/estudiantes?${query}`)
}

export async function getStudentTracking(courseId, studentId) {
  return requestJson(
    `/api/cursos/${encodeURIComponent(courseId)}/estudiantes/${encodeURIComponent(studentId)}/seguimiento`,
  )
}

export async function getCourseIndicators(courseId) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/indicadores`)
}

export async function getGroupAnalysis(courseId) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/analisis-grupal?umbral=6&minimoCalificaciones=7`)
}

export async function askCourseAssistant(courseId, pregunta) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/consultar`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ pregunta }),
  })
}

export async function getCourseRecommendations(courseId) {
  const recommendations = await requestJson(`/api/cursos/${encodeURIComponent(courseId)}/recomendaciones`)
  if (!Array.isArray(recommendations)) {
    throw new Error('La API devolvió una lista de recomendaciones inválida.')
  }

  return recommendations
}

export async function generateCourseRecommendations(courseId) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/recomendaciones/generar`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ umbral: 6, minimoCalificaciones: 7 }),
  })
}

export async function updateRecommendation(courseId, recommendationId, estado, texto) {
  return requestJson(
    `/api/cursos/${encodeURIComponent(courseId)}/recomendaciones/${encodeURIComponent(recommendationId)}/estado`,
    {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ estado, texto }),
    },
  )
}

export async function getAlertRules(courseId) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/alertas/reglas`)
}

export async function saveAlertRules(courseId, reglas) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/alertas/reglas`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ reglas }),
  })
}

export async function getCourseAlerts(courseId, estado = '') {
  const query = estado ? `?estado=${encodeURIComponent(estado)}` : ''
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/alertas${query}`)
}

export async function updateAlertState(courseId, studentId, tipo, estado) {
  return requestJson(
    `/api/cursos/${encodeURIComponent(courseId)}/alertas/${encodeURIComponent(studentId)}/${encodeURIComponent(tipo)}/estado`,
    {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ estado }),
    },
  )
}

export async function getAuditLogs({
  tipo = '',
  desdeUtc = '',
  hastaUtc = '',
  pagina = 1,
  tamanoPagina = 20,
} = {}) {
  const query = new URLSearchParams({ pagina: String(pagina), tamanoPagina: String(tamanoPagina) })
  if (tipo) query.set('tipo', tipo)
  if (desdeUtc) query.set('desdeUtc', desdeUtc)
  if (hastaUtc) query.set('hastaUtc', hastaUtc)
  return requestJson(`/api/auditoria?${query}`)
}

export async function getMoodleConfiguration() {
  return requestJson('/api/moodle/config', undefined, moodleIntegrationUrl)
}

export async function saveMoodleConfiguration({ baseUrl, token, timeoutSeconds }) {
  return requestJson('/api/moodle/config', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ baseUrl, token, timeoutSeconds }),
  }, moodleIntegrationUrl)
}

export async function verifyMoodleConnection() {
  return requestJson('/api/moodle/connection-test', { method: 'POST' }, moodleIntegrationUrl)
}
