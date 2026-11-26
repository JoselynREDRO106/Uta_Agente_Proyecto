import { useParams } from 'react-router-dom'
import GroupAnalysisPanel from '../components/GroupAnalysisPanel.jsx'

function AnalisisGrupalCurso() {
  const { id } = useParams()

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ANÁLISIS ACADÉMICO</span>
          <h1>Análisis grupal</h1>
          <p className="muted">Promedios sobre 10. La tendencia sigue el orden de las actividades y la tabla ordena de menor a mayor promedio.</p>
        </div>
      </div>
      <GroupAnalysisPanel courseId={id} />
    </>
  )
}

export default AnalisisGrupalCurso
