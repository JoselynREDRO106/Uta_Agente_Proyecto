**UNIVERSIDAD TÉCNICA DE AMBATO**
**FACULTAD DE INGENIERÍA EN SISTEMAS, ELECTRÓNICA E INDUSTRIAL**
**CARRERA DE INGENIERÍA EN SOFTWARE**
**CICLO ACADÉMICO: JULIO – DICIEMBRE 2026**

---

# AcademIA
## INFORME DEL SPRINT 1

**Planificación y resultados**

**Sistema inteligente de seguimiento del rendimiento académico integrado con Moodle**

| | |
|---|---|
| **Product Owner** | Ing. Hernán Naranjo, Mg. |
| **Equipo de desarrollo** | Curillo Pilamunga Diego Alexander |
| | Manobanda Masabanda Luis Alfredo |
| | Redroban Chimbo Joselyn Amparo |
| | Quispe Nasimba Daniela Alejandra |
| **Fecha de planificación** | 20/10/2026 |
| **Período del Sprint 1** | 19/10/2026 – 29/10/2026 |

**JULIO – DICIEMBRE 2026**
**AMBATO**

---

# ÍNDICE

1. INTRODUCCIÓN
   - 1.1 Descripción del problema
   - 1.2 Descripción del proyecto
   - 1.3 Alcance del Sprint 1
   - 1.4 Objetivos
   - 1.5 Marco de referencia
2. JUSTIFICACIÓN DEL STACK TECNOLÓGICO
3. ARQUITECTURA DEL SISTEMA
   - 3.1 Descripción general
   - 3.2 Componentes y responsabilidades
   - 3.3 Flujo conceptual de una sincronización y consulta
   - 3.4 Diagrama de arquitectura (descripción textual)
4. RECURSOS DEL PROYECTO
5. REQUERIMIENTOS DEL SISTEMA
   - 5.1 Requerimientos funcionales
   - 5.2 Requerimientos no funcionales
   - 5.3 Reglas de negocio
6. METODOLOGÍA SCRUM APLICADA
   - 6.1 Roles
   - 6.2 Eventos y artefactos
   - 6.3 Definición de Terminado
7. DATOS GENERALES DE LA PLANIFICACIÓN
8. PARTICIPANTES DE LA REUNIÓN
9. AGENDA DE LA REUNIÓN DE PLANIFICACIÓN
10. OBJETIVO DEL SPRINT 1
11. REVISIÓN Y PRIORIZACIÓN DEL PRODUCT BACKLOG
12. ANÁLISIS DE CAPACIDAD DEL SPRINT 1
13. HISTORIAS DE USUARIO SELECCIONADAS
    - 13.1 Tabla resumen
    - 13.2 Fichas, mockups y desglose de tareas por Historia de Usuario
14. CRITERIOS DE ACEPTACIÓN
15. ESTIMACIÓN DE LAS HISTORIAS DE USUARIO
16. DESCOMPOSICIÓN DE HISTORIAS DE USUARIO EN TAREAS
17. SPRINT BACKLOG
    - 17.1 Tablero Scrum (estado inicial)
    - 17.2 Resumen del Sprint Backlog
18. DISTRIBUCIÓN DE RESPONSABILIDADES
19. PLAN DE TRABAJO DEL SPRINT 1
    - 19.1 Orden de elaboración y dependencias
    - 19.2 Cronograma diario por integrante
    - 19.3 Riesgos e impedimentos
    - 19.4 Criterios de finalización del Sprint 1
20. DEFINICIÓN DE TERMINADO (DEFINITION OF DONE)
21. COMPROMISO DEL EQUIPO
22. CONCLUSIONES DE LA PLANIFICACIÓN
23. RESULTADOS DEL SPRINT 1
24. TESTING DE LAS HISTORIAS DE USUARIO
25. DIAGRAMAS
    - 25.1 Casos de uso generales
    - 25.2 Secuencia: inicio de sesión
    - 25.3 Secuencia: sincronización con Moodle
    - 25.4 Diagrama de clases
26. RETROSPECTIVA DEL SPRINT 1
27. MÉTRICAS Y CONCLUSIÓN

---

# 1. INTRODUCCIÓN

## 1.1 Descripción del problema

En la actualidad, el docente que utiliza Moodle como entorno virtual de aprendizaje debe revisar manualmente calificaciones, entregas y estados de actividad que se encuentran dispersos en distintas secciones de cada curso. Esta situación genera las siguientes dificultades:

- La información académica está fragmentada entre libros de calificaciones, actividades y reportes, lo que dificulta tener una visión consolidada del curso.
- La detección de dificultades académicas depende de la revisión manual y del tiempo disponible del docente, por lo que suele realizarse de forma tardía.
- No existen indicadores consolidados (promedios con su escala, distribución de calificaciones, progreso) que permitan comparar de forma rápida a estudiantes o actividades.
- Resulta difícil identificar a tiempo atrasos, actividades sin entrega o descensos en las evaluaciones.

**AcademIA** busca centralizar la información académica proveniente de Moodle y presentarla de forma clara, con indicadores transparentes y trazables, para apoyar la toma de decisiones pedagógicas del docente. El sistema asiste al docente; no sustituye su criterio profesional.

## 1.2 Descripción del proyecto

AcademIA es un sistema inteligente de seguimiento y análisis del rendimiento académico, diseñado para integrarse con la plataforma Moodle mediante sus Servicios Web REST. Su propósito principal es asistir a los docentes en la detección temprana de dificultades académicas a partir del análisis de calificaciones, actividades y contenidos temáticos.

A mediano plazo, el sistema contará con un agente de inteligencia artificial capaz de analizar la información académica y generar recomendaciones pedagógicas personalizadas para ayudar a los estudiantes a reforzar las áreas en las que presentan dificultades. El proyecto se desarrolla en tres Sprints bajo la metodología Scrum y sobre una arquitectura de microservicios.

## 1.3 Alcance del Sprint 1

**Incluye:**

- Autenticación y autorización con los roles Docente y Administrador técnico (RF-01).
- Configuración y verificación de la conexión con los servicios web de Moodle, con manejo seguro de credenciales técnicas (RF-02).
- Sincronización de cursos, matriculados, ítems de calificación, notas y estados de actividad, con registro de fechas y errores (RF-03).
- Selección de curso y período, y listado de estudiantes con búsqueda y filtros (RF-04).
- Versión básica del panel de indicadores: promedio con escala explícita, distribución de calificaciones, actividades evaluadas y progreso disponible (RF-05).
- Estructura base de microservicios, persistencia en SQL Server y aplicación React.js que servirán de base para los Sprints siguientes.

**No incluye (permanece en el Product Backlog):**

- Seguimiento individual detallado (RF-06), alertas académicas (RF-07), análisis grupal (RF-08).
- Agente de IA, recomendaciones e informes (RF-09, RF-10, RF-11). **En el Sprint 1 no se implementa el agente de IA.**
- Módulo completo de auditoría (RF-12). El Sprint 1 solo registra el resultado de cada sincronización.
- Planificación de los Sprints 2 y 3, que queda fuera de este informe.

## 1.4 Objetivos

**Objetivo general del proyecto:** Desarrollar un sistema basado en microservicios, integrado con Moodle, que permita al docente consultar indicadores del rendimiento académico de sus cursos, detectar tempranamente dificultades y recibir recomendaciones pedagógicas generadas por un agente de inteligencia artificial.

**Objetivos específicos del Sprint 1:**

1. Implementar el acceso seguro al sistema con roles Docente y Administrador técnico, restringiendo la información a los cursos autorizados.
2. Configurar y verificar la conexión con los servicios web REST de Moodle, protegiendo las credenciales técnicas.
3. Sincronizar cursos, matriculados, calificaciones y estados de actividad hacia SQL Server, registrando fechas y errores.
4. Permitir al docente seleccionar un curso y consultar su listado de estudiantes con búsqueda y filtros.
5. Mostrar un panel básico de indicadores con escala explícita, fuente y fecha de sincronización.
6. Dejar establecida la arquitectura de microservicios sobre la cual se construirán los siguientes incrementos.

## 1.5 Marco de referencia

No se identifica normativa externa específica aplicable a AcademIA. El proyecto se rige por los requerimientos no funcionales de **seguridad (RNF-01)** y **privacidad (RNF-02)**: uso de HTTPS durante el despliegue, secretos en variables de entorno, control de acceso por curso, mínimo privilegio, datos mínimos, acceso restringido, anonimización de datos para pruebas y prohibición de enviar información identificable a servicios externos sin autorización. Las reglas de negocio del proyecto (sección 5.3) se derivan de los requerimientos establecidos junto con el Product Owner.

---

# 2. JUSTIFICACIÓN DEL STACK TECNOLÓGICO

La arquitectura del proyecto establece tecnologías obligatorias. A continuación se justifica cada una e indica su alcance en el Sprint 1.

| Tecnología | Justificación | ¿Se implementa en el Sprint 1? |
|---|---|---|
| **React.js** (Frontend) | Permite construir una interfaz de una sola página con componentes reutilizables (formularios, tablas, tarjetas de indicadores y gráficos), adecuada para visualizar información académica con filtros y paginación. | Sí |
| **.NET 10 con Clean Architecture** (Backend / Middleware) | Facilita separar dominio, casos de uso, infraestructura y API en cada microservicio, de modo que la lógica de negocio sea independiente de Moodle y de la base de datos. Mejora la mantenibilidad (RNF-05) y la capacidad de probar los indicadores (RNF-06). | Sí |
| **SQL Server** (Persistencia) | Almacena de forma relacional usuarios, cursos, matrículas, calificaciones, estados de actividad y bitácora de sincronizaciones. Las migraciones versionan el esquema. | Sí |
| **Arquitectura de microservicios** | Separa responsabilidades funcionales (identidad, integración con Moodle, análisis académico) y permite incorporar servicios de IA en Sprints posteriores sin alterar los existentes. | Sí (servicios base) |
| **Servicios Web REST de Moodle** (Integración externa) | Es el mecanismo oficial para obtener cursos, matriculados, calificaciones y estados de actividad sin acceder directamente a la base de datos de Moodle. | Sí |
| **Python / FastAPI** (Servicios de IA) | Ecosistema adecuado para servicios de inteligencia artificial expuestos como API. | No. Sprints posteriores |
| **LangGraph** (Orquestación de IA) | Gestiona flujos y herramientas controladas del agente inteligente. | No. Sprints posteriores |
| **Ollama** (Modelos locales) | Ejecuta modelos de lenguaje locales, favoreciendo la privacidad (RNF-02) al evitar enviar información a servicios externos. | No. Sprints posteriores |

---

# 3. ARQUITECTURA DEL SISTEMA

## 3.1 Descripción general

AcademIA adopta una arquitectura de microservicios. Cada microservicio .NET 10 aplica Clean Architecture (capas Domain, Application, Infrastructure y API) y tiene una responsabilidad funcional definida. La aplicación React.js consume los endpoints REST protegidos con JWT. La información proveniente de Moodle se obtiene únicamente a través de sus Servicios Web REST y se almacena en SQL Server para su análisis posterior.

## 3.2 Componentes y responsabilidades

| Componente | Responsabilidad | Sprint |
|---|---|---|
| **Cliente React.js** | Interfaz de usuario: inicio de sesión, configuración de Moodle, sincronización, cursos, estudiantes y panel de indicadores. | 1 |
| **Servicio de Identidad** (.NET 10) | Usuarios, roles (Docente, Administrador técnico), emisión y validación de JWT. | 1 |
| **Servicio de Integración con Moodle** (.NET 10) | Configuración de la conexión, verificación de conectividad y consumo de los servicios web REST de Moodle. | 1 |
| **Servicio Académico** (.NET 10) | Cursos, estudiantes, matrículas, calificaciones, estados de actividad, bitácora de sincronización, autorización por curso e indicadores. | 1 |
| **SQL Server** | Persistencia relacional (esquema independiente por servicio). | 1 |
| **Moodle (externo)** | Fuente de la información académica vía Servicios Web REST. | 1 |
| **Servicio de Alertas, Análisis grupal e Informes** | Reglas configurables de alerta, análisis grupal y generación de informes. | Posteriores |
| **Servicio de Auditoría** | Registro de sincronizaciones, consultas y herramientas usadas por el agente. | Posteriores |
| **Servicios de IA** (Python / FastAPI + LangGraph + Ollama) | Agente de IA con herramientas controladas y recomendaciones pedagógicas. | Posteriores |

## 3.3 Flujo conceptual de una sincronización y consulta

1. El Administrador técnico inicia sesión y registra la URL de Moodle; la credencial técnica se lee desde variables de entorno.
2. El Servicio de Integración verifica la conectividad con los servicios web de Moodle.
3. Se solicita una sincronización. El Servicio de Integración consume los servicios web REST de Moodle (cursos, matriculados, ítems de calificación, notas y estados).
4. El Servicio Académico guarda los datos en SQL Server de forma idempotente y registra fecha, resultado y errores.
5. El Docente inicia sesión y recibe un JWT con su rol.
6. El Docente selecciona un curso autorizado; el Servicio Académico valida el permiso por curso y devuelve el listado paginado y los indicadores.
7. React muestra la información con su escala, la fuente (Moodle) y la fecha de última sincronización.

## 3.4 Diagrama de arquitectura (descripción textual)

```
┌─────────────────────┐
│  Docente / Admin    │
│  técnico (navegador)│
└─────────┬───────────┘
          │ HTTPS / REST + JWT
┌─────────▼───────────┐
│   React.js (SPA)    │
└───┬─────────┬───────┴─────────┐
    │         │                 │
┌───▼─────┐ ┌─▼───────────┐ ┌───▼──────────────┐
│Identidad│ │ Integración │ │    Académico     │
│ (.NET)  │ │ Moodle(.NET)│ │     (.NET)       │
└───┬─────┘ └─┬───────┬───┘ └───┬──────────────┘
    │         │       │         │
┌───▼─────────▼───────│─────────▼───┐
│          SQL Server               │
└───────────────────────────────────┘
                      │ REST (Servicios Web)
                ┌─────▼─────┐
                │  Moodle   │
                └───────────┘

 · · · · · · · · Sprints posteriores · · · · · · · ·
 Alertas / Informes / Auditoría  ·  Servicios de IA
 (Python/FastAPI + LangGraph + Ollama)
```

Los componentes con línea punteada (Sprints posteriores) no forman parte del incremento del Sprint 1.

---

# 4. RECURSOS DEL PROYECTO

**Tecnológicos (software)**

| Recurso | Uso |
|---|---|
| .NET 10 (ASP.NET Core) | Microservicios del backend con Clean Architecture |
| Entity Framework Core | Acceso a datos y migraciones |
| SQL Server | Persistencia relacional |
| React.js (JavaScript) | Interfaz web |
| Servicios Web REST de Moodle | Integración externa |
| Git (repositorio remoto) | Control de versiones |
| xUnit / Postman | Pruebas unitarias y de API |
| Swagger (OpenAPI) | Documentación de endpoints |
| Visual Studio / Visual Studio Code | Entornos de desarrollo |
| Tablero Kanban (herramienta a definir por el equipo) | Seguimiento del Sprint Backlog |
| Python / FastAPI, LangGraph, Ollama | Servicios de IA (Sprints posteriores) |

**Hardware:** equipos de desarrollo de los integrantes con capacidad para ejecutar el SDK de .NET 10, SQL Server, Node.js y navegador web. Para el Sprint 1 se requiere acceso a una instancia Moodle de pruebas.

