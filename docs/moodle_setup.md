\# Configuración de Moodle — EduMonitor AI



\## 1. Información general



\*\*Proyecto:\*\* EduMonitor AI  

\*\*Institución:\*\* Universidad Técnica de Ambato (UTA)  

\*\*Sprint:\*\* Sprint 1  

\*\*Responsabilidad:\*\* DevOps, configuración de Moodle y pruebas de servicios web  

\*\*Versión de Moodle:\*\* 4.5.x  

\*\*URL local:\*\* http://localhost:8080



\## 2. Objetivo



Documentar la configuración inicial de Moodle para el proyecto EduMonitor AI, incluyendo la creación de cursos, la organización de actividades académicas, la configuración de los servicios web y las pruebas realizadas para consultar información mediante la API REST.



\## 3. Entorno e infraestructura



El entorno local de Moodle utiliza Docker Compose para definir los servicios necesarios para su ejecución.



El archivo de configuración se encuentra en:



`infra/moodle/docker-compose.yml`



Los servicios definidos incluyen:



\- \*\*Base de datos:\*\* MariaDB 11.4.

\- \*\*Aplicación:\*\* Moodle 4.5.

\- \*\*Puerto local:\*\* 8080.

\- \*\*Volúmenes:\*\* almacenamiento persistente para la base de datos, Moodle y los datos de la plataforma.

\- \*\*Zona horaria configurada:\*\* America/Guayaquil.

\- \*\*Idioma configurado:\*\* español.



Las contraseñas se gestionan mediante variables de entorno y no deben publicarse en el repositorio.



\## 4. Configuración académica



Se crearon dos cursos para realizar pruebas con EduMonitor AI:



\- \*\*Servicios Web (SW).\*\*

\- \*\*Bases de Datos (BD).\*\*



Cada curso está organizado en cuatro unidades, identificadas como U1, U2, U3 y U4.



También se crearon actividades para probar el registro y la consulta de calificaciones:



\- Evaluación U1 y Evaluación U2.

\- Tarea U1 y Tarea U2.

\- Práctica 1 y Práctica 2.



Se registraron usuarios ficticios para las pruebas, entre ellos Andrea Lopez y Pedro Ramirez, además del usuario docente. Los estudiantes fueron matriculados en ambos cursos y se ingresaron calificaciones de prueba.



Los datos son ficticios y se utilizan únicamente para comprobar el funcionamiento de las consultas y el análisis académico del proyecto.



\## 5. Configuración de servicios web



Se habilitaron los servicios web de Moodle y el protocolo REST.



Se creó el servicio externo denominado \*\*EduMonitor\*\*, con nombre corto `edumonitor`. Se autorizó un usuario técnico para realizar consultas mediante la API.



Se generó un token de acceso para las pruebas. Por seguridad, el token no se incluye en este documento ni debe guardarse en el repositorio.



\### 5.1 Funciones configuradas



Se incorporaron las siguientes funciones al servicio:



\- `core\_webservice\_get\_site\_info`

\- `core\_course\_get\_courses`

\- `core\_course\_get\_contents`

\- `core\_course\_get\_courses\_by\_field`

\- `core\_enrol\_get\_enrolled\_users`

\- `gradereport\_user\_get\_grade\_items`

\- `gradereport\_user\_get\_grades\_table`



La disponibilidad de las funciones depende de los permisos asignados al usuario técnico y de las capacidades configuradas en Moodle.



\## 6. Pruebas realizadas



\### 6.1 Información del sitio



Se probó `core\_webservice\_get\_site\_info` para verificar el acceso al servicio web de Moodle.



\*\*Resultado:\*\* la consulta respondió correctamente.



\### 6.2 Consulta de cursos



Se probó `core\_course\_get\_courses\_by\_field` utilizando el nombre corto `SW`.



\*\*Resultado:\*\* se obtuvo información del curso Servicios Web, con identificador 2 y sin advertencias en la respuesta.



\### 6.3 Contenido del curso



Se consultó el contenido del curso mediante `core\_course\_get\_contents`.



\*\*Resultado:\*\* la respuesta incluyó las secciones General y U1-U4. En la prueba realizada, las secciones aparecieron sin módulos en la respuesta, por lo que es necesario revisar la visibilidad y ubicación de las actividades si se requiere que la API las devuelva.



\### 6.4 Consulta de calificaciones



Se probaron `gradereport\_user\_get\_grade\_items` y `gradereport\_user\_get\_grades\_table`.



\*\*Resultado:\*\* las consultas respondieron después de ajustar permisos del usuario técnico. Sin embargo, algunos totales de categorías y del curso aparecieron en cero o sin valores, aunque el libro de calificaciones de Moodle mostraba notas en determinadas actividades.



Este comportamiento queda pendiente de revisión para asegurar que las calificaciones y los totales se consulten correctamente desde la API.



\## 7. Seguridad



Para proteger la instalación y el repositorio:



\- No se deben incluir tokens de acceso en archivos versionados.

\- No se deben publicar contraseñas de administradores ni de bases de datos.

\- Las credenciales deben mantenerse en variables de entorno.

\- El archivo `.env` está excluido mediante `infra/moodle/.gitignore`.

\- Los usuarios y las calificaciones utilizados en las pruebas son ficticios.



El acceso a las funciones de Moodle debe limitarse a los permisos necesarios para las consultas del proyecto.



\## 8. Copias de seguridad



Se generó una copia de seguridad del curso Servicios Web desde la interfaz de Moodle.



\*\*Archivo:\*\* `ServiciosWeb\_backup.mbz`



\*\*Ubicación en el proyecto:\*\* `infra/moodle/backups/ServiciosWeb\_backup.mbz`



La copia se incorporó al repositorio para conservar un respaldo del curso utilizado durante las pruebas.



La copia de seguridad de Bases de Datos queda pendiente de incorporar si se requiere disponer de un respaldo de ambos cursos.



\## 9. Archivos del proyecto



Los archivos relacionados con esta actividad son:



\- `infra/moodle/docker-compose.yml`: definición de los servicios del entorno.

\- `infra/moodle/.gitignore`: exclusión del archivo `.env`.

\- `infra/moodle/backups/ServiciosWeb\_backup.mbz`: copia de seguridad del curso Servicios Web.

\- `docs/moodle\_setup.md`: documentación del procedimiento y las pruebas.



\## 10. Pendientes



Antes de considerar terminada la configuración, se recomienda:



1\. Revisar por qué algunas actividades no aparecen en la respuesta de `core\_course\_get\_contents`.

2\. Verificar los permisos y la configuración de las categorías de calificación.

3\. Comprobar que las consultas de calificaciones devuelvan los valores esperados.

4\. Incorporar la copia de seguridad de Bases de Datos, si corresponde.

5\. Añadir evidencias de las pruebas realizadas y revisar el documento con el equipo.



\## 11. Conclusión



Se configuró un entorno local de Moodle 4.5 para apoyar las pruebas iniciales de EduMonitor AI. Se crearon cursos, unidades, actividades y usuarios ficticios, y se habilitó el acceso mediante servicios web REST.



Las consultas de información del sitio y de cursos se probaron correctamente. Las consultas de calificaciones también respondieron, aunque se identificaron diferencias entre algunos resultados de la API y los valores observados en el libro de calificaciones. Estos aspectos deben verificarse antes de utilizar los datos para el análisis académico automatizado.



La configuración y las pruebas constituyen una base para continuar la integración entre Moodle y EduMonitor AI.



