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

Vite inicia la interfaz en `http://localhost:5173`. Define `VITE_API_URL=http://localhost:5000` para conectar con la API. La pantalla consulta `GET /health` y `GET /api/cursos`; si la API o SQL Server no estan disponibles, muestra el error.

Para importar un curso, escribe su ID de Moodle en el formulario y pulsa **Sincronizar curso**. El frontend solicita `POST /api/cursos/sincronizar/{courseId}`; el backend obtiene el curso desde Moodle y lo guarda en SQL Server. Luego la pantalla vuelve a consultar `GET /api/cursos` y muestra los cursos guardados. La URL y el token de Moodle se configuran solo en el backend.

## Rutas

- `/login`: inicio de sesion de demostracion.
- `/cursos`: cursos sincronizados y estado de conexion al middleware.
- `/cursos/:id`: resumen y seguimiento del curso.
- `/estudiantes/:id`: ficha individual y actividades.

Las vistas de seguimiento de curso y estudiante siguen siendo de demostracion.
