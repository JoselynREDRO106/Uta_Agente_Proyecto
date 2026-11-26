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
          <p className="muted">Propuestas agrupadas por actividad, con el umbral del análisis grupal.</p>
        </div>
      </div>
      <RecommendationsPanel courseId={id} />
    </>
  )
}

export default RecomendacionesCurso