**Humanos:** Product Owner (Ing. Hernán Naranjo, Mg.) y cuatro integrantes del equipo de desarrollo (sección 6.1).

**Datos y documentación:** requerimientos funcionales y no funcionales del proyecto; datos académicos de prueba anonimizados (RNF-02).

---

# 5. REQUERIMIENTOS DEL SISTEMA

## 5.1 Requerimientos funcionales

| Código | Requerimiento | Descripción | Sprint previsto |
|---|---|---|---|
| RF-01 | Autenticación y autorización | Acceso mediante credenciales del sistema, con roles docente y administrador técnico. Cada docente visualiza únicamente los cursos y estudiantes autorizados. | 1 |
| RF-02 | Conexión Moodle | Configurar URL y credenciales técnicas seguras y verificar la conectividad con los servicios web de Moodle. | 1 |
| RF-03 | Sincronización | Importar cursos, matriculados, ítems de calificación, notas y estados de actividad cuando la API y los permisos lo permitan. Registrar fechas y errores. | 1 |
| RF-04 | Gestión de cursos | Seleccionar curso, período y listado de estudiantes, con búsqueda y filtros. | 1 |
| RF-05 | Panel de indicadores | Mostrar promedio con escala explícita, distribución de calificaciones, actividades evaluadas y progreso disponible. | 1 (versión básica) |
| RF-06 | Seguimiento individual | Presentar evolución de notas, actividades sin entrega, pendientes de calificar y completadas, diferenciando sus estados. | Posterior |
| RF-07 | Alertas académicas | Aplicar reglas configurables para detectar descensos en evaluaciones, atrasos y baja finalización. Mostrar evidencia y evitar etiquetas definitivas. | Posterior |
| RF-08 | Análisis grupal | Identificar actividades con resultados bajos y tendencias de rendimiento del curso. | Posterior |
| RF-09 | Agente de IA | Responder preguntas del docente mediante datos obtenidos con herramientas controladas y distinguir hechos, inferencias y recomendaciones. | Posterior |
| RF-10 | Recomendaciones | Proponer acciones de apoyo concretas y justificadas mediante indicadores académicos. | Posterior |
| RF-11 | Informe | Generar resúmenes consultables o descargables con período, métricas, hallazgos, recomendaciones y fecha de actualización. | Posterior |
| RF-12 | Auditoría | Registrar sincronizaciones, consultas y herramientas utilizadas por el agente sin exponer secretos. | Posterior |

## 5.2 Requerimientos no funcionales

| Código | Requerimiento | Descripción |
|---|---|---|
| RNF-01 | Seguridad | HTTPS durante el despliegue, secretos en variables de entorno, control de acceso por curso y mínimo privilegio. |
| RNF-02 | Privacidad | Datos mínimos, acceso restringido, anonimización para pruebas y prohibición de enviar información identificable a servicios externos sin autorización. |
| RNF-03 | Confiabilidad | Manejo de errores de API, datos ausentes y sincronización incompleta sin presentar métricas engañosas. |
| RNF-04 | Usabilidad | Interfaz clara, filtros comprensibles, indicadores con unidades y mensajes de error útiles. |
| RNF-05 | Mantenibilidad | Repositorio Git, estructura modular y documentación de instalación y configuración. |
| RNF-06 | Calidad | Pruebas unitarias de indicadores, integración de API y pruebas de permisos; resultados reproducibles. |
| RNF-07 | Rendimiento | Consultas paginadas o limitadas y respuestas razonables para un curso de prueba; documentar tiempos medidos. |
| RNF-08 | Transparencia | Mostrar fuente, fecha de sincronización y justificación de alertas; las respuestas de IA deben reconocer incertidumbre. |

## 5.3 Reglas de negocio

Las siguientes reglas de negocio se derivan de los requerimientos funcionales y no funcionales acordados con el Product Owner. No corresponden a normativa externa. La columna **Evalúa** indica quién o qué verifica el cumplimiento de la regla.

| ID | Regla | Fuente | Evalúa | Sprint de aplicación |
|---|---|---|---|---|
| RN-01 | El acceso al sistema requiere credenciales válidas. Los roles existentes son Docente y Administrador técnico. | RF-01 | Código | 1 |
| RN-02 | Un docente solo puede visualizar los cursos y estudiantes que tiene autorizados. | RF-01, RNF-01 | Código | 1 |
| RN-03 | Solo el Administrador técnico puede configurar o modificar la conexión con Moodle. | RF-01, RF-02, RNF-01 (mínimo privilegio) | Código | 1 |
| RN-04 | La URL y las credenciales técnicas de Moodle se manejan como secretos en variables de entorno: no se guardan en el repositorio, no se muestran en la interfaz ni se escriben en registros. | RF-02, RNF-01, RNF-02 | Código y diseño | 1 |
| RN-05 | Solo se importa la información que la API y los permisos de Moodle permitan; lo que no esté disponible se identifica como “no disponible” y no se asume ni se inventa. | RF-03 | Código | 1 |
| RN-06 | Cada sincronización registra su fecha, su resultado y los errores ocurridos. | RF-03 | Código | 1 |
| RN-07 | Una sincronización incompleta o la ausencia de datos no debe producir métricas engañosas: el indicador se muestra con advertencia o como “sin datos”. Una nota ausente no equivale a cero. | RNF-03 | Código | 1 |
| RN-08 | Todo promedio se presenta con la escala de calificación explícita del ítem obtenida de Moodle. | RF-05, RNF-04 | Código y diseño | 1 |
| RN-09 | Toda información académica mostrada indica su fuente (Moodle) y la fecha de la última sincronización. | RNF-08 | Código | 1 |
| RN-10 | Se almacenan únicamente los datos mínimos necesarios; los datos de prueba están anonimizados y no se envía información identificable a servicios externos sin autorización. | RNF-02 | Diseño y equipo | 1 en adelante |
| RN-11 | Los listados y consultas son paginados o limitados. | RNF-07 | Código | 1 |
| RN-12 | Las actividades se clasifican diferenciando sin entrega, pendientes de calificar y completadas. En el Sprint 1 solo se importa el estado de actividad. | RF-06 | Código | 2 en adelante |
| RN-13 | Las alertas académicas aplican reglas configurables, muestran su evidencia y no establecen etiquetas definitivas sobre el estudiante. | RF-07, RNF-08 | Código y diseño | Posterior |
| RN-14 | El agente de IA solo propone: usa herramientas controladas, distingue hechos, inferencias y recomendaciones, reconoce incertidumbre y la decisión pedagógica corresponde al docente. | RF-09, RF-10, RNF-08 | Diseño y docente | Posterior |
| RN-15 | La auditoría registra sincronizaciones, consultas y herramientas utilizadas sin exponer secretos. | RF-12, RNF-01 | Código | Posterior |

---

# 6. METODOLOGÍA SCRUM APLICADA

## 6.1 Roles

| Integrante | Rol Scrum | Responsabilidad principal |
|---|---|---|
| Ing. Hernán Naranjo, Mg. | Product Owner | Representa las necesidades del producto, establece prioridades, aclara requerimientos y valida que las funcionalidades aporten valor. No forma parte del equipo de desarrollo ni realiza tareas de programación. |
| Curillo Pilamunga Diego Alexander | Equipo de desarrollo | Responsabilidad técnica: backend .NET 10 e integración con los servicios web REST de Moodle. |
| Manobanda Masabanda Luis Alfredo | Equipo de desarrollo | Responsabilidad técnica: modelo de datos en SQL Server, autorización y lógica de sincronización. |
| Redroban Chimbo Joselyn Amparo | Equipo de desarrollo | Responsabilidad técnica: interfaces React.js, endpoints de consulta y cálculo de indicadores. |
| Quispe Nasimba Daniela Alejandra | Equipo de desarrollo | Responsabilidad técnica: interfaces React.js y pruebas (unitarias, de API y de permisos). |

El equipo de desarrollo es autoorganizado y mantiene la responsabilidad colectiva sobre el incremento del producto. Las responsabilidades técnicas indicadas orientan la distribución del trabajo y no constituyen cargos adicionales de Scrum.

## 6.2 Eventos y artefactos

**Eventos aplicados al Sprint 1**

- **Planificación del Sprint:** reunión de 6 horas el 20/10/2026 donde se define el objetivo, se seleccionan las Historias de Usuario y se construye el Sprint Backlog.
- **Reunión diaria:** 15 minutos para sincronizar avances, plan del día e impedimentos.
- **Revisión del Sprint:** demostración del incremento al Product Owner al cierre del Sprint (29/10/2026).
- **Retrospectiva del Sprint:** análisis del proceso del equipo para mejorar en el siguiente Sprint.

**Artefactos**

- **Product Backlog:** lista priorizada de requerimientos (RF-01 a RF-12) gestionada por el Product Owner.
- **Sprint Backlog:** Historias de Usuario comprometidas y sus tareas (sección 17), representado en el tablero Scrum.
- **Incremento:** versión funcional y verificable del sistema al finalizar el Sprint.

**Historias de Usuario y las tres C:** cada historia se redacta como *Card* (formato Como / Quiero / Para), se refina mediante *Conversation* con el Product Owner y se cierra con *Confirmation* a través de criterios de aceptación. Las historias procuran cumplir las características INVEST (independiente, negociable, valiosa, estimable, pequeña y verificable).

## 6.3 Definición de Terminado

La Definición de Terminado completa del proyecto, diferenciada entre tarea e Historia de Usuario, se presenta en la sección 20.

---

# 7. DATOS GENERALES DE LA PLANIFICACIÓN

| Elemento | Información |
|---|---|
| Proyecto | AcademIA |
| Sprint | Sprint 1 |
| Fecha de inicio | 19/10/2026 |
| Fecha de planificación | 20/10/2026 |
| Fecha de entrega | 29/10/2026 |
| Duración de la reunión | 6 horas (360 minutos) |
| Product Owner | Ing. Hernán Naranjo, Mg. |
| Equipo de desarrollo | Cuatro integrantes |
| Metodología | Scrum |
| Arquitectura | Microservicios |

> **Nota aclaratoria sobre fechas:** la fecha de la reunión de planificación (20/10/2026) es posterior a la fecha de inicio del Sprint (19/10/2026). Se respetan las fechas proporcionadas sin modificarlas. Para el cálculo de capacidad se asume que el día 19/10 corresponde al arranque del Sprint y la preparación del entorno, y que la ejecución del trabajo planificado transcurre del 21/10/2026 al 29/10/2026. Se sugiere al Product Owner confirmar este criterio. Asimismo, las seis horas de la reunión de planificación no corresponden a la duración del Sprint.

---

# 8. PARTICIPANTES DE LA REUNIÓN

| N.º | Nombre completo | Participación | Responsabilidad |
|---|---|---|---|
| 1 | Ing. Hernán Naranjo, Mg. | Product Owner | Presentar y priorizar el Product Backlog, aclarar requerimientos, validar el objetivo del Sprint y los criterios de aceptación. |
| 2 | Curillo Pilamunga Diego Alexander | Equipo de desarrollo | Aportar al análisis de tareas de backend e integración con Moodle; estimar y comprometerse con el alcance. |
| 3 | Manobanda Masabanda Luis Alfredo | Equipo de desarrollo | Aportar al análisis del modelo de datos, autorización y sincronización; estimar y comprometerse. |
| 4 | Redroban Chimbo Joselyn Amparo | Equipo de desarrollo | Aportar al análisis de interfaces, endpoints de consulta e indicadores; estimar y comprometerse. |
| 5 | Quispe Nasimba Daniela Alejandra | Equipo de desarrollo | Aportar al análisis de interfaces y pruebas; estimar y comprometerse. |

---

# 9. AGENDA DE LA REUNIÓN DE PLANIFICACIÓN

| N.º | Actividad | Descripción | Duración (minutos) | Responsable |
|---|---|---|---|---|
| 1 | Revisión del Product Backlog | Presentación del producto y revisión de RF-01 a RF-12 y RNF-01 a RNF-08. | 30 | Product Owner |
| 2 | Priorización de requerimientos | Análisis de dependencias, valor y viabilidad; selección de requerimientos para el Sprint 1. | 40 | Product Owner y equipo |
| 3 | Definición del objetivo del Sprint | Redacción y acuerdo del objetivo concreto, alcanzable y verificable. | 20 | Product Owner y equipo |
| 4 | Elaboración y aclaración de Historias de Usuario | Redacción en formato Como / Quiero / Para y aclaración de dudas (Card y Conversation). | 60 | Equipo de desarrollo |
| 5 | Definición de criterios de aceptación | Criterios Dado / Cuando / Entonces verificables (Confirmation). | 50 | Equipo y Product Owner |
| 6 | Estimación del esfuerzo | Estimación relativa en puntos de historia. | 40 | Equipo de desarrollo |
| 7 | Descomposición de Historias de Usuario en tareas | Definición de tareas técnicas y estimación en horas. | 50 | Equipo de desarrollo |
| 8 | Distribución de responsabilidades | Análisis de capacidad y asignación de responsable y colaborador por tarea. | 30 | Equipo de desarrollo |
| 9 | Consolidación del Sprint Backlog | Construcción del tablero Scrum y verificación de coherencia. | 20 | Equipo de desarrollo |
| 10 | Compromiso final del equipo | Revisión del alcance con el Product Owner y compromiso del equipo. | 20 | Equipo y Product Owner |
| | **Total** | | **360** | |

---

# 10. OBJETIVO DEL SPRINT 1

> **Objetivo del Sprint 1:** Entregar un incremento funcional de AcademIA en el que un docente autenticado pueda consultar, para un curso autorizado de una instancia Moodle de pruebas previamente sincronizada, el listado de estudiantes y un panel básico de indicadores (promedio con escala explícita, distribución de calificaciones, actividades evaluadas y progreso disponible), mostrando la fuente y la fecha de sincronización, sobre una arquitectura de microservicios con React.js, .NET 10 y SQL Server.

**Valor esperado:** al finalizar el Sprint, el docente dispone de una vista consolidada y confiable de sus cursos de prueba, y el equipo cuenta con la base técnica (acceso seguro, integración con Moodle y persistencia) sobre la cual se construirán las alertas, el análisis grupal y el agente de IA.

**Verificación:** el objetivo se considera cumplido cuando se demuestra al Product Owner el flujo completo: inicio de sesión, verificación de la conexión con Moodle, sincronización, selección de curso, listado de estudiantes y panel de indicadores.

---

# 11. REVISIÓN Y PRIORIZACIÓN DEL PRODUCT BACKLOG

