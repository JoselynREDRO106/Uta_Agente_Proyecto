import { useParams } from 'react-router-dom'
import CourseAssistantPanel from '../components/CourseAssistantPanel.jsx'

function AsistenteCurso() {
  const { id } = useParams()

  return (
    <>
      <div className="page-heading">
        <div>
          <span className="eyebrow">ASISTENTE ACADÉMICO</span>
          <h1 id="assistant-heading">Agente académico</h1>
          <p className="muted">Conversa sobre los indicadores del curso. La IA responde con los datos agregados, sin nombres ni notas individuales.</p>
        </div>
      </div>
      <CourseAssistantPanel courseId={id} />
    </>
  )
}

export default AsistenteCurso
