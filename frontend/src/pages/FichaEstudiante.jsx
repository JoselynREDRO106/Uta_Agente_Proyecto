import { Link } from 'react-router-dom'

function FichaEstudiante() {
  return (
    <section className="content-card">
      <div className="card-heading">
        <div>
          <span className="eyebrow">SEGUIMIENTO INDIVIDUAL</span>
          <h2>Funcionalidad pendiente</h2>
          <p className="muted">El seguimiento detallado por estudiante corresponde a una historia posterior del Product Backlog.</p>
        </div>
      </div>
      <p className="course-message">
        No se muestran datos de demostración como si fueran resultados académicos reales.
        <Link className="back-link" to="/cursos"> Volver a cursos</Link>
      </p>
    </section>
  )
}

export default FichaEstudiante
