import { useEffect, useState } from 'react'
import {
  getMoodleConfiguration,
  saveMoodleConfiguration,
  verifyMoodleConnection,
} from '../services/api.js'

function ConfiguracionMoodle() {
  const [baseUrl, setBaseUrl] = useState('')
  const [token, setToken] = useState('')
  const [timeoutSeconds, setTimeoutSeconds] = useState(60)
  const [isConfigured, setIsConfigured] = useState(false)
  const [updatedAtUtc, setUpdatedAtUtc] = useState(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [checking, setChecking] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  useEffect(() => {
    let active = true
    getMoodleConfiguration()
      .then((configuration) => {
        if (!active) return
        setBaseUrl(configuration.baseUrl ?? '')
        setTimeoutSeconds(configuration.timeoutSeconds ?? 60)
        setIsConfigured(Boolean(configuration.isConfigured))
        setUpdatedAtUtc(configuration.updatedAtUtc ?? null)
      })
      .catch((loadError) => {
        if (active) setError(loadError.message)
      })
      .finally(() => {
        if (active) setLoading(false)
      })

    return () => {
      active = false
    }
  }, [])

  async function handleSave(event) {
    event.preventDefault()
    setError('')
    setMessage('')
    setSaving(true)

    try {
      await saveMoodleConfiguration({ baseUrl, token, timeoutSeconds: Number(timeoutSeconds) })
      setToken('')
      setIsConfigured(true)
      setUpdatedAtUtc(new Date().toISOString())
      setMessage('Configuración guardada. El token no se volverá a mostrar.')
    } catch (saveError) {
      setError(saveError.message)
    } finally {
      setSaving(false)
    }
  }

  async function handleVerify() {
    setError('')
    setMessage('')
    setChecking(true)
    try {
      const result = await verifyMoodleConnection()
      setMessage(`Conexión correcta con ${result.siteName} (${result.elapsedMilliseconds} ms).`)
    } catch (verifyError) {
      setError(verifyError.message)
    } finally {
      setChecking(false)
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ADMINISTRACIÓN</span>
          <h1>Conexión con Moodle</h1>
          <p className="muted">La configuración se guarda en el servicio independiente de Integración Moodle.</p>
        </div>
      </div>

      <section aria-labelledby="moodle-config-title" className="content-card moodle-config">
        <div className="card-heading">
          <div>
            <h2 id="moodle-config-title">Credenciales del servicio</h2>
            <p className="muted">El token se cifra al guardarse y nunca se devuelve a esta pantalla.</p>
          </div>
          <span className={`count-pill${isConfigured ? '' : ' count-pill--muted'}`}>
            {isConfigured ? 'Configurado' : 'Pendiente'}
          </span>
        </div>

        {loading ? <p className="muted">Cargando configuración…</p> : (
          <form className="moodle-config__form" onSubmit={handleSave}>
            <label htmlFor="moodle-base-url">URL base de Moodle</label>
            <input
              autoComplete="url"
              id="moodle-base-url"
              onChange={(event) => setBaseUrl(event.target.value)}
              placeholder="https://moodle.example.edu"
              required
              type="url"
              value={baseUrl}
            />

            <label htmlFor="moodle-token">Token de servicio web</label>
            <input
              autoComplete="new-password"
              id="moodle-token"
              onChange={(event) => setToken(event.target.value)}
              placeholder={isConfigured ? 'Dejar vacío para conservar el token actual' : 'Ingresa el token'}
              type="password"
              value={token}
            />
            <p className="muted">Por seguridad, el valor guardado no es visible. Si no escribes uno, se conserva.</p>

            <label htmlFor="moodle-timeout">Tiempo de espera (segundos)</label>
            <input
              id="moodle-timeout"
              max="120"
              min="5"
              onChange={(event) => setTimeoutSeconds(event.target.value)}
              required
              type="number"
              value={timeoutSeconds}
            />

            <div className="moodle-config__actions">
              <button className="button button--primary" disabled={saving || loading} type="submit">
                {saving ? 'Guardando…' : 'Guardar configuración'}
              </button>
              <button
                className="button button--secondary"
                disabled={!isConfigured || checking || saving}
                onClick={handleVerify}
                type="button"
              >
                {checking ? 'Verificando…' : 'Verificar conexión'}
              </button>
            </div>
          </form>
        )}

        {updatedAtUtc && (
          <p className="page-footnote">
            Última modificación: {new Date(updatedAtUtc).toLocaleString()}
          </p>
        )}
        {error && <p className="course-message course-message--error" role="alert">{error}</p>}
        {message && <p className="course-message course-message--success" role="status">{message}</p>}
      </section>
    </>
  )
}

export default ConfiguracionMoodle
