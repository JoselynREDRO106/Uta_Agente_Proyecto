import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { login } from '../services/api.js'

function Login() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setLoading(true)
    try {
      await login(email, password)
      navigate('/cursos', { replace: true })
    } catch (requestError) {
      setError(requestError.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="login-page">
      <section className="login-aside">
        <div className="login-brand">
          <span className="brand__mark">E</span>
          <strong>EduMonitor<span> AI</span></strong>
        </div>
        <div className="login-message">
          <span className="eyebrow eyebrow--light">ANALÍTICA ACADÉMICA</span>
          <h1>Comprende el progreso.<br /><em>Impulsa el aprendizaje.</em></h1>
          <p>Un espacio para acompañar a cada estudiante con información clara y oportuna.</p>
        </div>
        <p className="login-aside__footer">UNIVERSIDAD TÉCNICA DE AMBATO</p>
      </section>

      <section className="login-form-panel">
        <form className="login-card" onSubmit={handleSubmit}>
          <span className="eyebrow">BIENVENIDO/A</span>
          <h2>Inicia sesión</h2>
          <p className="muted">Ingresa tus credenciales para continuar al panel.</p>

          <label className="field-label" htmlFor="email">Correo electrónico</label>
          <input
            autoComplete="email"
            className="text-input"
            id="email"
            name="email"
            onChange={(event) => setEmail(event.target.value)}
            placeholder="nombre@uta.edu.ec"
            required
            type="email"
            value={email}
          />

          <label className="field-label" htmlFor="password">Contraseña</label>
          <input
            autoComplete="current-password"
            className="text-input"
            id="password"
            name="password"
            placeholder="Ingresa tu contraseña"
            required
            type="password"
            onChange={(event) => setPassword(event.target.value)}
            value={password}
          />

          {error && <p className="course-message course-message--error" role="alert">{error}</p>}
          <button className="button button--primary button--wide" disabled={loading} type="submit">
            {loading ? 'Validando…' : 'Ingresar'} <span aria-hidden="true">→</span>
          </button>
          <p className="login-note">Acceso para cuentas habilitadas por el administrador técnico.</p>
        </form>
      </section>
    </main>
  )
}

export default Login
