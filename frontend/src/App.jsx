import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout.jsx'
import Login from './pages/Login.jsx'
import Cursos from './pages/Cursos.jsx'
import DashboardCurso from './pages/DashboardCurso.jsx'
import FichaEstudiante from './pages/FichaEstudiante.jsx'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route element={<AppLayout />}>
          <Route path="/cursos" element={<Cursos />} />
          <Route path="/cursos/:id" element={<DashboardCurso />} />
          <Route path="/estudiantes/:id" element={<FichaEstudiante />} />
        </Route>
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
