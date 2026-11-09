import { BrowserRouter, Navigate, Outlet, Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout.jsx'
import Login from './pages/Login.jsx'
import Cursos from './pages/Cursos.jsx'
import DashboardCurso from './pages/DashboardCurso.jsx'
import FichaEstudiante from './pages/FichaEstudiante.jsx'
import { hasSession } from './services/api.js'
import './App.css'

function RequireAuth() {
  return hasSession() ? <Outlet /> : <Navigate to="/login" replace />
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
            <Route path="/estudiantes/:id" element={<FichaEstudiante />} />
          </Route>
        </Route>
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
