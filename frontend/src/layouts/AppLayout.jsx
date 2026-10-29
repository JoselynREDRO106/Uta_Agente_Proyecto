import { NavLink, Outlet } from 'react-router-dom'

function AppLayout() {
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
        </nav>

        <div className="sidebar__footer">
          <div className="profile-avatar">DR</div>
          <span><strong>Docente</strong><small>Panel académico</small></span>
          <span className="profile-menu" aria-hidden="true">···</span>
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
