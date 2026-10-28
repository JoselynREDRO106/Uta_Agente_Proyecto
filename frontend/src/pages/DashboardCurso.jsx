import { Link, useParams } from 'react-router-dom'
import MetricCard from '../components/MetricCard.jsx'

const students = [
  { id: '101', name: 'Ana Belén Torres', email: 'ana.torres@uta.edu.ec', grade: 91, trend: 'Alto rendimiento', tone: 'success' },
  { id: '102', name: 'Mateo Sebastián Ruiz', email: 'mateo.ruiz@uta.edu.ec', grade: 73, trend: 'En seguimiento', tone: 'warning' },
  { id: '103', name: 'Camila Fernanda López', email: 'camila.lopez@uta.edu.ec', grade: 54, trend: 'Requiere apoyo', tone: 'danger' },
]

function DashboardCurso() {
  const { id } = useParams()
  const courseName = id === '2' ? 'Bases de Datos' : id === '3' ? 'Ingeniería de Software' : 'Servicios Web'

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to="/cursos">← Volver a cursos</Link>
          <span className="eyebrow">CURSO · SW-4B</span>
          <h1>{courseName}</h1>
          <p className="muted">Vista general del progreso de la asignatura.</p>
        </div>
        <button className="button button--secondary" type="button">Periodo actual⌄</button>
      </div>

      <section aria-label="Indicadores del curso" className="metric-grid">
        <MetricCard detail="De 100 puntos posibles" label="Promedio general" tone="blue" value="82.4" />
        <MetricCard detail="Matriculados en el curso" label="Estudiantes" tone="violet" value="28" />
        <MetricCard detail="Actividades completadas" label="Finalización" tone="green" value="68%" />
      </section>

      <section className="content-card">
        <div className="card-heading">
          <div><h2>Seguimiento de estudiantes</h2><p className="muted">Revisa el avance individual y las alertas.</p></div>
          <span className="count-pill">3 destacados</span>
        </div>
        <div className="table-scroll">
          <table>
            <thead><tr><th>Estudiante</th><th>Promedio</th><th>Estado</th><th /></tr></thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td><Link className="student-cell" to={`/estudiantes/${student.id}`}><span className="student-avatar">{student.name.split(' ').map((part) => part[0]).slice(0, 2).join('')}</span><span><strong>{student.name}</strong><small>{student.email}</small></span></Link></td>
                  <td><span className="grade-value">{student.grade}</span><span className="muted"> / 100</span></td>
                  <td><span className={`status-pill status-pill--${student.tone}`}>{student.trend}</span></td>
                  <td><Link className="icon-link" aria-label={`Ver ficha de ${student.name}`} to={`/estudiantes/${student.id}`}>↗</Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </>
  )
}

export default DashboardCurso
