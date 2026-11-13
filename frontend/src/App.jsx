import { BrowserRouter, Navigate, Outlet, Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout.jsx'
import Login from './pages/Login.jsx'
import Cursos from './pages/Cursos.jsx'
import DashboardCurso from './pages/DashboardCurso.jsx'
import FichaEstudiante from './pages/FichaEstudiante.jsx'
import EstudiantesCurso from './pages/EstudiantesCurso.jsx'
import AnalisisGrupalCurso from './pages/AnalisisGrupalCurso.jsx'
import AsistenteCurso from './pages/AsistenteCurso.jsx'
import RecomendacionesCurso from './pages/RecomendacionesCurso.jsx'
import CourseWorkspace from './layouts/CourseWorkspace.jsx'
import InformesCurso from './pages/InformesCurso.jsx'
import AlertasCurso from './pages/AlertasCurso.jsx'
import ReglasAlerta from './pages/ReglasAlerta.jsx'
import HistorialSincronizaciones from './pages/HistorialSincronizaciones.jsx'
import Auditoria from './pages/Auditoria.jsx'
import { getCurrentUser, hasSession } from './services/api.js'
import ConfiguracionMoodle from './pages/ConfiguracionMoodle.jsx'
import './App.css'

function RequireAuth() {
  return hasSession() ? <Outlet /> : <Navigate to="/login" replace />
}

function RequireAdministrator() {
  return getCurrentUser()?.rol === 'Administrador técnico'
    ? <Outlet />
    : <Navigate to="/cursos" replace />
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route element={<RequireAuth />}>
          <Route element={<AppLayout />}>
            <Route path="/cursos" element={<Cursos />} />
            <Route path="/cursos/:id" element={<CourseWorkspace />}>
              <Route index element={<DashboardCurso />} />
              <Route path="estudiantes" element={<EstudiantesCurso />} />
              <Route path="estudiantes/:studentId" element={<FichaEstudiante />} />
              <Route path="analisis" element={<AnalisisGrupalCurso />} />
              <Route path="asistente" element={<AsistenteCurso />} />
              <Route path="agente" element={<AsistenteCurso />} />
              <Route path="recomendaciones" element={<RecomendacionesCurso />} />
              <Route path="informes" element={<InformesCurso />} />
              <Route path="alertas" element={<AlertasCurso />} />
              <Route path="alertas/reglas" element={<ReglasAlerta />} />
            </Route>
            <Route element={<RequireAdministrator />}>
              <Route path="/admin/moodle" element={<ConfiguracionMoodle />} />
              <Route path="/admin/sincronizaciones" element={<HistorialSincronizaciones />} />
              <Route path="/auditoria" element={<Auditoria />} />
              <Route path="/admin/auditoria" element={<Auditoria />} />
            </Route>
          </Route>
        </Route>
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
