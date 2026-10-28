const apiUrl = import.meta.env.VITE_API_URL

export async function getHealth() {
  if (!apiUrl) {
    throw new Error('Configura VITE_API_URL en frontend/.env para conectar con el middleware.')
  }

  const response = await fetch(`${apiUrl.replace(/\/$/, '')}/health`)

  if (!response.ok) {
    throw new Error(`El middleware respondió con HTTP ${response.status}.`)
  }

  return response.json()
}
