const apiUrl = import.meta.env.VITE_API_URL

function getApiUrl() {
  if (!apiUrl) {
    throw new Error('Configura VITE_API_URL en frontend/.env para conectar con la API.')
  }

  return apiUrl.replace(/\/$/, '')
}

async function requestJson(path, options) {
  const response = await fetch(`${getApiUrl()}${path}`, options)
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
