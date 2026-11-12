import { BrowserRouter, Navigate, Outlet, Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout.jsx'
import Login from './pages/Login.jsx'
import Cursos from './pages/Cursos.jsx'
import DashboardCurso from './pages/DashboardCurso.jsx'
import FichaEstudiante from './pages/FichaEstudiante.jsx'
import InformesCurso from './pages/InformesCurso.jsx'
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
            <Route path="/cursos/:id" element={<DashboardCurso />} />
            <Route path="/cursos/:id/informes" element={<InformesCurso />} />
            <Route path="/cursos/:courseId/estudiantes/:studentId" element={<FichaEstudiante />} />
            <Route element={<RequireAdministrator />}>
              <Route path="/admin/moodle" element={<ConfiguracionMoodle />} />
              <Route path="/admin/sincronizaciones" element={<HistorialSincronizaciones />} />
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
