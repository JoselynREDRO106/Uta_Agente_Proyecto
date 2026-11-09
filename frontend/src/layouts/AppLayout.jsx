import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { getCurrentUser, logout } from '../services/api.js'

function AppLayout() {
  const navigate = useNavigate()
  const user = getCurrentUser()

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
          <NavLink to="/cursos" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
            <span className="nav-link__icon" aria-hidden="true">▦</span>
            Cursos
          </NavLink>
          {user?.rol === 'Administrador técnico' && (
            <NavLink to="/admin/moodle" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
              <span className="nav-link__icon" aria-hidden="true">⚙</span>
              Moodle
            </NavLink>
          )}
          {user?.rol === 'Administrador técnico' && (
            <NavLink to="/admin/sincronizaciones" className={({ isActive }) => `nav-link${isActive ? ' is-active' : ''}`}>
              <span className="nav-link__icon" aria-hidden="true">↻</span>
              Sincronizaciones
            </NavLink>
          )}
        </nav>

        <div className="sidebar__footer">
          <div className="profile-avatar">{user?.nombre?.slice(0, 1).toUpperCase() ?? 'U'}</div>
          <span><strong>{user?.nombre ?? 'Usuario'}</strong><small>{user?.rol ?? 'Panel académico'}</small></span>
          <button className="profile-menu" onClick={handleLogout} type="button">Salir</button>
        </div>
      </aside>

      <main className="main-panel">
        <header className="topbar">
          <span>EduMonitor AI <span className="breadcrumb-separator">/</span> Panel académico</span>
          <span className="topbar__period"><span className="live-dot" /> Ciclo actual</span>
        </header>
        <div className="page-content">
          <Outlet />
        </div>
      </main>
    </div>
  )
}

export default AppLayout
