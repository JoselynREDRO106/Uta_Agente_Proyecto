const apiUrl = import.meta.env.VITE_API_URL
const accessTokenKey = 'edumonitor.accessToken'
const userKey = 'edumonitor.user'

function getApiUrl() {
  if (!apiUrl) {
    throw new Error('Configura VITE_API_URL en frontend/.env para conectar con la API.')
  }

  return apiUrl.replace(/\/$/, '')
}

async function requestJson(path, options) {
  const headers = new Headers(options?.headers)
  const accessToken = sessionStorage.getItem(accessTokenKey)
  if (accessToken) {
    headers.set('Authorization', `Bearer ${accessToken}`)
  }

  const response = await fetch(`${getApiUrl()}${path}`, { ...options, headers })
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

export async function login(email, password) {
  const result = await requestJson('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  })

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

export async function getCourseIndicators(courseId) {
  return requestJson(`/api/cursos/${encodeURIComponent(courseId)}/indicadores`)
}
