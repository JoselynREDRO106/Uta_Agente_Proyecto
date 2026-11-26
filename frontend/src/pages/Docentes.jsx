import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { createTeacherAccount, getCourseTeachers, getCourses } from '../services/api.js'

function Docentes() {
  const [searchParams] = useSearchParams()
  const [courses, setCourses] = useState([])
  const [coursesError, setCoursesError] = useState('')
  const [selectedCourseId, setSelectedCourseId] = useState(searchParams.get('curso') ?? '')
  const [teacherResult, setTeacherResult] = useState(null)
  const [teacherEmail, setTeacherEmail] = useState('')
  const [teacherPassword, setTeacherPassword] = useState('')
  const [accountMessage, setAccountMessage] = useState('')
  const [accountError, setAccountError] = useState('')
  const [creatingAccount, setCreatingAccount] = useState(false)

  useEffect(() => {
    let active = true
    getCourses()
      .then((result) => {
        if (active) setCourses(result)
      })
      .catch((error) => {
        if (active) setCoursesError(error.message)
      })
    return () => { active = false }
  }, [])

  useEffect(() => {
    if (!selectedCourseId) return undefined
    let active = true
    getCourseTeachers(selectedCourseId)
      .then((result) => {
        if (active) setTeacherResult({ id: selectedCourseId, teachers: result, error: '' })
      })
      .catch((error) => {
        if (active) setTeacherResult({ id: selectedCourseId, teachers: [], error: error.message })
      })
    return () => { active = false }
  }, [selectedCourseId])

  const teachersMatch = teacherResult?.id === selectedCourseId
  const teachers = selectedCourseId && teachersMatch ? teacherResult.teachers : []
  const teacherError = selectedCourseId && teachersMatch ? teacherResult.error : ''
  const teachersLoading = Boolean(selectedCourseId) && !teachersMatch

  function selectCourse(courseId) {
    setSelectedCourseId(courseId)
    setTeacherEmail('')
  }

  async function handleCreateTeacher(event) {
    event.preventDefault()
    const teacher = teachers.find((item) => item.email === teacherEmail)
    if (!teacher) return
    setCreatingAccount(true)
    setAccountMessage('')
    setAccountError('')
    try {
      await createTeacherAccount({
        nombre: teacher.nombre,
        email: teacher.email,
        password: teacherPassword,
        cursosMoodle: [Number(selectedCourseId)],
      })
      setAccountMessage(`Cuenta creada para ${teacher.nombre}. Ese docente debe iniciar sesión para ver el curso.`)
      setTeacherPassword('')
    } catch (error) {
      setAccountError(error.message)
    } finally {
      setCreatingAccount(false)
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ADMINISTRACIÓN</span>
          <h1>Cuentas docentes</h1>
          <p className="muted">Crea el acceso de un docente que ya aparece en la matrícula sincronizada de Moodle.</p>
        </div>
      </div>

      {coursesError && <p className="course-message course-message--error" role="alert">{coursesError}</p>}

      <section className="content-card admin-panel">
        <div className="card-heading">
          <div>
            <h2>Nueva cuenta</h2>
            <p className="muted">El docente tiene que existir en el curso. Si no aparece, sincroniza el curso primero.</p>
          </div>
        </div>
        <form className="feature-form admin-form" onSubmit={handleCreateTeacher}>
          <label htmlFor="teacher-course">Curso sincronizado</label>
          <select
            id="teacher-course"
            onChange={(event) => selectCourse(event.target.value)}
            value={selectedCourseId}
          >
            <option value="">Selecciona un curso</option>
            {courses.map((course) => (
              <option key={course.id} value={course.id}>{course.name} ({course.id})</option>
            ))}
          </select>
          <label htmlFor="teacher-email">Docente en Moodle</label>
          <select
            id="teacher-email"
            disabled={!selectedCourseId || teachersLoading || teachers.length === 0}
            onChange={(event) => setTeacherEmail(event.target.value)}
            value={teacherEmail}
          >
            <option value="">{teachersLoading ? 'Buscando docentes…' : 'Selecciona un docente'}</option>
            {teachers.filter((teacher) => teacher.email).map((teacher) => (
              <option key={teacher.id} value={teacher.email}>{teacher.nombre} · {teacher.email}</option>
            ))}
          </select>
          <label htmlFor="teacher-password">Contraseña inicial (mínimo 12 caracteres)</label>
          <input
            id="teacher-password"
            minLength={12}
            onChange={(event) => setTeacherPassword(event.target.value)}
            type="password"
            value={teacherPassword}
          />
          <div className="feature-form__footer">
            <button
              className="button button--primary"
              disabled={creatingAccount || !teacherEmail || teacherPassword.length < 12}
              type="submit"
            >
              {creatingAccount ? 'Creando cuenta…' : 'Crear cuenta docente'}
            </button>
          </div>
          {teacherError && <p className="course-message course-message--error" role="alert">{teacherError}</p>}
          {!teacherError && selectedCourseId && !teachersLoading && teachers.length === 0 && (
            <p className="muted">Ese curso no tiene docentes en la última sincronización.</p>
          )}
          {accountError && <p className="course-message course-message--error" role="alert">{accountError}</p>}
          {accountMessage && <p className="course-message course-message--success" role="status">{accountMessage}</p>}
        </form>
      </section>
    </>
  )
}

export default Docentes
