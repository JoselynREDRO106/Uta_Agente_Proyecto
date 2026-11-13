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
          <p className="muted">Consulta promedios, actividades y tendencias del curso.</p>
        </div>
      </div>
      <GroupAnalysisPanel courseId={id} />
    </>
  )
}

export default AnalisisGrupalCurso
