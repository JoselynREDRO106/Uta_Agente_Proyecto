import { useEffect, useState } from 'react'
import { NavLink, Outlet, useMatch, useNavigate } from 'react-router-dom'
import { CourseMenu } from './CourseWorkspace.jsx'
import TopbarActions from '../components/TopbarActions.jsx'
import { getCourses, getCurrentUser, logout } from '../services/api.js'

function AppLayout() {
  const navigate = useNavigate()
  const user = getCurrentUser()
  const nestedCourse = useMatch('/cursos/:id/*')
  const exactCourse = useMatch('/cursos/:id')
  const courseId = nestedCourse?.params.id ?? exactCourse?.params.id
  const [courseLabel, setCourseLabel] = useState(null)
  const courseName = courseLabel?.courseId && courseLabel.courseId === courseId ? courseLabel.name : ''

  useEffect(() => {
    if (!courseId) return undefined
    let active = true
    getCourses()
      .then((courses) => {
        if (!active) return
        const match = courses.find((course) => String(course.id) === String(courseId))
        setCourseLabel({ courseId, name: match?.name ?? '' })
      })
      .catch(() => {
        if (active) setCourseLabel({ courseId, name: '' })
      })
    return () => { active = false }
  }, [courseId])

  function handleLogout() {
    logout()
    navigate('/login', { replace: true })
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <NavLink className="brand" to="/cursos" aria-label="EduMonitor AI, inicio">
          <span className="brand__mark">E</span>
          <span>
            <strong>EduMonitor</strong>
            <small>ACADEMIC INTELLIGENCE</small>
          </span>
        </NavLink>

        <p className="sidebar__caption">ESPACIO DE TRABAJO</p>
        <nav className="sidebar__nav" aria-label="Navegación principal">
          <NavLink end to="/cursos" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
            <span className="nav-link__icon" aria-hidden="true">▦</span>
            Cursos
          </NavLink>
          {courseId && <CourseMenu courseId={courseId} courseName={courseName} />}
        </nav>
        {user?.rol === 'Administrador técnico' && (
          <>
            <p className="sidebar__caption sidebar__caption--section">ADMINISTRACIÓN</p>
            <nav className="sidebar__nav" aria-label="Administración">
              <NavLink end to="/admin/sincronizar" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
                <span className="nav-link__icon" aria-hidden="true">↓</span>
                Sincronizar
              </NavLink>
              <NavLink to="/admin/docentes" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
                <span className="nav-link__icon" aria-hidden="true">♙</span>
                Docentes
              </NavLink>
              <NavLink to="/admin/moodle" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
                <span className="nav-link__icon" aria-hidden="true">⚙</span>
                Moodle
              </NavLink>
              <NavLink to="/admin/sincronizaciones" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
                <span className="nav-link__icon" aria-hidden="true">↻</span>
                Historial
              </NavLink>
              <NavLink to="/auditoria" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
                <span className="nav-link__icon" aria-hidden="true">▤</span>
                Auditoría
              </NavLink>
            </nav>
          </>
        )}

        <div className="sidebar__footer">
          <div className="profile-avatar">{user?.nombre?.slice(0, 1).toUpperCase() ?? 'U'}</div>
          <span><strong>{user?.nombre ?? 'Usuario'}</strong><small>{user?.rol ?? 'Panel académico'}</small></span>
          <button className="profile-menu" onClick={handleLogout} type="button">Salir</button>
        </div>
      </aside>

      <main className="main-panel">
        <header className="topbar">
          <span>EduMonitor AI <span className="breadcrumb-separator">/</span> Panel académico</span>
          <div className="topbar__tools">
            <span className="topbar__period"><span className="live-dot" /> Ciclo actual</span>
            <TopbarActions courseId={courseId} courseName={courseName} />
          </div>
        </header>
        <div className="page-content">
          <Outlet />
        </div>
      </main>
    </div>
  )
}

export default AppLayout
