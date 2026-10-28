import { Link, useParams } from 'react-router-dom'
import MetricCard from '../components/MetricCard.jsx'

const activities = [
  { name: 'Evaluación U1 · Fundamentos', category: 'Evaluaciones', date: '12 oct 2026', grade: '78 / 100', state: 'Calificado', tone: 'success' },
  { name: 'Tarea · Modelo de datos', category: 'Tareas', date: '18 oct 2026', grade: '—', state: 'Pendiente', tone: 'warning' },
  { name: 'Práctica · Consultas SQL', category: 'Actividades prácticas', date: '22 oct 2026', grade: '—', state: 'Sin entrega', tone: 'neutral' },
]

function FichaEstudiante() {
  const { id } = useParams()
  const student = id === '103'
    ? { name: 'Camila Fernanda López', email: 'camila.lopez@uta.edu.ec', initials: 'CL', grade: '54.0', note: 'Requiere acompañamiento para mejorar su rendimiento.' }
    : id === '101'
      ? { name: 'Ana Belén Torres', email: 'ana.torres@uta.edu.ec', initials: 'AT', grade: '91.0', note: 'Mantiene un progreso constante en sus actividades.' }
      : { name: 'Mateo Sebastián Ruiz', email: 'mateo.ruiz@uta.edu.ec', initials: 'MR', grade: '73.0', note: 'Se recomienda dar seguimiento a sus próximas entregas.' }

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to="/cursos/1">← Volver al curso</Link>
          <span className="eyebrow">FICHA DEL ESTUDIANTE</span>
          <h1>Resumen individual</h1>
        </div>
      </div>

      <section className="student-profile content-card">
        <div className="student-avatar student-avatar--large">{student.initials}</div>
        <div className="student-profile__identity"><h2>{student.name}</h2><p className="muted">{student.email}</p><span className="status-pill status-pill--warning">En seguimiento</span></div>
        <p className="student-profile__note">{student.note}</p>
      </section>

      <section aria-label="Resumen del estudiante" className="metric-grid metric-grid--two">
        <MetricCard detail="Rendimiento acumulado del curso" label="Promedio actual" tone="blue" value={student.grade} />
        <MetricCard detail="De las actividades planificadas" label="Finalización" tone="green" value="67%" />
      </section>

      <section className="content-card">
        <div className="card-heading"><div><h2>Actividades del curso</h2><p className="muted">Notas y estado de entrega.</p></div><span className="count-pill">{activities.length} actividades</span></div>
        <div className="table-scroll">
          <table>
            <thead><tr><th>Actividad</th><th>Categoría</th><th>Fecha</th><th>Nota</th><th>Estado</th></tr></thead>
            <tbody>
              {activities.map((activity) => (
                <tr key={activity.name}>
                  <td><strong>{activity.name}</strong></td><td>{activity.category}</td><td>{activity.date}</td><td>{activity.grade}</td>
                  <td><span className={`status-pill status-pill--${activity.tone}`}>{activity.state}</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </>
  )
}

export default FichaEstudiante
