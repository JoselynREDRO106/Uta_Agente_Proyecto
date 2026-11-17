import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import MetricCard from '../components/MetricCard.jsx'
import {
  createTeacherAccount,
  getCourseIndicators,
  getCourseTeachers,
  getCourses,
  getCurrentUser,
} from '../services/api.js'

function formatValue(value, suffix = '') {
  return value == null ? 'Sin datos' : `${value}${suffix}`
}

function DashboardCurso() {
  const { id } = useParams()
  const isAdministrator = getCurrentUser()?.rol === 'Administrador técnico'
  const [courseResult, setCourseResult] = useState(null)
  const [teachers, setTeachers] = useState([])
  const [teacherError, setTeacherError] = useState('')
  const [accountDraft, setAccountDraft] = useState({ email: '', password: '' })
  const [accountMessage, setAccountMessage] = useState('')
  const [creatingAccount, setCreatingAccount] = useState(false)
  const courseLoading = courseResult?.id !== id
  const course = courseLoading ? null : courseResult.course
  const indicators = courseLoading ? null : courseResult.indicators
  const error = courseLoading ? '' : courseResult.error

  useEffect(() => {
    let active = true
    Promise.all([getCourses(), getCourseIndicators(id)])
      .then(([courses, result]) => {
        if (active) {
          setCourseResult({
            id,
            course: courses.find((item) => String(item.id) === id) ?? null,
            indicators: result,
            error: '',
          })
        }
      })
      .catch((requestError) => {
        if (active) {
          setCourseResult({ id, course: null, indicators: null, error: requestError.message })
        }
      })

    return () => { active = false }
  }, [id])

  useEffect(() => {
    let active = true
    getCourseTeachers(id)
      .then((result) => {
        if (active) {
          setTeachers(result)
          setTeacherError('')
        }
      })
      .catch((requestError) => {
        if (active) {
          setTeachers([])
          setTeacherError(requestError.message)
        }
      })
    return () => { active = false }
  }, [id])

  async function createAccount(teacher) {
    setCreatingAccount(true)
    setAccountMessage('')
    try {
      await createTeacherAccount({
        nombre: teacher.nombre,
        email: teacher.email,
        password: accountDraft.password,
        cursosMoodle: [Number(id)],
      })
      setAccountMessage(`Cuenta docente creada para ${teacher.nombre}. Debe iniciar sesión de nuevo para ver el curso.`)
      setAccountDraft({ email: '', password: '' })
    } catch (requestError) {
      setAccountMessage(requestError.message)
    } finally {
      setCreatingAccount(false)
    }
  }

  const average = indicators?.promedio
  const progress = indicators?.progresoPorcentaje
  const lastSync = indicators?.ultimaSincronizacionUtc
    ? new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeStyle: 'short' })
      .format(new Date(indicators.ultimaSincronizacionUtc))
    : 'Sin sincronización registrada'

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">RESUMEN DEL CURSO</span>
          <h1>{course?.name ?? (courseLoading ? 'Cargando curso…' : 'Curso')}</h1>
          <p className="muted">Fuente: Moodle · Última sincronización: {lastSync}</p>
        </div>
      </div>

      {error && <p className="course-message course-message--error" role="alert">{error}</p>}

      <section aria-label="Indicadores del curso" className="metric-grid">
        <MetricCard
          detail={average == null ? 'No existen calificaciones con escala disponible' : `Escala normalizada de 0 a ${indicators.notaMaxima}`}
          label="Promedio general"
          tone="blue"
          value={formatValue(average == null ? null : Number(average).toFixed(2), average == null ? '' : ' / 100')}
        />
        <MetricCard
          detail="Matrículas sincronizadas"
          label="Estudiantes"
          tone="violet"
          value={formatValue(indicators?.estudiantes)}
        />
        <MetricCard
          detail="Notas calificadas sobre el total de registros posibles"
          label="Progreso"
          tone="green"
          value={formatValue(progress, progress == null ? '' : '%')}
        />
      </section>

      {indicators && !indicators.datosDisponibles && (
        <p className="course-message course-message--warning" role="status">
          No hay datos suficientes con escala de calificación para calcular el promedio. No se asigna cero a notas ausentes.
        </p>
      )}
      {indicators?.estadoSincronizacion === 'Incompleta' && (
        <p className="course-message course-message--warning" role="status">
          La última sincronización quedó incompleta. Algunos indicadores pueden no estar disponibles.
        </p>
      )}

      <section className="content-card indicator-distribution">
        <div className="card-heading">
          <div>
            <h2>Distribución de calificaciones</h2>
            <p className="muted">Porcentaje normalizado sobre 100; solo incluye notas calificadas.</p>
          </div>
          <span className="count-pill">{indicators?.actividadesEvaluadas ?? 0} / {indicators?.actividadesTotales ?? 0} actividades evaluadas</span>
        </div>
        <div className="distribution-grid">
          {(indicators?.distribucion ?? []).map((band) => (
            <div className="distribution-item" key={band.rango}>
              <span>{band.rango}</span>
              <strong>{band.cantidad}</strong>
              <small>calificaciones</small>
            </div>
          ))}
          {!indicators?.distribucion?.length && <p className="muted">Sin datos para mostrar.</p>}
        </div>
      </section>

      <section className="content-card">
        <div className="card-heading">
          <div>
            <h2>Docentes sincronizados</h2>
            <p className="muted">Salen de la matrícula de Moodle. La cuenta de acceso se crea aparte, con rol Docente.</p>
          </div>
          <span className="count-pill">{teachers.length} docentes</span>
        </div>
        {teacherError && <p className="course-message course-message--error" role="alert">{teacherError}</p>}
        {!teacherError && teachers.length === 0 && (
          <p className="muted">Este curso todavía no tiene docentes en la última sincronización. Vuelve a sincronizar el ID de Moodle.</p>
        )}
        {teachers.length > 0 && (
          <div className="table-scroll">
            <table>
              <thead>
                <tr><th>Nombre</th><th>Correo</th><th>Rol en Moodle</th>{isAdministrator && <th>Cuenta</th>}</tr>
              </thead>
              <tbody>
                {teachers.map((teacher) => (
                  <tr key={teacher.id}>
                    <td>{teacher.nombre}</td>
                    <td>{teacher.email || 'Sin correo'}</td>
                    <td>{teacher.rolMoodle}</td>
                    {isAdministrator && (
                      <td>
                        <button
                          className="button button--secondary"
                          disabled={creatingAccount || !teacher.email || accountDraft.email !== teacher.email || accountDraft.password.length < 12}
                          onClick={() => createAccount(teacher)}
                          type="button"
                        >
                          Crear cuenta
                        </button>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        {isAdministrator && teachers.length > 0 && (
          <form className="feature-form" onSubmit={(event) => event.preventDefault()}>
            <label htmlFor="teacher-account-email">Correo del docente</label>
            <select
              id="teacher-account-email"
              onChange={(event) => setAccountDraft((draft) => ({ ...draft, email: event.target.value }))}
              value={accountDraft.email}
            >
              <option value="">Selecciona un docente</option>
              {teachers.filter((teacher) => teacher.email).map((teacher) => (
                <option key={teacher.id} value={teacher.email}>{teacher.nombre}</option>
              ))}
            </select>
            <label htmlFor="teacher-account-password">Contraseña inicial (mínimo 12 caracteres)</label>
            <input
              id="teacher-account-password"
              minLength={12}
              onChange={(event) => setAccountDraft((draft) => ({ ...draft, password: event.target.value }))}
              type="password"
              value={accountDraft.password}
            />
            {accountMessage && <p className="course-message" role="status">{accountMessage}</p>}
          </form>
        )}
      </section>

      <div className="course-dashboard-shortcuts">
        <Link className="button button--secondary" to={`/cursos/${id}/estudiantes`}>Ver estudiantes</Link>
        <Link className="button button--secondary" to={`/cursos/${id}/alertas`}>Revisar alertas</Link>
        <Link className="button button--secondary" to={`/cursos/${id}/informes`}>Consultar informes</Link>
      </div>
    </>
  )
}

export default DashboardCurso
