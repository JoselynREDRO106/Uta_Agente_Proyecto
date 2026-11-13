import { useParams } from 'react-router-dom'
import RecommendationsPanel from '../components/RecommendationsPanel.jsx'

function RecomendacionesCurso() {
  const { id } = useParams()

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">APOYO PEDAGÓGICO</span>
          <h1>Recomendaciones</h1>
          <p className="muted">Genera y gestiona propuestas basadas en los resultados del curso.</p>
        </div>
      </div>
      <RecommendationsPanel courseId={id} />
    </>
  )
}

export default RecomendacionesCurso
