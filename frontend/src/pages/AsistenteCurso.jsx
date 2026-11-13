import { useParams } from 'react-router-dom'
import CourseAssistantPanel from '../components/CourseAssistantPanel.jsx'

function AsistenteCurso() {
  const { id } = useParams()

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ASISTENTE ACADÉMICO</span>
          <h1>Consultar al agente de IA</h1>
          <p className="muted">Haz preguntas sobre los indicadores agregados de este curso.</p>
        </div>
      </div>
      <CourseAssistantPanel courseId={id} />
    </>
  )
}

export default AsistenteCurso
