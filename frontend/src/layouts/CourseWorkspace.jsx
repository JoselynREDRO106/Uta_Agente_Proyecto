import { NavLink, Outlet, useParams } from 'react-router-dom'

function CourseWorkspace() {
  const { id } = useParams()
  const base = `/cursos/${id}`
  const links = [
    { label: 'Resumen', to: base, end: true, icon: '▦' },
    { label: 'Estudiantes', to: `${base}/estudiantes`, icon: '♙' },
    { label: 'Análisis grupal', to: `${base}/analisis`, icon: '▥' },
    { label: 'Agente IA', to: `${base}/agente`, icon: '✦' },
    { label: 'Recomendaciones', to: `${base}/recomendaciones`, icon: '✓' },
    { label: 'Alertas', to: `${base}/alertas`, icon: '!' },
    { label: 'Informes', to: `${base}/informes`, icon: '▤' },
  ]

  return (
    <div className="course-workspace">
      <nav aria-label="Panel del curso" className="course-workspace__nav content-card">
        <div className="course-workspace__nav-heading">
          <span className="eyebrow">PANEL DEL CURSO</span>
          <strong>Curso {id}</strong>
        </div>
        {links.map(({ label, to, end, icon }) => (
          <NavLink
            className={({ isActive }) => `course-workspace__link${isActive ? ' is-active' : ''}`}
            end={end}
            key={to}
            to={to}
          >
            <span aria-hidden="true">{icon}</span>
            {label}
          </NavLink>
        ))}
        <NavLink className="course-workspace__back" to="/cursos">
          ← Volver a mis cursos
        </NavLink>
      </nav>
      <div className="course-workspace__content">
        <Outlet />
      </div>
    </div>
  )
}

export default CourseWorkspace
