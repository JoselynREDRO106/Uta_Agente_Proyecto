import { NavLink, Outlet, useParams } from 'react-router-dom'

const courseLinks = [
  { label: 'Resumen', path: '', end: true, icon: '▦' },
  { label: 'Estudiantes', path: '/estudiantes', icon: '♙' },
  { label: 'Análisis grupal', path: '/analisis', icon: '▥' },
  { label: 'Agente IA', path: '/agente', icon: '✦' },
  { label: 'Recomendaciones', path: '/recomendaciones', icon: '✓' },
  { label: 'Planes', path: '/planes', icon: '✉' },
  { label: 'Pruebas', path: '/pruebas', icon: '✔' },
  { label: 'Alertas', path: '/alertas', icon: '!' },
  { label: 'Informes', path: '/informes', icon: '▤' },
]

export function CourseMenu({ courseId, courseName }) {
  const base = `/cursos/${courseId}`

  return (
    <div className="sidebar__submenu">
      {courseName && <p className="sidebar__course-name">{courseName}</p>}
      {courseLinks.map(({ label, path, end, icon }) => (
        <NavLink
          className={({ isActive }) => `nav-link nav-link--sub${isActive ? ' is-active' : ''}`}
          end={end}
          key={path}
          to={`${base}${path}`}
        >
          <span className="nav-link__icon" aria-hidden="true">{icon}</span>
          {label}
        </NavLink>
      ))}
    </div>
  )
}

function CourseWorkspace() {
  const { id } = useParams()

  return <Outlet key={id} />
}

export default CourseWorkspace