| Prioridad | Código RF | Requerimiento | Justificación de prioridad | Seleccionado |
|---|---|---|---|---|
| 1 | RF-01 | Autenticación y autorización | Es prerrequisito de todo el sistema: sin identidad y roles no se puede restringir el acceso por curso (RN-02, RNF-01). | Sí |
| 2 | RF-02 | Conexión Moodle | Sin conexión verificada con Moodle no existe fuente de datos; incorpora el manejo seguro de secretos (RN-04). | Sí |
| 3 | RF-03 | Sincronización | Depende de RF-02. Provee los datos que alimentan cursos, estudiantes e indicadores; aporta el manejo de errores (RNF-03). | Sí |
| 4 | RF-04 | Gestión de cursos | Depende de RF-01 y RF-03. Es la puerta de entrada del docente a la información. | Sí |
| 5 | RF-05 | Panel de indicadores | Depende de RF-03 y RF-04. Entrega el primer valor visible al docente. Se limita a una versión básica. | Sí (versión básica) |
| 6 | RF-06 | Seguimiento individual | Requiere datos ya sincronizados y estables; se apoya en RF-03 y RF-05. | No |
| 7 | RF-07 | Alertas académicas | Requiere reglas configurables y datos históricos consolidados. | No |
| 8 | RF-08 | Análisis grupal | Se apoya en los indicadores de RF-05 y las alertas de RF-07. | No |
| 9 | RF-12 | Auditoría | El Sprint 1 solo registra el resultado de las sincronizaciones (RN-06); la auditoría completa se aborda con el agente. | No |
| 10 | RF-09 | Agente de IA | Requiere servicios de IA (Python/FastAPI, LangGraph, Ollama) y datos confiables. No se implementa en el Sprint 1. | No |
| 11 | RF-10 | Recomendaciones | Depende de RF-09 e indicadores consolidados. | No |
| 12 | RF-11 | Informe | Depende de indicadores, hallazgos y recomendaciones previos. | No |

**Por qué los requerimientos seleccionados son necesarios para el objetivo:** RF-01 garantiza el acceso seguro y por curso; RF-02 y RF-03 proveen y validan los datos de Moodle; RF-04 permite que el docente acceda a sus cursos y estudiantes; y RF-05 (versión básica) materializa el valor del Sprint al mostrar los indicadores. En conjunto forman una cadena de dependencias completa y verificable de extremo a extremo, además de establecer la arquitectura de microservicios.

---

# 12. ANÁLISIS DE CAPACIDAD DEL SPRINT 1

Al ser el primer Sprint del proyecto **no existe velocidad histórica** del equipo, por lo que no se utiliza. La capacidad se calcula con los siguientes **supuestos declarados**:

- Días de ejecución: 21, 22, 23, 26, 27, 28 y 29 de octubre de 2026 (7 días hábiles).
- Dedicación supuesta: 4 horas diarias por integrante (28 horas brutas por persona).
- Reserva para imprevistos: 10 % de las horas brutas (incluye la reunión diaria de 15 minutos).
- Los días 19 y 20 de octubre se destinan al arranque del Sprint y a la reunión de planificación, y no se cuentan como capacidad de ejecución.

| Integrante | Responsabilidad técnica | Horas brutas | Reserva para imprevistos | Horas netas | Horas planificadas | Holgura |
|---|---|---|---|---|---|---|
| Curillo Pilamunga Diego Alexander | Backend .NET 10 e integración con Moodle | 28 h | 2,8 h | 25,2 h | 18 h | 7,2 h |
| Manobanda Masabanda Luis Alfredo | Modelo de datos, autorización y sincronización | 28 h | 2,8 h | 25,2 h | 18,5 h | 6,7 h |
| Redroban Chimbo Joselyn Amparo | React.js, endpoints de consulta e indicadores | 28 h | 2,8 h | 25,2 h | 19 h | 6,2 h |
| Quispe Nasimba Daniela Alejandra | React.js y pruebas | 28 h | 2,8 h | 25,2 h | 18,5 h | 6,7 h |
| **Total** | | **112 h** | **11,2 h** | **100,8 h** | **74 h** | **26,8 h** |

Las horas planificadas (74 h) no superan la capacidad neta (100,8 h) y representan aproximadamente el 73 % de ella, por lo que existe una holgura de 26,8 horas para imprevistos, integración y ajustes. Los puntos de historia (36 SP) miden complejidad relativa y no se convierten directamente en horas.

---

# 13. HISTORIAS DE USUARIO SELECCIONADAS

## 13.1 Tabla resumen

| Código HU | Código RF | Nombre de la historia | Historia de Usuario (Como / Quiero / Para) | Prioridad | Estimación |
|---|---|---|---|---|---|
| HU-01 | RF-01 | Iniciar sesión en el sistema | **Como** docente o administrador técnico, **quiero** iniciar sesión con mis credenciales, **para** acceder de forma segura a las funcionalidades que corresponden a mi rol. | Crítica | 5 SP |
| HU-02 | RF-01 | Restringir el acceso por rol y por curso | **Como** docente, **quiero** que el sistema me muestre únicamente los cursos y estudiantes que tengo autorizados, **para** proteger la privacidad de la información académica. | Crítica | 3 SP |
| HU-03 | RF-02 | Configurar y verificar la conexión con Moodle | **Como** administrador técnico, **quiero** registrar la URL y verificar la conexión con los servicios web de Moodle, **para** asegurar que el sistema pueda obtener información académica de forma segura. | Crítica | 5 SP |
| HU-04 | RF-03 | Sincronizar cursos y matriculados | **Como** administrador técnico, **quiero** sincronizar los cursos y estudiantes matriculados desde Moodle, **para** contar con la estructura académica en el sistema. | Alta | 5 SP |
| HU-05 | RF-03 | Sincronizar calificaciones y estados de actividad | **Como** administrador técnico, **quiero** sincronizar los ítems de calificación, las notas y los estados de actividad y consultar el registro de errores, **para** disponer de datos confiables y conocer cuando la sincronización sea incompleta. | Alta | 8 SP |
| HU-06 | RF-04 | Seleccionar curso y listar estudiantes | **Como** docente, **quiero** seleccionar uno de mis cursos y consultar su listado de estudiantes con búsqueda y filtros, **para** localizar rápidamente a quienes necesito revisar. | Alta | 5 SP |
| HU-07 | RF-05 | Consultar el panel básico de indicadores | **Como** docente, **quiero** ver el promedio con su escala, la distribución de calificaciones, las actividades evaluadas y el progreso disponible de un curso, **para** tener una visión general del rendimiento del grupo. | Alta | 5 SP |
| | | **Total** | | | **36 SP** |

## 13.2 Fichas, mockups y desglose de tareas por Historia de Usuario

Cada historia incluye su ficha (con un programador responsable principal), el mockup que la ilustra y el desglose de sus tareas con responsable, colaborador, horas y fechas. Las 7 historias suman 36 SP y 74 horas. El programador responsable de cada ficha es quien concentra la mayor carga de tareas de la historia y responde por su integración; los demás integrantes participan como responsables o colaboradores de tareas específicas, según se detalla en cada tarea. Las fechas de cada tarea corresponden al cronograma de la sección 19.2.

### HU-01 — Iniciar sesión en el sistema

