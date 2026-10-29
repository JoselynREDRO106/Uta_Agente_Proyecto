# EduMonitor AI · Frontend

Interfaz inicial en React y Vite para el seguimiento academico de cursos.

## Requisitos

- Node.js 20.19 o superior (o 22.12 o superior)
- npm

## Desarrollo local

Desde esta carpeta instala las dependencias, copia `.env.example` a `.env` y ejecuta:

```sh
npm install
npm run dev
```

Vite inicia la interfaz en `http://localhost:5173`. La variable `VITE_API_URL` apunta al middleware; la pantalla de cursos muestra el resultado de `GET /health`. Si el backend no esta iniciado, el estado de conexion presenta el error.

## Rutas

- `/login`: inicio de sesion de demostracion.
- `/cursos`: listado y estado de conexion al middleware.
- `/cursos/:id`: resumen y seguimiento del curso.
- `/estudiantes/:id`: ficha individual y actividades.

Los registros mostrados son datos de ejemplo para la estructura inicial del Sprint 1.