| Historia de Usuario | |
|---|---|
| **Número:** 1 | **Usuario:** Docente y Administrador técnico |
| **Nombre historia:** Iniciar sesión en el sistema | |
| **Prioridad en negocio:** Crítica | **Riesgo en desarrollo:** Media |
| **Puntos estimados:** 5 SP (12,5 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | |
| **Descripción:** Como docente o administrador técnico, quiero iniciar sesión con mis credenciales, para acceder de forma segura a las funcionalidades que corresponden a mi rol. (RF-01, RN-01) | |
| **Validación (criterios de aceptación):**<br>• CA-01.1: Dado un usuario con credenciales válidas, cuando pulsa Ingresar, entonces recibe un JWT con su rol y es redirigido a la pantalla inicial de ese rol.<br>• CA-01.2: Dado un usuario con credenciales incorrectas, cuando pulsa Ingresar, entonces ve el mensaje «Credenciales incorrectas» sin indicar qué dato falló y no se emite token.<br>• CA-01.3: Dado el almacenamiento de usuarios, cuando se revisa la base de datos, entonces las contraseñas están guardadas con hash y ninguna en texto plano.<br>• CA-01.4: Dado un usuario con sesión iniciada, cuando cierra sesión, entonces el token se elimina del cliente y el acceso a una ruta privada lo redirige al inicio de sesión. | |
| **Mockups asociados:** M-01 (Inicio de sesión) | |

*Mockup de HU-01*

**Mockup M-01 — Inicio de sesión** (rol: Público; ruta: /login)

```
INICIO DE SESIÓN                              Rol: Público
──────────────────────────────────────────────────────
                     AcademIA
          Usuario      [ ______________________ ]
          Contraseña   [ ______________________ ]
          ⚠ Credenciales incorrectas
                     [ Ingresar ]
──────────────────────────────────────────────────────
Versión 1.0.0
```
*Figura 1. Mockup M-01 — Inicio de sesión*

*Desglose de tareas — HU-01*

| Tarea | |
|---|---|
| **Número tarea:** TA-01.1 | **Número historia:** HU-01 |
| **Nombre tarea:** Estructura de la solución y del repositorio | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 21/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Crear el repositorio Git y la solución .NET 10 del Servicio de Identidad con Clean Architecture (Domain, Application, Infrastructure y API), las convenciones de nombres y la documentación Swagger inicial. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-01.2 | **Número historia:** HU-01 |
| **Nombre tarea:** Modelo de usuarios y roles | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Definir en el Servicio de Identidad las entidades Usuario y Rol (Docente y Administrador técnico) con EF Core, su migración en SQL Server y usuarios semilla de prueba anonimizados. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-01.3 | **Número historia:** HU-01 |
| **Nombre tarea:** Endpoint de inicio de sesión (POST /auth/login) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 22/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Validar usuario y contraseña contra el hash almacenado, emitir un JWT con el rol del usuario y responder con el mismo mensaje ante cualquier credencial incorrecta. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-01.4 | **Número historia:** HU-01 |
| **Nombre tarea:** Pantalla de inicio de sesión (M-01) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 21/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Quispe Nasimba Daniela Alejandra |
| **Descripción:** Construir en React la pantalla /login según M-01: validación de campos, llamada al endpoint de login, almacenamiento del token, mensaje de error y cierre de sesión. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-01.5 | **Número historia:** HU-01 |
| **Nombre tarea:** Pruebas del inicio y cierre de sesión | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 22/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Escribir pruebas xUnit del login (credenciales válidas, usuario inexistente y contraseña errónea) y la colección de Postman del flujo de inicio y cierre de sesión. | |

### HU-02 — Restringir el acceso por rol y por curso

| Historia de Usuario | |
|---|---|
| **Número:** 2 | **Usuario:** Docente |
| **Nombre historia:** Restringir el acceso por rol y por curso | |
| **Prioridad en negocio:** Crítica | **Riesgo en desarrollo:** Media |
| **Puntos estimados:** 3 SP (6,0 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | |
| **Descripción:** Como docente, quiero que el sistema me muestre únicamente los cursos y estudiantes que tengo autorizados, para proteger la privacidad de la información académica. (RF-01, RN-02, RN-03, RNF-01) | |
| **Validación (criterios de aceptación):**<br>• CA-02.1: Dado un token con rol Docente, cuando intenta acceder a la configuración de Moodle, entonces el servicio responde 403 (RN-03).<br>• CA-02.2: Dada una petición sin token o con token vencido, cuando llega a un endpoint protegido, entonces responde 401 y la interfaz regresa al inicio de sesión.<br>• CA-02.3: Dado un docente autenticado, cuando consulta sus cursos, entonces solo recibe los cursos que tiene asignados (RN-02).<br>• CA-02.4: Dado un usuario autenticado, cuando observa el menú, entonces solo aparecen las opciones de su rol. | |
| **Mockups asociados:** M-02 (Menú por rol) | |

*Mockup de HU-02*

**Mockup M-02 — Menú por rol** (rol: Docente y Administrador técnico; ruta: /inicio)

```
INICIO                         usuario@ejemplo | Cerrar sesión
──────────────────────────────────────────────────────
[ Cursos ]   [ Sincronización* ]   [ Configuración Moodle* ]

  * Visibles solo para el Administrador técnico.
  El Docente ve únicamente la opción «Cursos».
  Acceso no permitido → «Acceso no autorizado» (403).
──────────────────────────────────────────────────────
Versión 1.0.0
```
*Figura 2. Mockup M-02 — Menú por rol*

*Desglose de tareas — HU-02*

| Tarea | |
|---|---|
| **Número tarea:** TA-02.1 | **Número historia:** HU-02 |
| **Nombre tarea:** Políticas de autorización por rol y por curso | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 22/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Crear la entidad de asignación docente–curso y las políticas de autorización por rol y por curso en los servicios, respondiendo 401 sin token válido y 403 sin permiso. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-02.2 | **Número historia:** HU-02 |
| **Nombre tarea:** Menú y rutas protegidas según el rol (M-02) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 22/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Quispe Nasimba Daniela Alejandra |
| **Descripción:** Leer el rol del token, mostrar en React el menú que corresponde (M-02), proteger las rutas y redirigir al inicio de sesión ante una respuesta 401. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-02.3 | **Número historia:** HU-02 |
| **Nombre tarea:** Pruebas de permisos | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Probar con xUnit y Postman el acceso con token vencido, con rol sin permiso y de un docente que consulta un curso no asignado. | |

### HU-03 — Configurar y verificar la conexión con Moodle

| Historia de Usuario | |
|---|---|
| **Número:** 3 | **Usuario:** Administrador técnico |
| **Nombre historia:** Configurar y verificar la conexión con Moodle | |
| **Prioridad en negocio:** Crítica | **Riesgo en desarrollo:** Alta |
| **Puntos estimados:** 5 SP (11,5 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | |
| **Descripción:** Como administrador técnico, quiero registrar la URL y verificar la conexión con los servicios web de Moodle, para asegurar que el sistema pueda obtener información académica de forma segura. (RF-02, RN-03, RN-04) | |
| **Validación (criterios de aceptación):**<br>• CA-03.1: Dado un administrador técnico, cuando registra la URL de Moodle, entonces esta se guarda y la credencial técnica se lee desde variables de entorno, sin devolverse en la API ni escribirse en registros (RN-04).<br>• CA-03.2: Dada una configuración válida, cuando pulsa Verificar conexión, entonces se muestra «Conexión exitosa» con el nombre del sitio y el tiempo de respuesta.<br>• CA-03.3: Dada una URL inaccesible o una credencial rechazada, cuando se verifica la conexión, entonces se muestra un mensaje diferenciado y útil sin revelar datos sensibles.<br>• CA-03.4: Dado un usuario con rol Docente, cuando intenta abrir o modificar la configuración de Moodle, entonces el acceso es denegado (403). | |
| **Mockups asociados:** M-03 (Configuración de Moodle) | |

*Mockup de HU-03*

**Mockup M-03 — Configuración de Moodle** (rol: Administrador técnico; ruta: /configuracion/moodle)

```
CONFIGURACIÓN DE MOODLE                  Rol: Administrador técnico
──────────────────────────────────────────────────────
  URL de Moodle            [ https://______________ ]
  Credencial técnica       ● Configurada (no se muestra)

  [ Guardar ]   [ Verificar conexión ]

  ✔ Conexión exitosa · Sitio: ______ · Respuesta: ___ ms
──────────────────────────────────────────────────────
Versión 1.0.0
```
*Figura 3. Mockup M-03 — Configuración de Moodle*

*Desglose de tareas — HU-03*

| Tarea | |
|---|---|
| **Número tarea:** TA-03.1 | **Número historia:** HU-03 |
| **Nombre tarea:** Servicio de Integración con Moodle y cliente REST | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Crear el Servicio de Integración con Moodle con Clean Architecture y el cliente HTTP para los servicios web REST de Moodle, incluida la consulta de información del sitio. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-03.2 | **Número historia:** HU-03 |
| **Nombre tarea:** Configuración de Moodle (URL y secretos) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 22/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Implementar la entidad de configuración (URL) y los endpoints de consulta y actualización restringidos al Administrador técnico, leyendo la credencial técnica desde variables de entorno sin exponerla en respuestas ni registros. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-03.3 | **Número historia:** HU-03 |
| **Nombre tarea:** Verificación de la conexión | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Implementar el endpoint de verificación que devuelve el nombre del sitio y el tiempo de respuesta, con mensajes diferenciados para URL inaccesible y credencial rechazada. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-03.4 | **Número historia:** HU-03 |
| **Nombre tarea:** Pantalla de configuración de Moodle (M-03) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 21/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Redroban Chimbo Joselyn Amparo |
| **Descripción:** Construir en React la pantalla M-03 con el campo de URL, el indicador de credencial técnica configurada, el botón Verificar conexión y el resultado de la verificación. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-03.5 | **Número historia:** HU-03 |
| **Nombre tarea:** Pruebas de la conexión | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Probar con respuestas simuladas de Moodle los casos de conexión válida, URL inválida y credencial inválida, y comprobar que no se exponen secretos. | |

### HU-04 — Sincronizar cursos y matriculados

| Historia de Usuario | |
|---|---|
| **Número:** 4 | **Usuario:** Administrador técnico |
| **Nombre historia:** Sincronizar cursos y matriculados | |
| **Prioridad en negocio:** Alta | **Riesgo en desarrollo:** Media |
| **Puntos estimados:** 5 SP (11,0 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | |
| **Descripción:** Como administrador técnico, quiero sincronizar los cursos y estudiantes matriculados desde Moodle, para contar con la estructura académica en el sistema. (RF-03, RN-05, RN-06) | |
| **Validación (criterios de aceptación):**<br>• CA-04.1: Dada una conexión verificada, cuando se ejecuta la sincronización, entonces los cursos y matriculados de Moodle quedan almacenados en SQL Server.<br>• CA-04.2: Dado que la sincronización se ejecuta más de una vez, cuando finaliza, entonces no se duplican cursos, estudiantes ni matrículas.<br>• CA-04.3: Dada una sincronización finalizada, cuando se consulta la bitácora, entonces existe un registro con fecha y hora, resultado y errores (RN-06).<br>• CA-04.4: Dado un conjunto grande de datos, cuando se consulta el resultado de la sincronización, entonces la consulta es paginada (RN-11). | |
| **Mockups asociados:** M-04 (Sincronización) | |

*Mockup de HU-04*

**Mockup M-04 — Sincronización** (rol: Administrador técnico; ruta: /sincronizacion)

```
SINCRONIZACIÓN                          Rol: Administrador técnico
──────────────────────────────────────────────────────
  [ Sincronizar ]      Última sincronización: __/__/____ __:__
  Estado: ● Completa   ○ Incompleta

  Fecha        | Resultado   | Cursos | Errores
  ─────────────┼─────────────┼────────┼──────────
  __/__/____   | Completa    |   _    | 0
  __/__/____   | Incompleta  |   _    | Ver detalle
──────────────────────────────────────────────────────
Versión 1.0.0
```
*Figura 4. Mockup M-04 — Sincronización*

*Desglose de tareas — HU-04*

| Tarea | |
|---|---|
| **Número tarea:** TA-04.1 | **Número historia:** HU-04 |
| **Nombre tarea:** Servicio Académico y modelo de datos de cursos | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 21/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Crear la solución del Servicio Académico y el modelo de datos de Curso, Estudiante, Matrícula y Bitácora de sincronización, con su migración en SQL Server. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-04.2 | **Número historia:** HU-04 |
| **Nombre tarea:** Consumo de cursos y matriculados de Moodle | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 26/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Consumir los servicios web de Moodle para cursos y usuarios matriculados, con paginación, y mapear las respuestas a objetos del dominio. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-04.3 | **Número historia:** HU-04 |
| **Nombre tarea:** Caso de uso de sincronización idempotente | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 26/10/2026 | **Fecha fin:** 26/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Implementar el caso de uso que sincroniza cursos y matriculados sin duplicar registros cuando se ejecuta más de una vez. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-04.4 | **Número historia:** HU-04 |
| **Nombre tarea:** Bitácora de sincronización | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Registrar en la bitácora la fecha y hora, el resultado y los errores de cada sincronización y exponer su consulta paginada. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-04.5 | **Número historia:** HU-04 |
| **Nombre tarea:** Pruebas de la sincronización de cursos | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 26/10/2026 | **Fecha fin:** 26/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Escribir pruebas unitarias y de integración del mapeo de datos y de la idempotencia de la sincronización. | |

### HU-05 — Sincronizar calificaciones y estados de actividad

| Historia de Usuario | |
|---|---|
| **Número:** 5 | **Usuario:** Administrador técnico |
| **Nombre historia:** Sincronizar calificaciones y estados de actividad | |
| **Prioridad en negocio:** Alta | **Riesgo en desarrollo:** Alta |
| **Puntos estimados:** 8 SP (14,5 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | |
| **Descripción:** Como administrador técnico, quiero sincronizar los ítems de calificación, las notas y los estados de actividad y consultar el registro de errores, para disponer de datos confiables y conocer cuando la sincronización sea incompleta. (RF-03, RN-05, RN-06, RN-07, RNF-03) | |
| **Validación (criterios de aceptación):**<br>• CA-05.1: Dados cursos sincronizados, cuando se sincronizan las calificaciones, entonces los ítems, notas y estados de actividad se almacenan junto con la escala de cada ítem.<br>• CA-05.2: Dado que Moodle no entrega algún dato por permisos o error de API, cuando termina la sincronización, entonces se registra el error, se marca como «Incompleta» y los datos faltantes se identifican como no disponibles (RN-05, RN-07).<br>• CA-05.3: Dada una nota ausente en Moodle, cuando se almacena, entonces se guarda como «sin dato» y no como cero.<br>• CA-05.4: Dado un administrador técnico, cuando abre la pantalla de sincronización, entonces ve la fecha de la última sincronización, su estado y el detalle de errores. | |
| **Mockups asociados:** M-04 (Sincronización (compartido con HU-04)) | |

*Mockup de HU-05*

**Mockup M-04 — Sincronización (compartido con HU-04)** (rol: Administrador técnico; ruta: /sincronizacion)

Esta historia utiliza el mockup M-04 presentado en HU-04, ampliado con el estado «Completa / Incompleta», el conteo de registros importados y el detalle de errores por curso.

*Desglose de tareas — HU-05*

| Tarea | |
|---|---|
| **Número tarea:** TA-05.1 | **Número historia:** HU-05 |
| **Nombre tarea:** Modelo de calificaciones y estados de actividad | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 26/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Definir las entidades Ítem de calificación (con su escala), Calificación y Estado de actividad, con migración en SQL Server y soporte del valor «sin dato». | |

| Tarea | |
|---|---|
| **Número tarea:** TA-05.2 | **Número historia:** HU-05 |
| **Nombre tarea:** Consumo de notas y estados desde Moodle | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,5 h |
| **Fecha inicio:** 26/10/2026 | **Fecha fin:** 27/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Consumir los servicios web de Moodle para ítems de calificación, notas y estados de actividad, manejando permisos insuficientes y funciones no habilitadas. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-05.3 | **Número historia:** HU-05 |
| **Nombre tarea:** Sincronización de calificaciones y estados | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 26/10/2026 | **Fecha fin:** 27/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Implementar el caso de uso de sincronización de calificaciones y estados, marcando la sincronización como «Incompleta» ante errores y guardando las notas ausentes como «sin dato». | |

| Tarea | |
|---|---|
| **Número tarea:** TA-05.4 | **Número historia:** HU-05 |
| **Nombre tarea:** Endpoints de sincronización y estado | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 27/10/2026 | **Fecha fin:** 27/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Implementar los endpoints para ejecutar la sincronización y para consultar su estado, fecha y errores. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-05.5 | **Número historia:** HU-05 |
| **Nombre tarea:** Pantalla de sincronización (M-04) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,5 h |
| **Fecha inicio:** 26/10/2026 | **Fecha fin:** 27/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Quispe Nasimba Daniela Alejandra |
| **Descripción:** Construir en React la pantalla M-04: botón de sincronización, estado, fecha de la última sincronización y detalle de errores. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-05.6 | **Número historia:** HU-05 |
| **Nombre tarea:** Pruebas de errores y datos ausentes | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 27/10/2026 | **Fecha fin:** 27/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Probar el manejo de errores de la API de Moodle, los datos ausentes y la sincronización incompleta. | |

### HU-06 — Seleccionar curso y listar estudiantes

| Historia de Usuario | |
|---|---|
| **Número:** 6 | **Usuario:** Docente |
| **Nombre historia:** Seleccionar curso y listar estudiantes | |
| **Prioridad en negocio:** Alta | **Riesgo en desarrollo:** Media |
| **Puntos estimados:** 5 SP (9,5 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | |
| **Descripción:** Como docente, quiero seleccionar uno de mis cursos y consultar su listado de estudiantes con búsqueda y filtros, para localizar rápidamente a quienes necesito revisar. (RF-04, RN-02, RN-11) | |
| **Validación (criterios de aceptación):**<br>• CA-06.1: Dado un docente autenticado, cuando abre la sección de cursos, entonces puede seleccionar curso y período únicamente entre los autorizados.<br>• CA-06.2: Dado un curso seleccionado, cuando se abre su detalle, entonces se lista a sus estudiantes de forma paginada (RN-11).<br>• CA-06.3: Dado el listado de estudiantes, cuando el docente busca por nombre o aplica el filtro con o sin calificaciones, entonces se muestran solo los resultados coincidentes.<br>• CA-06.4: Dado un docente que consulta un curso no autorizado por identificador directo, cuando envía la petición, entonces el sistema responde 403. | |
| **Mockups asociados:** M-05 (Cursos y estudiantes) | |

*Mockup de HU-06*

**Mockup M-05 — Cursos y estudiantes** (rol: Docente; ruta: /cursos)

```
CURSOS Y ESTUDIANTES                     Rol: Docente
──────────────────────────────────────────────────────
  Curso [ ____________ ▾ ]   Período [ ________ ▾ ]
  Buscar [ ______________ ]  Filtro [ Todos ▾ ]

  Estudiante            | Calificaciones registradas
  ──────────────────────┼───────────────────────────
  ____________________  | Sí
  ____________________  | No
  ◀ 1 2 3 ▶
──────────────────────────────────────────────────────
Versión 1.0.0
```
*Figura 5. Mockup M-05 — Cursos y estudiantes*

*Desglose de tareas — HU-06*

| Tarea | |
|---|---|
| **Número tarea:** TA-06.1 | **Número historia:** HU-06 |
| **Nombre tarea:** Endpoints de cursos y estudiantes | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 22/10/2026 | **Fecha fin:** 23/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Curillo Pilamunga Diego Alexander |
| **Descripción:** Implementar los endpoints de cursos autorizados y de estudiantes por curso, con paginación, búsqueda por nombre y filtro con o sin calificaciones registradas. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-06.2 | **Número historia:** HU-06 |
| **Nombre tarea:** Pantalla de selección de curso y período (M-05) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 2,0 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Quispe Nasimba Daniela Alejandra |
| **Descripción:** Construir en React la selección de curso y período según M-05, mostrando solo los cursos autorizados. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-06.3 | **Número historia:** HU-06 |
| **Nombre tarea:** Tabla de estudiantes con búsqueda y filtros (M-05) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 21/10/2026 | **Fecha fin:** 22/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Redroban Chimbo Joselyn Amparo |
| **Descripción:** Construir en React la tabla de estudiantes con cuadro de búsqueda, filtro y paginación según M-05. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-06.4 | **Número historia:** HU-06 |
| **Nombre tarea:** Pruebas de autorización, paginación y filtros | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 28/10/2026 | **Fecha fin:** 28/10/2026 |
| **Programador responsable:** Quispe Nasimba Daniela Alejandra | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Probar el acceso 403 a cursos no autorizados y el comportamiento de la paginación, la búsqueda y los filtros. | |

### HU-07 — Consultar el panel básico de indicadores

| Historia de Usuario | |
|---|---|
| **Número:** 7 | **Usuario:** Docente |
| **Nombre historia:** Consultar el panel básico de indicadores | |
| **Prioridad en negocio:** Alta | **Riesgo en desarrollo:** Media |
| **Puntos estimados:** 5 SP (9,0 h de tareas) | **Iteración asignada:** 1 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | |
| **Descripción:** Como docente, quiero ver el promedio con su escala, la distribución de calificaciones, las actividades evaluadas y el progreso disponible de un curso, para tener una visión general del rendimiento del grupo. (RF-05, RN-07, RN-08, RN-09) | |
| **Validación (criterios de aceptación):**<br>• CA-07.1: Dado un curso con datos sincronizados, cuando el docente abre el panel, entonces se muestra el promedio con su escala explícita (RN-08).<br>• CA-07.2: Dado el mismo curso, cuando se abre el panel, entonces se muestran la distribución de calificaciones por rangos, las actividades evaluadas y el progreso disponible.<br>• CA-07.3: Dado un curso con sincronización incompleta o sin datos, cuando se abre el panel, entonces se muestra una advertencia y los indicadores no disponibles aparecen como «sin datos» (RN-07).<br>• CA-07.4: Dado cualquier indicador mostrado, cuando se observa el panel, entonces se indica la fuente (Moodle) y la fecha de la última sincronización (RN-09).<br>• CA-07.5: Dado un conjunto de calificaciones conocido, cuando se calculan los indicadores, entonces los resultados coinciden con los valores calculados manualmente. | |
| **Mockups asociados:** M-06 (Panel de indicadores) | |

*Mockup de HU-07*

**Mockup M-06 — Panel de indicadores** (rol: Docente; ruta: /cursos/{id}/panel)

```
PANEL DE INDICADORES — Curso: ________      Rol: Docente
──────────────────────────────────────────────────────
  ⚠ Datos incompletos (solo si la sincronización lo fue)
  ┌───────────────┐ ┌───────────────┐ ┌───────────────┐
  │ Promedio      │ │ Actividades   │ │ Progreso      │
  │ __ / (escala) │ │ evaluadas: __ │ │ ▓▓▓▓░░ __ %   │
  └───────────────┘ └───────────────┘ └───────────────┘
  Distribución de calificaciones
   rango A ▇▇▇▇   rango B ▇▇▇   rango C ▇▇   rango D ▇
──────────────────────────────────────────────────────
  Fuente: Moodle · Última sincronización: __/__/____ __:__
```
*Figura 6. Mockup M-06 — Panel de indicadores*

*Desglose de tareas — HU-07*

| Tarea | |
|---|---|
| **Número tarea:** TA-07.1 | **Número historia:** HU-07 |
| **Nombre tarea:** Cálculo de indicadores | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,5 h |
| **Fecha inicio:** 23/10/2026 | **Fecha fin:** 26/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Manobanda Masabanda Luis Alfredo |
| **Descripción:** Implementar en la capa de aplicación el cálculo del promedio con su escala, la distribución por rangos, las actividades evaluadas y el progreso disponible, con pruebas unitarias xUnit. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-07.2 | **Número historia:** HU-07 |
| **Nombre tarea:** Endpoint de indicadores | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 1,0 h |
| **Fecha inicio:** 27/10/2026 | **Fecha fin:** 27/10/2026 |
| **Programador responsable:** Curillo Pilamunga Diego Alexander | **Colaborador:** Redroban Chimbo Joselyn Amparo |
| **Descripción:** Exponer el endpoint de indicadores por curso, incluyendo la fuente, la fecha de la última sincronización y la advertencia de datos incompletos. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-07.3 | **Número historia:** HU-07 |
| **Nombre tarea:** Panel de indicadores (M-06) | |
| **Tipo de tarea:** ☒ Desarrollo ☐ Corrección ☐ Mejora ☐ Otra | **Horas estimadas:** 3,0 h |
| **Fecha inicio:** 27/10/2026 | **Fecha fin:** 28/10/2026 |
| **Programador responsable:** Manobanda Masabanda Luis Alfredo | **Colaborador:** Redroban Chimbo Joselyn Amparo |
| **Descripción:** Construir en React el panel M-06 con tarjetas, gráfico de distribución, progreso, advertencias y pie de fuente y fecha. | |

| Tarea | |
|---|---|
| **Número tarea:** TA-07.4 | **Número historia:** HU-07 |
| **Nombre tarea:** Pruebas del panel de indicadores | |
| **Tipo de tarea:** ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra | **Horas estimadas:** 1,5 h |
| **Fecha inicio:** 28/10/2026 | **Fecha fin:** 28/10/2026 |
| **Programador responsable:** Redroban Chimbo Joselyn Amparo | **Colaborador:** Quispe Nasimba Daniela Alejandra |
| **Descripción:** Probar el panel con un curso completo y con uno de datos incompletos, comparando con cálculos manuales. | |

---

# 14. CRITERIOS DE ACEPTACIÓN

| Código HU | N.º | Criterio de aceptación | Resultado verificable |
|---|---|---|---|
| HU-01 | CA-01.1 | **Dado** un usuario con credenciales válidas, **cuando** pulsa «Ingresar», **entonces** recibe un JWT que contiene su rol y es redirigido a la pantalla inicial de ese rol. | Respuesta 200 del endpoint de login con token que incluye el claim de rol; redirección observable en la interfaz. |
| HU-01 | CA-01.2 | **Dado** un usuario con credenciales incorrectas, **cuando** pulsa «Ingresar», **entonces** se muestra el mensaje «Credenciales incorrectas» sin indicar qué dato falló y no se emite token. | Respuesta 401 sin token; mismo mensaje para usuario inexistente y contraseña errónea. |
| HU-01 | CA-01.3 | **Dado** el almacenamiento de usuarios, **cuando** se revisa la base de datos, **entonces** las contraseñas están guardadas con hash y ninguna está en texto plano. | Inspección de la tabla de usuarios en SQL Server. |
| HU-01 | CA-01.4 | **Dado** un usuario con sesión iniciada, **cuando** cierra sesión, **entonces** el token se elimina del cliente y el acceso a una ruta privada lo redirige al inicio de sesión. | Prueba manual en la interfaz y prueba automatizada de la ruta protegida. |
| HU-02 | CA-02.1 | **Dado** un token con rol Docente, **cuando** intenta acceder a la configuración de Moodle, **entonces** el servicio responde 403 (RN-03). | Respuesta 403 verificada con Postman. |
| HU-02 | CA-02.2 | **Dado** una petición sin token o con token vencido, **cuando** llega a un endpoint protegido, **entonces** responde 401 y la interfaz regresa al inicio de sesión. | Respuesta 401 en Postman y redirección en la interfaz. |
| HU-02 | CA-02.3 | **Dado** un docente autenticado, **cuando** consulta sus cursos, **entonces** solo recibe los cursos que tiene asignados (RN-02). | Prueba automatizada con dos docentes de prueba y cursos distintos. |
| HU-02 | CA-02.4 | **Dado** un usuario autenticado, **cuando** observa el menú, **entonces** solo aparecen las opciones de su rol. | Revisión visual con un usuario por rol. |
| HU-03 | CA-03.1 | **Dado** un administrador técnico, **cuando** registra la URL de Moodle, **entonces** esta se guarda y la credencial técnica se lee desde variables de entorno, sin devolverse en ninguna respuesta de la API ni escribirse en registros (RN-04). | Revisión de respuestas de la API, de los registros y del repositorio sin secretos. |
| HU-03 | CA-03.2 | **Dado** una configuración válida, **cuando** pulsa «Verificar conexión», **entonces** el sistema muestra «Conexión exitosa» con el nombre del sitio Moodle y el tiempo de respuesta. | Resultado en la interfaz contra la instancia Moodle de pruebas. |
| HU-03 | CA-03.3 | **Dado** una URL inaccesible o una credencial rechazada, **cuando** se verifica la conexión, **entonces** se muestra un mensaje diferenciado y útil sin revelar datos sensibles. | Pruebas automatizadas con respuestas simuladas de Moodle (URL inválida y token inválido). |
| HU-03 | CA-03.4 | **Dado** un usuario con rol Docente, **cuando** intenta abrir o modificar la configuración de Moodle, **entonces** el acceso es denegado (403). | Respuesta 403 verificada con Postman. |
| HU-04 | CA-04.1 | **Dado** una conexión verificada, **cuando** se ejecuta la sincronización, **entonces** los cursos y matriculados de Moodle quedan almacenados en SQL Server. | Conteo de registros en la base de datos comparado con el origen de pruebas. |
| HU-04 | CA-04.2 | **Dado** que la sincronización se ejecuta más de una vez, **cuando** finaliza, **entonces** no se duplican cursos, estudiantes ni matrículas. | Prueba automatizada de idempotencia. |
| HU-04 | CA-04.3 | **Dado** una sincronización finalizada, **cuando** se consulta la bitácora, **entonces** existe un registro con fecha y hora, resultado y errores (RN-06). | Registro visible en la bitácora y en la pantalla de sincronización. |
| HU-04 | CA-04.4 | **Dado** un conjunto grande de datos, **cuando** se consulta el resultado de la sincronización, **entonces** la consulta es paginada (RN-11). | Respuesta de la API con parámetros de página y tamaño. |
| HU-05 | CA-05.1 | **Dado** cursos sincronizados, **cuando** se sincronizan las calificaciones, **entonces** los ítems, notas y estados de actividad se almacenan junto con la escala de cada ítem. | Revisión de las tablas en SQL Server y de la escala asociada. |
| HU-05 | CA-05.2 | **Dado** que Moodle no entrega algún dato por permisos o error de API, **cuando** termina la sincronización, **entonces** se registra el error, la sincronización se marca como «Incompleta» y los datos faltantes se identifican como no disponibles (RN-05, RN-07). | Prueba automatizada con respuesta simulada de error y estado «Incompleta» en la interfaz. |
| HU-05 | CA-05.3 | **Dado** una nota ausente en Moodle, **cuando** se almacena, **entonces** se guarda como «sin dato» y no como cero. | Prueba unitaria del mapeo de notas ausentes. |
| HU-05 | CA-05.4 | **Dado** un administrador técnico, **cuando** abre la pantalla de sincronización, **entonces** ve la fecha de la última sincronización, su estado y el detalle de errores. | Pantalla M-04 mostrando estos datos. |
| HU-06 | CA-06.1 | **Dado** un docente autenticado, **cuando** abre la sección de cursos, **entonces** puede seleccionar un curso y período únicamente entre los autorizados. | Prueba con dos docentes de prueba. |
| HU-06 | CA-06.2 | **Dado** un curso seleccionado, **cuando** se abre su detalle, **entonces** se lista a sus estudiantes de forma paginada (RN-11). | Respuesta paginada de la API y tabla paginada en la interfaz. |
| HU-06 | CA-06.3 | **Dado** el listado de estudiantes, **cuando** el docente busca por nombre o aplica el filtro «con/sin calificaciones», **entonces** se muestran solo los resultados coincidentes. | Prueba automatizada del filtrado y verificación manual. |
| HU-06 | CA-06.4 | **Dado** un docente que intenta consultar un curso no autorizado (por identificador directo), **cuando** envía la petición, **entonces** el sistema responde 403. | Respuesta 403 verificada con Postman. |
| HU-07 | CA-07.1 | **Dado** un curso con datos sincronizados, **cuando** el docente abre el panel, **entonces** se muestra el promedio con su escala explícita (RN-08). | Panel M-06 con el promedio acompañado de su escala. |
| HU-07 | CA-07.2 | **Dado** el mismo curso, **cuando** se abre el panel, **entonces** se muestran la distribución de calificaciones por rangos, el número de actividades evaluadas y el progreso disponible. | Verificación visual contra el conjunto de datos de prueba. |
| HU-07 | CA-07.3 | **Dado** un curso con sincronización incompleta o sin datos, **cuando** se abre el panel, **entonces** se muestra una advertencia y los indicadores no disponibles aparecen como «sin datos», sin valores engañosos (RN-07). | Prueba con un curso de datos incompletos. |
| HU-07 | CA-07.4 | **Dado** cualquier indicador mostrado, **cuando** se observa el panel, **entonces** se indica la fuente (Moodle) y la fecha de la última sincronización (RN-09). | Pie del panel con fuente y fecha. |
| HU-07 | CA-07.5 | **Dado** un conjunto de calificaciones conocido, **cuando** se calculan los indicadores, **entonces** los resultados coinciden con los valores calculados manualmente. | Pruebas unitarias del cálculo de indicadores aprobadas (RNF-06). |

---

# 15. ESTIMACIÓN DE LAS HISTORIAS DE USUARIO

La estimación se realiza con una escala relativa: **1 punto** (esfuerzo muy pequeño), **2 puntos** (pequeño), **3 puntos** (moderado), **5 puntos** (considerable) y **8 puntos** (alto). Los puntos representan complejidad y esfuerzo relativo, incorporando incertidumbre y riesgo; **no equivalen a horas exactas de programación**. Al ser el primer Sprint, no se emplea una velocidad histórica del equipo.

| Código HU | Historia de Usuario | Complejidad | Puntos de historia | Justificación |
|---|---|---|---|---|
| HU-01 | Iniciar sesión en el sistema | Considerable | 5 | Incluye la estructura inicial del microservicio de identidad con Clean Architecture, hash de contraseñas, emisión de JWT y pantalla de login. |
| HU-02 | Restringir el acceso por rol y por curso | Moderada | 3 | Reutiliza la infraestructura de HU-01; requiere políticas de autorización, asignación docente–curso y rutas protegidas. |
| HU-03 | Configurar y verificar la conexión con Moodle | Considerable | 5 | Primera integración con servicios externos, manejo seguro de secretos y tratamiento de distintos errores de conexión. |
| HU-04 | Sincronizar cursos y matriculados | Considerable | 5 | Modelo de datos, consumo de servicios web, mapeo, idempotencia, paginación y bitácora. |
| HU-05 | Sincronizar calificaciones y estados de actividad | Alta | 8 | Mayor incertidumbre: depende de permisos y funciones habilitadas en Moodle; requiere manejo de datos ausentes, escalas y sincronización incompleta. |
| HU-06 | Seleccionar curso y listar estudiantes | Considerable | 5 | Endpoints con autorización por curso, paginación, búsqueda y filtros, y dos pantallas React. |
| HU-07 | Consultar el panel básico de indicadores | Considerable | 5 | Cálculo de indicadores con pruebas unitarias, endpoint y visualización con gráfico, advertencias, fuente y fecha. |
| | **Total** | | **36** | |

---

# 16. DESCOMPOSICIÓN DE HISTORIAS DE USUARIO EN TAREAS

Este apartado consolida en tablas el desglose de tareas de cada Historia de Usuario; el detalle individual de cada tarea (tipo, fechas y descripción) se encuentra en las fichas de la sección 13.2. Cada tarea tiene un responsable principal y, cuando corresponde, un colaborador. El Product Owner no tiene tareas de desarrollo asignadas. No se incluyen tareas de IA (Python/FastAPI, LangGraph, Ollama) por no formar parte del Sprint 1.

**Abreviaturas de integrantes:** Curillo = Curillo Pilamunga Diego Alexander · Manobanda = Manobanda Masabanda Luis Alfredo · Redroban = Redroban Chimbo Joselyn Amparo · Quispe = Quispe Nasimba Daniela Alejandra.

### HU-01 — Iniciar sesión en el sistema (12,5 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-01.1 | HU-01 | Crear el repositorio Git y la solución .NET 10 del Servicio de Identidad con Clean Architecture (Domain, Application, Infrastructure, API), convenciones de nombres y Swagger. | Curillo | Manobanda | 2,5 |
| TA-01.2 | HU-01 | Definir las entidades Usuario y Rol (Docente, Administrador técnico) con EF Core, migración en SQL Server y usuarios semilla de prueba anonimizados. | Manobanda | Curillo | 2,5 |
| TA-01.3 | HU-01 | Implementar el endpoint POST /auth/login: validación con hash de contraseña, emisión de JWT con claim de rol y mensaje de error genérico. | Curillo | Manobanda | 3,0 |
| TA-01.4 | HU-01 | Construir en React la pantalla de inicio de sesión (M-01) con validación de campos, llamada al endpoint, almacenamiento del token y cierre de sesión. | Redroban | Quispe | 3,0 |
| TA-01.5 | HU-01 | Escribir pruebas unitarias xUnit del login (válido, usuario inexistente, contraseña errónea) y la colección Postman del flujo de inicio y cierre de sesión. | Quispe | Curillo | 1,5 |

### HU-02 — Restringir el acceso por rol y por curso (6 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-02.1 | HU-02 | Crear la entidad de asignación docente–curso y las políticas de autorización por rol y por curso en los servicios (respuestas 401 y 403). | Manobanda | Curillo | 2,0 |
| TA-02.2 | HU-02 | Implementar en React el menú y las rutas protegidas según el rol (M-02) y la redirección ante 401. | Redroban | Quispe | 2,0 |
| TA-02.3 | HU-02 | Realizar pruebas de permisos: token vencido, rol sin permiso y docente consultando un curso no asignado (xUnit y Postman). | Quispe | Manobanda | 2,0 |

### HU-03 — Configurar y verificar la conexión con Moodle (11,5 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-03.1 | HU-03 | Crear el Servicio de Integración con Moodle (Clean Architecture) y el cliente HTTP para los servicios web REST de Moodle (consulta de información del sitio). | Curillo | Manobanda | 3,0 |
| TA-03.2 | HU-03 | Implementar la entidad de configuración (URL) y los endpoints de consulta y actualización restringidos al Administrador técnico; leer la credencial técnica desde variables de entorno. | Manobanda | Curillo | 2,5 |
| TA-03.3 | HU-03 | Implementar el endpoint de verificación de conexión con resultado (nombre del sitio, tiempo de respuesta) y mensajes diferenciados para URL inaccesible y credencial rechazada. | Curillo | Manobanda | 2,0 |
| TA-03.4 | HU-03 | Construir en React la pantalla de configuración de Moodle (M-03) con URL, indicador de credencial configurada y botón de verificación. | Quispe | Redroban | 2,5 |
| TA-03.5 | HU-03 | Realizar pruebas de conexión con respuestas simuladas de Moodle (válida, URL inválida y credencial inválida) y comprobar que no se exponen secretos. | Quispe | Manobanda | 1,5 |

### HU-04 — Sincronizar cursos y matriculados (11 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-04.1 | HU-04 | Crear la solución del Servicio Académico y el modelo de datos de Curso, Estudiante, Matrícula y Bitácora de sincronización, con migración en SQL Server. | Manobanda | Curillo | 2,5 |
| TA-04.2 | HU-04 | Consumir los servicios web de Moodle para cursos y usuarios matriculados, con paginación, y mapear las respuestas a objetos del dominio. | Curillo | Manobanda | 3,0 |
| TA-04.3 | HU-04 | Implementar el caso de uso de sincronización de cursos y matriculados de forma idempotente (sin duplicados). | Manobanda | Curillo | 2,5 |
| TA-04.4 | HU-04 | Registrar en la bitácora la fecha y hora, el resultado y los errores de cada sincronización, y exponer su consulta paginada. | Manobanda | Curillo | 1,5 |
| TA-04.5 | HU-04 | Escribir pruebas unitarias y de integración del mapeo y de la idempotencia de la sincronización. | Quispe | Curillo | 1,5 |

### HU-05 — Sincronizar calificaciones y estados de actividad (14,5 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-05.1 | HU-05 | Definir las entidades Ítem de calificación (con escala), Calificación y Estado de actividad, con migración en SQL Server y soporte para «sin dato». | Manobanda | Curillo | 2,0 |
| TA-05.2 | HU-05 | Consumir los servicios web de Moodle para ítems de calificación, notas y estados de actividad, manejando permisos insuficientes y funciones no habilitadas. | Curillo | Manobanda | 3,5 |
| TA-05.3 | HU-05 | Implementar el caso de uso de sincronización de calificaciones y estados con marca «Incompleta» ante errores y almacenamiento de notas ausentes como «sin dato». | Quispe | Curillo | 3,0 |
| TA-05.4 | HU-05 | Implementar los endpoints para ejecutar la sincronización y consultar su estado y errores. | Redroban | Curillo | 1,5 |
| TA-05.5 | HU-05 | Construir en React la pantalla de sincronización (M-04): botón de ejecución, estado, fecha de última sincronización y detalle de errores. | Redroban | Quispe | 2,5 |
| TA-05.6 | HU-05 | Realizar pruebas de manejo de errores de API, datos ausentes y sincronización incompleta. | Quispe | Manobanda | 2,0 |

### HU-06 — Seleccionar curso y listar estudiantes (9,5 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-06.1 | HU-06 | Implementar los endpoints de cursos autorizados y de estudiantes por curso, con paginación, búsqueda por nombre y filtro con/sin calificaciones. | Redroban | Curillo | 3,0 |
| TA-06.2 | HU-06 | Construir en React la pantalla de selección de curso y período (M-05). | Redroban | Quispe | 2,0 |
| TA-06.3 | HU-06 | Construir en React la tabla de estudiantes con búsqueda, filtros y paginación (M-05). | Quispe | Redroban | 3,0 |
| TA-06.4 | HU-06 | Realizar pruebas de autorización por curso (acceso 403) y de paginación y filtros. | Quispe | Manobanda | 1,5 |

### HU-07 — Consultar el panel básico de indicadores (9 h)

| Código tarea | Código HU relacionada | Descripción de la tarea | Responsable | Colaborador | Estimación (horas) |
|---|---|---|---|---|---|
| TA-07.1 | HU-07 | Implementar en la capa de aplicación el cálculo de indicadores: promedio con escala, distribución por rangos, actividades evaluadas y progreso disponible, con pruebas unitarias xUnit. | Redroban | Manobanda | 3,5 |
| TA-07.2 | HU-07 | Exponer el endpoint de indicadores por curso, incluyendo fuente, fecha de última sincronización y advertencia de datos incompletos. | Curillo | Redroban | 1,0 |
| TA-07.3 | HU-07 | Construir en React el panel de indicadores (M-06): tarjetas, gráfico de distribución, progreso, advertencias y pie de fuente y fecha. | Manobanda | Redroban | 3,0 |
| TA-07.4 | HU-07 | Realizar pruebas del panel con un curso completo y uno con datos incompletos, comparando con cálculos manuales. | Redroban | Quispe | 1,5 |

**Total: 32 tareas · 74 horas.**

---

# 17. SPRINT BACKLOG

## 17.1 Tablero Scrum (estado inicial)

El tablero representa el estado inicial del Sprint, al cierre de la planificación. Todas las tareas se encuentran en «Tareas No Iniciadas»; las columnas «Tareas En Progreso» y «Tareas Completadas» están vacías y se actualizarán durante la ejecución del Sprint.

| Ítems del Backlog comprometidos | Tareas No Iniciadas | Tareas En Progreso | Tareas Completadas |
|---|---|---|---|
| **HU-01** Iniciar sesión en el sistema (5 SP) | TA-01.1 (Curillo)<br>TA-01.2 (Manobanda)<br>TA-01.3 (Curillo)<br>TA-01.4 (Redroban)<br>TA-01.5 (Quispe) | | |
| **HU-02** Restringir el acceso por rol y por curso (3 SP) | TA-02.1 (Manobanda)<br>TA-02.2 (Redroban)<br>TA-02.3 (Quispe) | | |
| **HU-03** Configurar y verificar la conexión con Moodle (5 SP) | TA-03.1 (Curillo)<br>TA-03.2 (Manobanda)<br>TA-03.3 (Curillo)<br>TA-03.4 (Quispe)<br>TA-03.5 (Quispe) | | |
| **HU-04** Sincronizar cursos y matriculados (5 SP) | TA-04.1 (Manobanda)<br>TA-04.2 (Curillo)<br>TA-04.3 (Manobanda)<br>TA-04.4 (Manobanda)<br>TA-04.5 (Quispe) | | |
| **HU-05** Sincronizar calificaciones y estados de actividad (8 SP) | TA-05.1 (Manobanda)<br>TA-05.2 (Curillo)<br>TA-05.3 (Quispe)<br>TA-05.4 (Redroban)<br>TA-05.5 (Redroban)<br>TA-05.6 (Quispe) | | |
| **HU-06** Seleccionar curso y listar estudiantes (5 SP) | TA-06.1 (Redroban)<br>TA-06.2 (Redroban)<br>TA-06.3 (Quispe)<br>TA-06.4 (Quispe) | | |
| **HU-07** Consultar el panel básico de indicadores (5 SP) | TA-07.1 (Redroban)<br>TA-07.2 (Curillo)<br>TA-07.3 (Manobanda)<br>TA-07.4 (Redroban) | | |

## 17.2 Resumen del Sprint Backlog

| ID | Historia de Usuario | Responsable principal | Participan en tareas | SP | Horas | Prioridad |
|---|---|---|---|---|---|---|
| HU-01 | Iniciar sesión en el sistema | Curillo | Manobanda, Redroban, Quispe | 5 | 12,5 h | Crítica |
| HU-02 | Restringir el acceso por rol y por curso | Manobanda | Redroban, Quispe | 3 | 6 h | Crítica |
| HU-03 | Configurar y verificar la conexión con Moodle | Curillo | Manobanda, Quispe | 5 | 11,5 h | Crítica |
| HU-04 | Sincronizar cursos y matriculados | Manobanda | Curillo, Quispe | 5 | 11 h | Alta |
| HU-05 | Sincronizar calificaciones y estados de actividad | Quispe | Curillo, Manobanda, Redroban | 8 | 14,5 h | Alta |
| HU-06 | Seleccionar curso y listar estudiantes | Redroban | Quispe | 5 | 9,5 h | Alta |
| HU-07 | Consultar el panel básico de indicadores | Redroban | Curillo, Manobanda | 5 | 9 h | Alta |
| **Total** | **7 historias · 32 tareas** | **Los 4 integrantes** | | **36** | **74 h** | |

---

# 18. DISTRIBUCIÓN DE RESPONSABILIDADES

| Integrante | Responsabilidades principales | Historias de Usuario relacionadas | Tareas asignadas |
|---|---|---|---|
| Curillo Pilamunga Diego Alexander | Estructura del servicio de identidad, emisión de JWT, integración con los servicios web REST de Moodle, endpoint de indicadores. | HU-01, HU-03, HU-04, HU-05, HU-07 | TA-01.1, TA-01.3, TA-03.1, TA-03.3, TA-04.2, TA-05.2, TA-07.2 (7 tareas · 18 h) |
| Manobanda Masabanda Luis Alfredo | Modelo de datos en SQL Server, migraciones, autorización por rol y curso, configuración de Moodle, lógica de sincronización y panel de indicadores. | HU-01, HU-02, HU-03, HU-04, HU-05, HU-07 | TA-01.2, TA-02.1, TA-03.2, TA-04.1, TA-04.3, TA-04.4, TA-05.1, TA-07.3 (8 tareas · 18,5 h) |
| Redroban Chimbo Joselyn Amparo | Interfaces React.js (login, menú, sincronización, selección de curso), endpoints de consulta de cursos y estudiantes, cálculo de indicadores. | HU-01, HU-02, HU-05, HU-06, HU-07 | TA-01.4, TA-02.2, TA-05.4, TA-05.5, TA-06.1, TA-06.2, TA-07.1, TA-07.4 (8 tareas · 19 h) |
| Quispe Nasimba Daniela Alejandra | Interfaces React.js (configuración Moodle, tabla de estudiantes), sincronización de calificaciones, pruebas unitarias, de API y de permisos. | HU-01, HU-02, HU-03, HU-04, HU-05, HU-06 | TA-01.5, TA-02.3, TA-03.4, TA-03.5, TA-04.5, TA-05.3, TA-05.6, TA-06.3, TA-06.4 (9 tareas · 18,5 h) |
| **Total** | | | **32 tareas · 74 h** |

El Product Owner no figura como responsable de tareas de desarrollo.

---

# 19. PLAN DE TRABAJO DEL SPRINT 1

## 19.1 Orden de elaboración y dependencias

1. **HU-01 (inicio del Sprint)** es la base técnica: establece el repositorio, la estructura de Clean Architecture y la autenticación sobre las cuales se apoyan las demás historias.
2. **HU-02** depende de HU-01: añade las políticas de autorización por rol y por curso que utilizan las historias posteriores.
3. **HU-03** depende de HU-01 y HU-02 (solo el Administrador técnico configura Moodle). Establece la conexión que habilita toda sincronización.
4. **HU-04** depende de HU-03: proporciona la estructura académica (cursos, estudiantes, matrículas).
5. **HU-05** depende de HU-04: importa notas y estados asociados a los cursos ya sincronizados.
6. **HU-06** depende de HU-02 y HU-04: lista cursos y estudiantes autorizados.
7. **HU-07** depende de HU-05 y HU-06: calcula y muestra los indicadores sobre datos ya sincronizados.

Las interfaces React se desarrollan en paralelo con los endpoints a partir de un contrato de API acordado, utilizando datos simulados hasta su integración. Si fuera necesario simplificar alcance, el Product Owner y el equipo acuerdan reducir primero los filtros de HU-06 y la visualización gráfica de HU-07, sin alterar la lógica de datos.

## 19.2 Cronograma diario por integrante

El 19/10 y el 20/10 corresponden al arranque y a la planificación. Ningún integrante supera las 4 horas diarias de tareas; la reunión diaria de 15 minutos se cubre con la reserva.

| Día | Curillo | Manobanda | Redroban | Quispe |
|---|---|---|---|---|
| Mié 21/10 | TA-01.1 (2,5 h)<br>TA-03.1 (1,5 h) | TA-04.1 (2,5 h)<br>TA-01.2 (1,5 h) | TA-01.4 (3 h)<br>TA-06.2 (1 h) | TA-03.4 (2,5 h)<br>TA-06.3 (1,5 h) |
| Jue 22/10 | TA-01.3 (3 h)<br>TA-03.1 (1 h) | TA-01.2 (1 h)<br>TA-02.1 (2 h)<br>TA-03.2 (1 h) | TA-06.2 (1 h)<br>TA-02.2 (2 h)<br>TA-06.1 (1 h) | TA-06.3 (1,5 h)<br>TA-01.5 (1,5 h) |
| Vie 23/10 | TA-03.1 (0,5 h)<br>TA-03.3 (2 h)<br>TA-04.2 (1,5 h) | TA-03.2 (1,5 h)<br>TA-04.4 (1,5 h)<br>TA-05.1 (1 h) | TA-06.1 (2 h)<br>TA-07.1 (2 h) | TA-02.3 (2 h)<br>TA-03.5 (1,5 h) |
| Lun 26/10 | TA-04.2 (1,5 h)<br>TA-05.2 (2,5 h) | TA-04.3 (2,5 h)<br>TA-05.1 (1 h) | TA-07.1 (1,5 h)<br>TA-05.5 (1 h) | TA-04.5 (1,5 h)<br>TA-05.3 (2,5 h) |
| Mar 27/10 | TA-05.2 (1 h)<br>TA-07.2 (1 h) | TA-07.3 (1,5 h) | TA-05.4 (1,5 h)<br>TA-05.5 (1,5 h) | TA-05.3 (0,5 h)<br>TA-05.6 (2 h) |
| Mié 28/10 | Apoyo a la integración y reserva | TA-07.3 (1,5 h) | TA-07.4 (1,5 h) | TA-06.4 (1,5 h) |
| Jue 29/10 | Integración final, pruebas del flujo completo, Revisión del Sprint (demostración al Product Owner) y Retrospectiva. | | | |

| Verificación de horas | Curillo | Manobanda | Redroban | Quispe |
|---|---|---|---|---|
| Horas de tareas en el cronograma | 18 h | 18,5 h | 19 h | 18,5 h |
| Coincide con la tabla de capacidad | Sí | Sí | Sí | Sí |

## 19.3 Riesgos e impedimentos

Se trata de riesgos potenciales identificados durante la planificación; ninguno se presenta como ocurrido.

| N.º | Riesgo o impedimento | Posible impacto | Probabilidad | Medida de mitigación |
|---|---|---|---|---|
| 1 | No contar a tiempo con una instancia Moodle de pruebas. | No se podría validar HU-03 a HU-05 contra un entorno real. | Media | Solicitar la instancia al Product Owner el 21/10; mientras tanto, utilizar respuestas simuladas de los servicios web para las pruebas. |
| 2 | Permisos insuficientes en el token de los servicios web de Moodle (funciones no habilitadas). | Datos de calificaciones o estados incompletos. | Media | Solicitar la habilitación de las funciones necesarias desde el inicio; manejar el error y marcar la sincronización como «Incompleta» (RNF-03). |
| 3 | Configuración de la arquitectura de microservicios (puertos, configuraciones, comunicación). | Retrasos en la integración entre servicios. | Media | Usar una plantilla común de solución Clean Architecture y documentar la configuración en el repositorio (RNF-05). |
| 4 | Integración entre React y .NET (CORS, manejo del JWT). | Retrasos en las pantallas y en la autorización. | Media | Acordar el contrato de API el 21/10, probar con Postman y desarrollar interfaces con datos simulados en paralelo. |
| 5 | Configuración de SQL Server (conexión, migraciones) en los equipos de los integrantes. | Inconsistencias en el esquema entre integrantes. | Baja | Versionar migraciones EF Core en Git y documentar la configuración. |
| 6 | Coordinación del equipo (dependencias entre tareas de backend y frontend). | Bloqueos y trabajo duplicado. | Media | Reunión diaria de 15 minutos, tablero actualizado y revisión cruzada de cambios. |
| 7 | Disponibilidad de entornos de desarrollo (SDK de .NET 10, SQL Server, Node.js). | Integrantes sin poder ejecutar el proyecto. | Baja | Verificar los entornos de todos los integrantes el 21/10 antes de iniciar las tareas. |
| 8 | Alcance exigente para 7 días de ejecución. | No concluir todas las historias. | Media | Mantener la holgura de 26,8 h, priorizar HU-01 a HU-05 y simplificar primero HU-06 (filtros) y HU-07 (gráfico) si fuera necesario, acordándolo con el Product Owner. |

## 19.4 Criterios de finalización del Sprint 1

- Las siete Historias de Usuario cumplen sus criterios de aceptación y satisfacen la Definición de Terminado.
- Se demuestra al Product Owner el flujo completo: inicio de sesión, verificación de conexión con Moodle, sincronización, selección de curso, listado de estudiantes y panel de indicadores.
- Los secretos (credencial técnica de Moodle) no aparecen en el repositorio, en las respuestas de la API ni en los registros.
- Las pruebas unitarias de indicadores y las pruebas de permisos están definidas y se ejecutan de forma reproducible.
- El código está integrado en el repositorio Git y existe documentación mínima de instalación y configuración.
- Las tareas están cerradas en el tablero Scrum.

---

# 20. DEFINICIÓN DE TERMINADO (DEFINITION OF DONE)

**Una tarea se considera terminada cuando:**

1. Se implementó de acuerdo con su descripción y con el requerimiento relacionado.
2. El código está integrado en el repositorio Git mediante la rama de integración acordada y fue revisado por otro integrante.
3. Compila sin errores y se ejecuta en el entorno local de desarrollo.
4. Incluye las pruebas correspondientes (unitarias o de API) cuando la tarea involucra lógica.
5. Maneja los errores previstos con mensajes útiles y sin exponer información sensible.
6. No contiene secretos en el código ni en el repositorio (RN-04, RNF-01).
7. Está actualizada en el tablero Scrum.

**Una Historia de Usuario se considera completamente terminada cuando:**

1. Todas sus tareas cumplen la Definición de Terminado de tarea.
2. Cumple todos sus criterios de aceptación, verificados con los resultados indicados en la sección 14.
3. Respeta las reglas de negocio aplicables (sección 5.3) y los requerimientos no funcionales relacionados.
4. Las pruebas unitarias, de API y de permisos pertinentes se ejecutan de forma reproducible.
5. Cuenta con la documentación técnica necesaria (instalación, configuración y endpoints en Swagger).
6. Se puede demostrar funcionando de extremo a extremo al Product Owner, y este la valida.

---

# 21. COMPROMISO DEL EQUIPO

El equipo de desarrollo, conformado por Curillo Pilamunga Diego Alexander, Manobanda Masabanda Luis Alfredo, Redroban Chimbo Joselyn Amparo y Quispe Nasimba Daniela Alejandra, se compromete a completar las siete Historias de Usuario seleccionadas (36 puntos de historia y 74 horas de trabajo estimado) durante el Sprint 1, que se extiende del 19/10/2026 al 29/10/2026.

Este compromiso:

- Fue definido conjuntamente con el Product Owner, Ing. Hernán Naranjo, Mg., durante la reunión de planificación del 20/10/2026.
- Se basa en la selección de Historias de Usuario realizada por el equipo considerando su capacidad (100,8 horas netas disponibles frente a 74 horas planificadas) y el tiempo disponible.
- Implica que el equipo asume la responsabilidad de organizar y ejecutar las tareas del Sprint Backlog de manera autoorganizada.
- Establece que el alcance comprometido se mantiene durante la ejecución del Sprint. Cualquier necesidad de cambio se gestionará mediante la colaboración con el Product Owner.

| Integrante | Compromiso |
|---|---|
| Curillo Pilamunga Diego Alexander | ☐ Aceptado |
| Manobanda Masabanda Luis Alfredo | ☐ Aceptado |
| Redroban Chimbo Joselyn Amparo | ☐ Aceptado |
| Quispe Nasimba Daniela Alejandra | ☐ Aceptado |
| Ing. Hernán Naranjo, Mg. (Product Owner) | ☐ Conforme con el alcance |

---

# 22. CONCLUSIONES DE LA PLANIFICACIÓN

1. **Importancia de la planificación.** La reunión de planificación permitió transformar los requerimientos de AcademIA en un plan ejecutable con objetivo, alcance, estimaciones y responsables claros. Este plan reduce la incertidumbre del primer Sprint, donde además se establece la base técnica de la arquitectura de microservicios.

2. **Priorización de funcionalidades.** Se seleccionaron RF-01 a RF-04 y una versión básica de RF-05 porque forman una cadena de dependencias coherente (acceso seguro, conexión con Moodle, datos sincronizados, consulta e indicadores). Los requerimientos de seguimiento, alertas, análisis grupal, agente de IA, recomendaciones, informes y auditoría completa permanecen en el Product Backlog para Sprints posteriores, evitando comprometer un alcance inviable.

3. **Selección de Historias de Usuario.** Se definieron siete historias, con 36 puntos de historia, redactadas en el formato Como / Quiero / Para y asociadas a criterios de aceptación verificables, de acuerdo con las tres C y las características INVEST.

4. **Distribución de actividades.** Las 32 tareas se repartieron entre los cuatro integrantes de forma equilibrada (entre 18 y 19 horas cada uno), según sus responsabilidades técnicas y manteniendo colaboración en las tareas críticas, sin crear roles Scrum adicionales ni asignar tareas de programación al Product Owner.

5. **Organización del Sprint Backlog.** El tablero Scrum de cuatro columnas quedó en su estado inicial, con todas las tareas en «Tareas No Iniciadas», lo que otorga trazabilidad entre requerimientos, historias, tareas y responsables. Las reglas de negocio documentadas guían la implementación y la verificación de los criterios.

6. **Valor del incremento esperado.** Al finalizar el Sprint, el docente podrá consultar de forma consolidada y transparente el estado de sus cursos de prueba, y el proyecto contará con la infraestructura de acceso, integración y persistencia necesaria para incorporar alertas y servicios de IA.

7. **Preparación del equipo.** Con capacidad, dependencias, riesgos y Definición de Terminado establecidos, y con una holgura de 26,8 horas, el equipo queda en condiciones de iniciar la ejecución del Sprint 1 con un compromiso claro frente al Product Owner.

---

# 23. RESULTADOS DEL SPRINT 1

Las secciones 23 a 27 corresponden al cierre del Sprint 1 (19/10/2026 – 29/10/2026). La tabla de funcionalidades refleja una revisión del código disponible en la rama de trabajo y no sustituye la evidencia histórica del Sprint. Las casillas de pruebas, retrospectiva y métricas se completan únicamente con resultados ejecutados o datos confirmados por el equipo; esta revisión no declara como realizada ninguna funcionalidad ni prueba sin evidencia.

## 23.1 HU-01 – Iniciar sesión en el sistema

**Objetivo:** Permitir el inicio y cierre de sesión con credenciales y rol, emitiendo un JWT y manejando de forma segura las contraseñas.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Estructura de la solución y del repositorio | TA-01.1 | Parcial |
| Persistencia de usuarios, roles y asignaciones | TA-01.2 | Parcial |
| Endpoint POST /api/auth/login con emisión de JWT | TA-01.3 | Implementado; pendiente prueba funcional |
| Pantalla de inicio de sesión conectada a la API | TA-01.4 | Implementada; pendiente prueba funcional |
| Pruebas del inicio y cierre de sesión | TA-01.5 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 con Clean Architecture | Servicio de Identidad | Separa dominio, casos de uso e infraestructura y facilita las pruebas. |
| Entity Framework Core + SQL Server | Usuarios, roles y migraciones | Persistencia relacional con esquema versionado. |
| JWT | Identidad y rol dentro del token | Permite validar el acceso sin reenviar credenciales. |
| React.js | Pantalla de inicio de sesión | Componentes reutilizables para formularios. |
| xUnit y Postman | Pruebas unitarias y de API | Verifican el login y el cierre de sesión de forma reproducible. |

**Descripción de lo construido:** La API implementa login con JWT y hash PBKDF2-SHA256; el administrador inicial se aprovisiona una sola vez mediante variables de entorno y las cuentas posteriores se crean desde un endpoint administrativo. React envía las credenciales a la API y conserva la sesión en `sessionStorage`. La persistencia se define en el esquema SQL, pero no se verificó la inicialización de la base ni se ejecutaron pruebas funcionales; el cierre de sesión elimina el token del navegador.

## 23.2 HU-02 – Restringir el acceso por rol y por curso

**Objetivo:** Garantizar que cada usuario acceda solo a las opciones de su rol y que el docente vea únicamente sus cursos autorizados.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Políticas de autorización por rol y por curso | TA-02.1 | Implementadas; pendientes pruebas funcionales |
| Menú y rutas protegidas según el rol (M-02) | TA-02.2 | Parcial |
| Pruebas de permisos | TA-02.3 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 | Políticas de autorización | Aplica el mínimo privilegio (RNF-01) en cada servicio. |
| SQL Server | Asignación docente–curso | Almacena qué cursos puede ver cada docente. |
| React.js | Menú y rutas protegidas | Muestra solo las opciones del rol autenticado. |
| xUnit y Postman | Pruebas de permisos | Verifican respuestas 401 y 403. |

**Descripción de lo construido:** La API requiere JWT por defecto; la sincronización técnica está limitada al rol administrador y los docentes solo reciben cursos y datos de cursos asignados. Las rutas de React requieren una sesión, aunque no hay una administración visual de cuentas/asignaciones. Deben probarse respuestas 401/403 y aislamiento entre cursos antes de considerar aceptada la historia.

## 23.3 HU-03 – Configurar y verificar la conexión con Moodle

**Objetivo:** Registrar la URL de Moodle y comprobar la conectividad con sus servicios web sin exponer las credenciales técnicas.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Servicio de Integración con Moodle y cliente REST | TA-03.1 | Parcial |
| Configuración de Moodle (URL y secretos) | TA-03.2 | Parcial |
| Verificación de la conexión | TA-03.3 | Parcial |
| Pantalla de configuración de Moodle (M-03) | TA-03.4 | No realizado |
| Pruebas de la conexión | TA-03.5 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 con Clean Architecture | Servicio de Integración con Moodle | Aísla la dependencia externa en un microservicio. |
| Servicios Web REST de Moodle | Verificación de conectividad | Mecanismo oficial de integración con Moodle. |
| Variables de entorno | Credencial técnica | Evita secretos en el repositorio (RN-04, RNF-01). |
| React.js | Pantalla de configuración | Permite al Administrador técnico verificar la conexión. |
| xUnit y Postman | Pruebas de conexión | Cubren conexión válida, URL inválida y credencial inválida. |

**Descripción de lo construido:** El cliente REST lee URL y token desde configuración/variables de entorno, y las operaciones de sincronización consultan Moodle. No se implementó una pantalla administrativa para editar o verificar la conexión ni una respuesta específica con nombre del sitio y tiempo de respuesta. No se verificaron una instancia y credenciales reales; esta historia sigue parcial.

## 23.4 HU-04 – Sincronizar cursos y matriculados

**Objetivo:** Importar desde Moodle los cursos y estudiantes matriculados hacia SQL Server, registrando fecha, resultado y errores.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Servicio Académico y modelo de datos de cursos | TA-04.1 | Parcial |
| Consumo de cursos y matriculados de Moodle | TA-04.2 | Parcial |
| Caso de uso de sincronización idempotente | TA-04.3 | Parcial |
| Bitácora de sincronización | TA-04.4 | Parcial |
| Pruebas de la sincronización de cursos | TA-04.5 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 con Clean Architecture | Servicio Académico | Concentra la lógica de cursos y matrículas. |
| Servicios Web REST de Moodle | Cursos y matriculados | Fuente oficial de la información académica. |
| Entity Framework Core + SQL Server | Persistencia y bitácora | Transacciones confiables y migraciones. |
| xUnit | Pruebas de mapeo e idempotencia | Evitan duplicados al repetir la sincronización. |

**Descripción de lo construido:** El flujo de calificaciones consulta usuarios matriculados, guarda matrículas de quienes Moodle identifica como estudiantes y persiste cursos, actividades y calificaciones. El repositorio registra sincronizaciones completas/incompletas y las claves de Moodle ayudan a evitar duplicados. No se ha comprobado la idempotencia con ejecuciones reales contra SQL Server y Moodle; la bitácora cubre el flujo de calificaciones, no una sincronización completa de todos los cursos.

## 23.5 HU-05 – Sincronizar calificaciones y estados de actividad

**Objetivo:** Importar ítems de calificación, notas y estados de actividad, identificando los datos faltantes y las sincronizaciones incompletas.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Modelo de calificaciones y estados de actividad | TA-05.1 | Parcial |
| Consumo de notas y estados desde Moodle | TA-05.2 | Parcial |
| Sincronización de calificaciones y estados | TA-05.3 | Parcial |
| Endpoints de sincronización y estado | TA-05.4 | Parcial |
| Pantalla de sincronización (M-04) | TA-05.5 | No realizado |
| Pruebas de errores y datos ausentes | TA-05.6 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 con Clean Architecture | Servicio Académico | Mantiene la lógica de sincronización independiente de Moodle. |
| Servicios Web REST de Moodle | Calificaciones y estados de actividad | Fuente oficial; sujeta a los permisos habilitados. |
| Entity Framework Core + SQL Server | Ítems, notas y estados | Almacena la escala y el valor «sin dato». |
| React.js | Pantalla de sincronización | Muestra estado, fecha y errores. |
| xUnit y Postman | Pruebas de errores | Verifican el manejo de datos ausentes (RNF-03). |

**Descripción de lo construido:** El endpoint de calificaciones obtiene datos de Moodle, persiste escala máxima, notas y estado de sincronización. Los indicadores exponen la fecha y estado de la última sincronización; la interfaz de curso muestra una advertencia si quedó incompleta. No existe una pantalla dedicada para administrar sincronizaciones y faltan pruebas con Moodle para verificar todos los escenarios de error y estados ausentes.

## 23.6 HU-06 – Seleccionar curso y listar estudiantes

**Objetivo:** Permitir al docente elegir uno de sus cursos y consultar el listado paginado de estudiantes con búsqueda y filtros.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Endpoints de cursos y estudiantes | TA-06.1 | Parcial |
| Pantalla de selección de curso y período (M-05) | TA-06.2 | Parcial |
| Tabla de estudiantes con búsqueda, filtros y paginación (M-05) | TA-06.3 | Implementada; pendiente prueba funcional |
| Pruebas de autorización, paginación y filtros | TA-06.4 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 | Endpoints de cursos y estudiantes | Aplican autorización por curso y paginación (RN-02, RN-11). |
| SQL Server | Consultas de cursos y estudiantes | Fuente de los listados. |
| React.js | Selección de curso y tabla de estudiantes | Interfaz con búsqueda, filtros y paginación. |
| xUnit y Postman | Pruebas de autorización y filtros | Verifican accesos 403 y resultados filtrados. |

**Descripción de lo construido:** La pantalla lista cursos obtenidos de la API. El detalle consulta estudiantes matriculados con búsqueda, filtro de estado y paginación; la API verifica la asignación del docente antes de entregar información académica. La selección de período no está implementada y los controles de acceso, filtros y resultados requieren pruebas funcionales.

## 23.7 HU-07 – Consultar el panel básico de indicadores

**Objetivo:** Mostrar al docente el promedio con su escala, la distribución de calificaciones, las actividades evaluadas y el progreso disponible, con fuente y fecha.

**Funcionalidades implementadas:**

| Funcionalidad | Tarea | Estado |
|---|---|---|
| Cálculo de indicadores desde calificaciones persistidas | TA-07.1 | Implementado; pendiente prueba funcional |
| Endpoint de indicadores | TA-07.2 | Implementado; pendiente prueba funcional |
| Panel de indicadores conectado a la API (M-06) | TA-07.3 | Implementado; pendiente prueba funcional |
| Pruebas del panel de indicadores | TA-07.4 | No realizado |

**Tecnologías utilizadas:**

| Tecnología | Uso | ¿Por qué se eligió? |
|---|---|---|
| .NET 10 con Clean Architecture | Cálculo y endpoint de indicadores | Lógica de indicadores independiente de la interfaz. |
| React.js | Panel de indicadores | Tarjetas, gráfico y advertencias. |
| xUnit | Pruebas unitarias de indicadores | Comprueban los cálculos con un conjunto de datos conocido (RNF-06). |

**Descripción de lo construido:** El panel obtiene del API el promedio normalizado a 100, distribución, actividades evaluadas, matrículas, progreso y datos paginados de estudiantes. Las notas sin escala no se convierten en cero; se informa la fuente Moodle, última sincronización y advertencia de sincronización incompleta. La validación con datos reales de SQL Server y Moodle está pendiente, por lo que la historia aún no se declara aceptada.

## 23.8 Validación técnica del repositorio

Esta validación se realizó durante la revisión posterior del código y no representa horas, pruebas ni resultados históricos del Sprint 1.

| Comprobación | Resultado | Evidencia |
|---|---|---|
| Compilación backend | Aprobada, 0 errores y 0 advertencias | `dotnet build backend\Uta.AcademicPerformance.Middleware.slnx` |
| Compilación frontend | Aprobada | `npm run build` desde `frontend` |
| Análisis estático frontend | Aprobado, sin advertencias | `npm run lint` desde `frontend` |
| Smoke test HTTP | `/health`, `/health/database` y OpenAPI de desarrollo respondieron 200; `GET /api/cursos` sin JWT respondió 401 | API local con claves temporales; no se mutó la base |
| Pruebas automatizadas | No ejecutadas; no se encontró proyecto de pruebas .NET ni script `test` en `frontend/package.json` | Revisión de la solución y manifiesto |
| Esquema y flujo académico SQL/Moodle | Pendiente; la base local está accesible, pero conserva el esquema anterior y no se ejecutaron scripts ni sincronización para no modificar datos locales | Requiere aprobación para actualizar la base y credenciales/servicios Moodle |
| Dependencias NuGet | Corregido: `System.Security.Cryptography.Pkcs` transitivo 9.0.11 tenía un hallazgo alto y uno medio; se fijó 9.0.21. `dotnet list package --vulnerable --include-transitive` y la evaluación CVE de dependencias directas no reportaron vulnerabilidades conocidas | Auditoría NuGet y evaluación CVE |

---

# 24. TESTING DE LAS HISTORIAS DE USUARIO

Para cada Historia de Usuario se definen dos casos de prueba (14 en total), tomados de sus criterios de aceptación (sección 14). En esta revisión no se ejecutaron esos 14 casos y no se encontró evidencia previa de su ejecución; por ello se marcan como pendientes de evidencia del equipo, no como fallidos. La compilación y el lint de la sección 23.8 no sustituyen estos casos de aceptación.

## 24.1 HU-01 – Iniciar sesión en el sistema

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Inicio de sesión válido | Usuario y contraseña válidos y botón Ingresar | Recibe un JWT con su rol y es redirigido a la pantalla inicial | Pendiente de evidencia del equipo |
| Credenciales incorrectas | Contraseña errónea y botón Ingresar | Mensaje «Credenciales incorrectas», sin token (401) | Pendiente de evidencia del equipo |

## 24.2 HU-02 – Restringir el acceso por rol y por curso

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Rol sin permiso | Token de Docente que intenta abrir la configuración de Moodle | El servicio responde 403 | Pendiente de evidencia del equipo |
| Curso no asignado | Docente que consulta un curso que no tiene asignado | Solo recibe sus cursos; el acceso directo al otro curso responde 403 | Pendiente de evidencia del equipo |

## 24.3 HU-03 – Configurar y verificar la conexión con Moodle

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Conexión válida | Verificar conexión con URL y credencial correctas | «Conexión exitosa» con nombre del sitio y tiempo de respuesta | Pendiente de evidencia del equipo |
| Credencial rechazada | Verificar conexión con una credencial inválida | Mensaje diferenciado y útil, sin datos sensibles | Pendiente de evidencia del equipo |

## 24.4 HU-04 – Sincronizar cursos y matriculados

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Sincronización de cursos | Ejecutar la sincronización con la conexión verificada | Cursos y matriculados almacenados en SQL Server | Pendiente de evidencia del equipo |
| Idempotencia | Ejecutar la sincronización dos veces seguidas | Sin duplicados y con registro en la bitácora (fecha, resultado y errores) | Pendiente de evidencia del equipo |

## 24.5 HU-05 – Sincronizar calificaciones y estados de actividad

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Sincronización de calificaciones | Sincronizar calificaciones de cursos ya importados | Ítems, notas, estados y escala almacenados | Pendiente de evidencia del equipo |
| Datos no disponibles | Moodle sin permiso en una de las funciones consultadas | Error registrado, estado «Incompleta» y notas ausentes como «sin dato» | Pendiente de evidencia del equipo |

## 24.6 HU-06 – Seleccionar curso y listar estudiantes

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Listado, búsqueda y filtro | Seleccionar un curso, buscar por nombre y aplicar el filtro | Resultados paginados que coinciden con la búsqueda y el filtro | Pendiente de evidencia del equipo |
| Curso no autorizado | Consultar un curso ajeno por identificador directo | El sistema responde 403 | Pendiente de evidencia del equipo |

## 24.7 HU-07 – Consultar el panel básico de indicadores

| Caso | Entrada | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| Panel con datos completos | Abrir el panel de un curso sincronizado | Promedio con escala, distribución, actividades evaluadas, progreso, fuente y fecha | Pendiente de evidencia del equipo |
| Datos incompletos | Abrir el panel de un curso con sincronización incompleta | Advertencia visible e indicadores no disponibles como «sin datos» | Pendiente de evidencia del equipo |

---

# 25. DIAGRAMAS

Se presentan los diagramas del diseño previsto para el Sprint 1 en formato PlantUML, que puede renderizarse en cualquier editor compatible y pegarse como imagen en este informe. Al cierre del Sprint deben ajustarse a lo realmente implementado.

## 25.1 Diagrama de casos de uso generales del sistema

| Actor | Casos de uso del Sprint 1 |
|---|---|
| Docente | Iniciar y cerrar sesión; seleccionar curso y período; listar estudiantes con búsqueda y filtros; consultar el panel de indicadores |
| Administrador técnico | Iniciar y cerrar sesión; configurar la conexión con Moodle; verificar la conexión; ejecutar la sincronización; consultar el estado y los errores de sincronización |
| Moodle (sistema externo) | Proveer cursos, matriculados, calificaciones y estados de actividad mediante sus servicios web REST |

```
@startuml
left to right direction
actor Docente
actor "Administrador técnico" as Admin
actor Moodle <<sistema>>
rectangle AcademIA {
  usecase "Iniciar y cerrar sesión" as UC1
  usecase "Configurar conexión con Moodle" as UC2
  usecase "Verificar conexión" as UC3
  usecase "Sincronizar datos" as UC4
  usecase "Consultar estado de sincronización" as UC5
  usecase "Seleccionar curso y listar estudiantes" as UC6
  usecase "Consultar panel de indicadores" as UC7
}
Docente --> UC1
Docente --> UC6
Docente --> UC7
Admin --> UC1
Admin --> UC2
Admin --> UC4
Admin --> UC5
UC2 .> UC3 : <<include>>
UC4 --> Moodle
UC3 --> Moodle
@enduml
```

*Figura 7. Casos de uso generales de AcademIA (Sprint 1)*

## 25.2 Diagrama de secuencia: inicio de sesión

```
@startuml
actor Usuario
participant "React.js" as FE
participant "Servicio de Identidad" as ID
database "SQL Server" as DB
Usuario -> FE : ingresa usuario y contraseña
FE -> ID : POST /auth/login
ID -> DB : busca usuario y verifica hash
DB --> ID : usuario y rol
ID --> FE : 200 con JWT (o 401 si falla)
FE --> Usuario : redirige a la pantalla de su rol
@enduml
```

*Figura 8. Secuencia del inicio de sesión*

## 25.3 Diagrama de secuencia: sincronización con Moodle

```
@startuml
actor "Administrador técnico" as Admin
participant "React.js" as FE
participant "Servicio de Integración" as INT
participant "Moodle (REST)" as MD
participant "Servicio Académico" as ACA
database "SQL Server" as DB
Admin -> FE : pulsa Sincronizar
FE -> ACA : POST /sincronizacion/ejecutar (JWT)
ACA -> INT : solicita datos de Moodle
INT -> MD : cursos, matriculados, notas y estados
MD --> INT : datos (o error por permisos)
INT --> ACA : datos mapeados y errores
ACA -> DB : guarda sin duplicar y registra bitácora
ACA --> FE : estado Completa / Incompleta
FE --> Admin : muestra fecha, estado y errores
@enduml
```

*Figura 9. Secuencia de la sincronización*

## 25.4 Diagrama de clases

```
@startuml
class Rol {
  +Id : int
  +Nombre : string
}
class Usuario {
  +Id : int
  +NombreUsuario : string
  +ClaveHash : string
  +RolId : int
}
class ConfiguracionMoodle {
  +Id : int
  +Url : string
}
class Curso {
  +Id : int
  +IdMoodle : int
  +Nombre : string
  +Periodo : string
}
class Estudiante {
  +Id : int
  +IdMoodle : int
  +Nombre : string
}
class Matricula {
  +CursoId : int
  +EstudianteId : int
}
class AsignacionDocenteCurso {
  +UsuarioId : int
  +CursoId : int
}
class ItemCalificacion {
  +Id : int
  +CursoId : int
  +Nombre : string
  +Escala : string
}
class Calificacion {
  +ItemId : int
  +EstudianteId : int
  +Valor : decimal?
}
class EstadoActividad {
  +ItemId : int
  +EstudianteId : int
  +Estado : string
}
class SincronizacionLog {
  +Id : int
  +Fecha : DateTime
  +Resultado : string
  +Errores : string
}
Rol "1" -- "*" Usuario
Usuario "1" -- "*" AsignacionDocenteCurso
Curso "1" -- "*" AsignacionDocenteCurso
Curso "1" -- "*" Matricula
Estudiante "1" -- "*" Matricula
Curso "1" -- "*" ItemCalificacion
ItemCalificacion "1" -- "*" Calificacion
ItemCalificacion "1" -- "*" EstadoActividad
@enduml
```

*Figura 10. Modelo de clases previsto para el Sprint 1*

---

# 26. RETROSPECTIVA DEL SPRINT 1

La retrospectiva recoge los riesgos considerados en la planificación, los aspectos positivos del trabajo, una reflexión general y las mejoras que el equipo propone. Se realiza en la reunión de retrospectiva del cierre del Sprint y debe completarse con la experiencia real del equipo.

## 26.1 Problemas encontrados

La existencia de los riesgos y la forma en que el equipo los gestionó no se pueden reconstruir desde el código. Completar esta tabla con la retrospectiva real del equipo; no inferir respuestas de esta revisión técnica.

Riesgos considerados en la planificación (sección 19.3) y su gestión real:

| N.º | Riesgo considerado | ¿Se presentó? | Cómo se gestionó |
|---|---|---|---|
| 1 | Instancia Moodle de pruebas no disponible a tiempo | ☐ Sí ☐ No | **[Completar]** |
| 2 | Permisos insuficientes del token de los servicios web de Moodle | ☐ Sí ☐ No | **[Completar]** |
| 3 | Configuración de la arquitectura de microservicios | ☐ Sí ☐ No | **[Completar]** |
| 4 | Integración entre React y .NET (CORS, JWT) | ☐ Sí ☐ No | **[Completar]** |
| 5 | Configuración de SQL Server | ☐ Sí ☐ No | **[Completar]** |
| 6 | Coordinación del equipo | ☐ Sí ☐ No | **[Completar]** |
| 7 | Disponibilidad de entornos de desarrollo | ☐ Sí ☐ No | **[Completar]** |
| 8 | Alcance exigente para 7 días de ejecución | ☐ Sí ☐ No | **[Completar]** |

Otros problemas no previstos: **[Completar]**

## 26.2 Aspectos positivos del Sprint

- **[Completar]** ¿Qué funcionó bien en la organización del equipo y en el uso del tablero Scrum?
- **[Completar]** ¿Qué decisiones técnicas resultaron acertadas (arquitectura, Clean Architecture, pruebas)?
- **[Completar]** ¿Cómo fue la colaboración con el Product Owner?

## 26.3 Reflexiones del equipo

**[Completar]** Redactar una reflexión general sobre el Sprint 1: qué base técnica quedó lista, qué se aprendió de la integración con Moodle y cómo influyó la planificación en el avance.

## 26.4 Mejoras propuestas

| Propuesta | Objetivo |
|---|---|
| **[Completar]** | **[Completar]** |
| **[Completar]** | **[Completar]** |
| **[Completar]** | **[Completar]** |

---

# 27. MÉTRICAS Y CONCLUSIÓN

## 27.1 Métricas del Sprint 1

Se comparan los valores planificados con los resultados reales. Solo la columna «Planificado» está definida. Las métricas reales y porcentajes no se pueden deducir del repositorio ni de las validaciones técnicas hechas después del Sprint; completarlos con el tablero y los registros de trabajo de las fechas del Sprint.

| Métrica | Planificado | Resultado real | % Cumplimiento |
|---|---|---|---|
| Historias de Usuario completadas | 7 | **[Completar]** | **[Completar]** |
| Puntos de historia completados | 36 SP | **[Completar]** | **[Completar]** |
| Tareas completadas | 32 | **[Completar]** | **[Completar]** |
| Horas de trabajo | 74 h (capacidad neta 100,8 h) | **[Completar]** | **[Completar]** |
| Criterios de aceptación cumplidos | 29 | **[Completar]** | **[Completar]** |
| Casos de prueba exitosos | 14 | **[Completar]** | **[Completar]** |
| Conexión con Moodle verificada | Sí | **[Completar]** | **[Completar]** |
| Sincronización de cursos y calificaciones | Sí | **[Completar]** | **[Completar]** |
| Panel básico de indicadores demostrado | Sí | **[Completar]** | **[Completar]** |

*Tabla 2. Métricas del Sprint 1*

## 27.2 Análisis de cumplimiento

**[Completar]** Explicar qué porcentaje del compromiso se cumplió y qué factores contribuyeron (reparto de tareas, cronograma, revisión cruzada, holgura disponible) o dificultaron el cumplimiento. Si hubo historias parciales o trasladadas, indicar cuáles y por qué.

## 27.3 Conclusión del Sprint 1

**[Completar]** Redactar la conclusión comparando el resultado con el objetivo del Sprint 1 (sección 10): si un docente autenticado pudo consultar, para un curso autorizado y sincronizado de la instancia Moodle de pruebas, el listado de estudiantes y el panel básico de indicadores con fuente y fecha de sincronización, y qué base técnica queda para los siguientes incrementos.
