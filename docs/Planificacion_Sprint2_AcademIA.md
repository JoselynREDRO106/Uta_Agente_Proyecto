![](imagen)![](imagen)__UNIVERSIDAD __

__TÉCNICA DE AMBATO__

__FACULTAD DE INGENIERÍA EN SISTEMAS, ELECTRÓNICA E INDUSTRIAL__

__CARRERA DE INGENIERÍA EN SOFTWARE__

__CICLO ACADÉMICO: JULIO – DICIEMBRE 2026__

__EduMonitor__

__PLANIFICACIÓN DEL SPRINT 2__

__Especificación y diseño__

__Sistema inteligente de seguimiento del rendimiento académico integrado con Moodle__

__Docente:__  
Ing\. Hernán Naranjo, Mg\.

__Integrantes:__

Curillo Pilamunga Diego Alexander

Manobanda Masabanda Luis Alfredo

Redroban Chimbo Joselyn Amparo

Quispe Nasimba Daniela Alejandra

__JULIO – DICIEMBRE 2026__

__AMBATO__

__Índice __

[1\. INTRODUCCIÓN	1](\#\_Toc245130107)

[1\.1 Contexto	1](\#\_Toc245130108)

[1\.2 Naturaleza del Sprint 2	1](\#\_Toc245130109)

[1\.3 Alcance del Sprint 2	1](\#\_Toc245130110)

[1\.4 Objetivos	1](\#\_Toc245130111)

[1\.5 Marco de referencia	2](\#\_Toc245130112)

[2\. DATOS GENERALES DE LA PLANIFICACIÓN	2](\#\_Toc245130113)

[3\. PARTICIPANTES DE LA REUNIÓN	2](\#\_Toc245130114)

[4\. AGENDA DE LA REUNIÓN DE PLANIFICACIÓN	3](\#\_Toc245130115)

[5\. OBJETIVO DEL SPRINT 2	3](\#\_Toc245130116)

[6\. REVISIÓN Y PRIORIZACIÓN DEL PRODUCT BACKLOG	3](\#\_Toc245130117)

[7\. REGLAS DE NEGOCIO APLICABLES	4](\#\_Toc245130118)

[8\. ANÁLISIS DE CAPACIDAD DEL SPRINT 2	4](\#\_Toc245130119)

[9\. DISEÑOS Y MODELOS TRANSVERSALES	5](\#\_Toc245130120)

[9\.1 Arquitectura ampliada	5](\#\_Toc245130121)

[9\.2 Modelo de datos ampliado	6](\#\_Toc245130122)

[9\.3 Flujo de una consulta al agente de IA	7](\#\_Toc245130123)

[10\. HISTORIAS DE USUARIO DEL SPRINT 2	7](\#\_Toc245130124)

[10\.1 Tabla resumen	7](\#\_Toc245130125)

[10\.2 Fichas, mockups, modelos, diseños y tareas por historia	8](\#\_Toc245130126)

[HU\-08 — Consultar el seguimiento individual de un estudiante	8](\#\_Toc245130127)

[HU\-09 — Configurar reglas y consultar alertas académicas	11](\#\_Toc245130128)

[HU\-10 — Consultar el análisis grupal del curso	14](\#\_Toc245130129)

[HU\-11 — Consultar al agente de IA	16](\#\_Toc245130130)

[HU\-12 — Recibir recomendaciones pedagógicas justificadas	19](\#\_Toc245130131)

[HU\-13 — Generar y descargar el informe del curso	22](\#\_Toc245130132)

[HU\-14 — Consultar el registro de auditoría	24](\#\_Toc245130133)

[Tareas habilitadoras para el Sprint 2	27](\#\_Toc245130134)

[11\. PLAN DE PRUEBAS FUNCIONALES	28](\#\_Toc245130135)

[11\.1 Estrategia	28](\#\_Toc245130136)

[11\.2 Casos de prueba por criterio de aceptación	29](\#\_Toc245130137)

[HU\-08 — Consultar el seguimiento individual de un estudiante	29](\#\_Toc245130138)

[HU\-09 — Configurar reglas y consultar alertas académicas	29](\#\_Toc245130139)

[HU\-10 — Consultar el análisis grupal del curso	29](\#\_Toc245130140)

[HU\-11 — Consultar al agente de IA	30](\#\_Toc245130141)

[HU\-12 — Recibir recomendaciones pedagógicas justificadas	30](\#\_Toc245130142)

[HU\-13 — Generar y descargar el informe del curso	30](\#\_Toc245130143)

[HU\-14 — Consultar el registro de auditoría	30](\#\_Toc245130144)

[11\.3 Matriz de trazabilidad	31](\#\_Toc245130145)

[12\. ESTIMACIÓN DE LAS HISTORIAS DE USUARIO	31](\#\_Toc245130146)

[13\. DESCOMPOSICIÓN EN TAREAS \(CONSOLIDADO\)	32](\#\_Toc245130147)

[14\. SPRINT BACKLOG	33](\#\_Toc245130148)

[14\.1 Tablero Scrum \(estado inicial\)	33](\#\_Toc245130149)

[14\.2 Resumen del Sprint Backlog	34](\#\_Toc245130150)

[15\. DISTRIBUCIÓN DE RESPONSABILIDADES	34](\#\_Toc245130151)

[16\. PLAN DE TRABAJO DEL SPRINT 2	35](\#\_Toc245130152)

[16\.1 Orden de elaboración y dependencias	35](\#\_Toc245130153)

[16\.2 Cronograma diario por integrante	35](\#\_Toc245130154)

[16\.3 Riesgos e impedimentos	36](\#\_Toc245130155)

[16\.4 Criterios de finalización del Sprint 2	36](\#\_Toc245130156)

[17\. DEFINICIÓN DE TERMINADO	37](\#\_Toc245130157)

[18\. COMPROMISO DEL EQUIPO	37](\#\_Toc245130158)

[19\. CONCLUSIONES	38](\#\_Toc245130159)

#  <a id="_Toc245118792"></a><a id="_Toc245119139"></a><a id="_Toc245130107"></a>1\. INTRODUCCIÓN

## <a id="_Toc245118793"></a><a id="_Toc245119140"></a><a id="_Toc245130108"></a>1\.1 Contexto

EduMonitor es un sistema de seguimiento y análisis del rendimiento académico integrado con Moodle\. En el Sprint 1 se planificó el incremento base: acceso por roles, conexión con Moodle, sincronización de cursos, matriculados, calificaciones y estados de actividad, listado de estudiantes y un panel básico de indicadores\. Los requerimientos RF\-06 a RF\-12 \(seguimiento individual, alertas, análisis grupal, agente de IA, recomendaciones, informes y auditoría\) permanecen en el Product Backlog y aún no cuentan con historias, mockups, modelos ni pruebas definidos\.

## <a id="_Toc245118794"></a><a id="_Toc245119141"></a><a id="_Toc245130109"></a>1\.2 Naturaleza del Sprint 2

El Sprint 2 es un Sprint de planificación, especificación y diseño\. No se escribe código de producción__\.__ Su incremento es un paquete de documentación validado por el Product Owner que deja listos para su construcción los requerimientos RF\-06 a RF\-12\. El paquete comprende:

- __Historias de Usuario__ en formato Como / Quiero / Para, con criterios de aceptación verificables\.
- __Mockups__ de cada pantalla nueva\.
- __Modelos__: modelo de datos y diagramas de clases y de secuencia\.
- __Diseños__: arquitectura ampliada y diseño técnico de cada servicio, incluidos los servicios de IA \(Python / FastAPI, LangGraph y Ollama\)\.
- __Pruebas funcionales__ planteadas sobre lo que evaluará cada criterio de aceptación\.

## <a id="_Toc245118795"></a><a id="_Toc245119142"></a><a id="_Toc245130110"></a>1\.3 Alcance del Sprint 2

__Incluye:__

- Especificación y diseño de RF\-06, RF\-07, RF\-08, RF\-09, RF\-10, RF\-11 y RF\-12\.
- Reglas de negocio nuevas derivadas de esos requerimientos \(sección 7\)\.
- Diseño de la arquitectura ampliada, del modelo de datos ampliado y del flujo del agente de IA\.
- Plan de pruebas funcionales con su matriz de trazabilidad\.
- Entrega de la planificación el 10/11/2026 y presentación del Sprint 2 el 12/11/2026\.

__No incluye:__

- Implementación de código de producción, ni de los servicios de IA, ni de las nuevas pantallas\.
- Ejecución de las pruebas funcionales \(solo se diseñan los casos\)\.
- Planificación de otros Sprints\.

## <a id="_Toc245118796"></a><a id="_Toc245119143"></a><a id="_Toc245130111"></a>1\.4 Objetivos

__Objetivo general del proyecto:__ desarrollar un sistema basado en microservicios, integrado con Moodle, que permita al docente consultar indicadores del rendimiento académico, detectar tempranamente dificultades y recibir recomendaciones pedagógicas generadas por un agente de inteligencia artificial\.

__Objetivos específicos del Sprint 2:__

1. Redactar las Historias de Usuario de RF\-06 a RF\-12 con criterios de aceptación verificables\.
2. Diseñar los mockups de las pantallas nuevas\.
3. Elaborar el modelo de datos ampliado y los diseños técnicos de los servicios nuevos, incluido el servicio de IA\.
4. Plantear los casos de prueba funcional asociados a cada criterio de aceptación y su trazabilidad\.
5. Validar el paquete con el Product Owner y presentarlo el 12/11/2026\.

## <a id="_Toc245118797"></a><a id="_Toc245119144"></a><a id="_Toc245130112"></a>1\.5 Marco de referencia

No se identifica normativa externa específica aplicable a EduMonitor\. Se aplican los requerimientos no funcionales de seguridad \(RNF\-01\) y privacidad \(RNF\-02\), que condicionan especialmente el diseño del agente de IA: control de acceso por curso, mínimo privilegio, datos mínimos y prohibición de enviar información identificable a servicios externos sin autorización\.

# <a id="_Toc245118798"></a><a id="_Toc245119145"></a><a id="_Toc245130113"></a>2\. DATOS GENERALES DE LA PLANIFICACIÓN

__Elemento__

__Información__

Proyecto

EduMonitor

Sprint

Sprint 2 \(especificación y diseño, sin código\)

Fecha de inicio

04/11/2026

Entrega de la planificación

10/11/2026

Presentación del Sprint 2

12/11/2026

Días hábiles considerados

04, 05, 06, 09, 10 y 11 de noviembre de 2026 \(6 días\)

Reunión de planificación

04/11/2026 \(duración supuesta: 3 horas, 180 minutos\)

Product Owner

Ing\. Hernán Naranjo, Mg\.

Equipo de desarrollo

Cuatro integrantes

Metodología

Scrum

Arquitectura

Microservicios

*Tabla 1\. *Datos generales de la planificación del Sprint 2

# <a id="_Toc245118799"></a><a id="_Toc245119146"></a><a id="_Toc245130114"></a>3\. PARTICIPANTES DE LA REUNIÓN

__N\.º__

__Nombre completo__

__Participación__

__Responsabilidad__

1

Ing\. Hernán Naranjo, Mg\.

Product Owner

Priorizar el Product Backlog, aclarar requerimientos y validar el alcance, los criterios y los entregables del Sprint\.

2

Curillo Pilamunga Diego Alexander

Equipo de desarrollo

Estimar, definir tareas de diseño técnico y comprometerse con el alcance\.

3

Manobanda Masabanda Luis Alfredo

Equipo de desarrollo

Estimar, definir tareas de modelado de datos y comprometerse con el alcance\.

4

Redroban Chimbo Joselyn Amparo

Equipo de desarrollo

Estimar, definir tareas de mockups y comprometerse con el alcance\.

5

Quispe Nasimba Daniela Alejandra

Equipo de desarrollo

Estimar, definir tareas de especificación y pruebas y comprometerse con el alcance\.

*Tabla 2\. *Participantes de la reunión de planificación

# <a id="_Toc245118800"></a><a id="_Toc245119147"></a><a id="_Toc245130115"></a>4\. AGENDA DE LA REUNIÓN DE PLANIFICACIÓN

__N\.º__

__Actividad__

__Descripción__

__Duración \(minutos\)__

__Responsable__

1

Revisión del Product Backlog

Revisión del avance del Sprint 1 y de RF\-06 a RF\-12\.

20

Product Owner

2

Priorización y selección

Selección de los requerimientos a especificar y diseñar\.

25

Product Owner y equipo

3

Definición del objetivo del Sprint

Acuerdo del objetivo verificable del Sprint 2\.

15

Product Owner y equipo

4

Definición de entregables e Historias de Usuario

Historias, mockups, modelos y diseños que se entregarán\.

30

Equipo de desarrollo

5

Criterios de aceptación y pruebas funcionales

Qué evaluará cada criterio y cómo se probará\.

30

Equipo y Product Owner

6

Estimación del esfuerzo

Estimación relativa en puntos de historia\.

20

Equipo de desarrollo

7

Descomposición en tareas y distribución

Tareas de diseño, responsables y colaboradores\.

25

Equipo de desarrollo

8

Consolidación del Sprint Backlog y compromiso

Tablero Scrum y compromiso del equipo\.

15

Equipo y Product Owner

__Total__

__180__

*Tabla 3\. *Agenda de la reunión de planificación

# <a id="_Toc245118801"></a><a id="_Toc245119148"></a><a id="_Toc245130116"></a>5\. OBJETIVO DEL SPRINT 2

<a id="_Toc245118802"></a>__Objetivo del Sprint 2:__ Dejar listo para construir el diseño de los requerimientos RF\-06 a RF\-12 de EduMonitor\. Para el 10/11/2026, el equipo entregará al Product Owner, para su validación, un paquete de especificación y diseño que incluye siete Historias de Usuario con criterios de aceptación verificables, mockups de cada pantalla nueva \(M\-07 a M\-14\), el modelo de datos y los diseños técnicos ampliados, con el servicio de IA incluido, y un plan de pruebas funcionales trazable\. Así, los siguientes incrementos podrán construirse sin ambigüedades\. En este Sprint no se escribe código de producción\.

__Verificación:__ el objetivo se cumple cuando cada historia cuenta con su ficha, mockups, modelo, diseño técnico y casos de prueba, la matriz de trazabilidad no deja ningún criterio sin caso de prueba y el Product Owner valida el paquete\.

# <a id="_Toc245119155"></a><a id="_Toc245130117"></a>6\. REVISIÓN Y PRIORIZACIÓN DEL PRODUCT BACKLOG

__Prioridad__

__Código RF__

__Requerimiento__

__Justificación de prioridad__

__Seleccionado__

1

RF\-06

Seguimiento individual

Extiende los datos del Sprint 1 con la vista por estudiante; base de alertas y recomendaciones\.

Sí

2

RF\-07

Alertas académicas

Depende de RF\-06; define reglas, evidencia y manejo de datos incompletos\.

Sí

3

RF\-08

Análisis grupal

Complementa los indicadores del Sprint 1 con resultados por actividad y tendencias\.

Sí

4

RF\-09

Agente de IA

Es el componente de mayor riesgo; su diseño temprano reduce incertidumbre\. Depende de las herramientas que exponen RF\-06 a RF\-08\.

Sí

5

RF\-10

Recomendaciones

Depende de RF\-07 y RF\-09\.

Sí

6

RF\-11

Informe

Consolida métricas, hallazgos y recomendaciones previos\.

Sí

7

RF\-12

Auditoría

Registra sincronizaciones, consultas y herramientas; se diseña al conocer todos los eventos\.

Sí

–

RF\-01 a RF\-05

Requerimientos del Sprint 1

Fueron planificados en el Sprint 1\.

No \(otro Sprint\)

*Tabla 4\. *Revisión y priorización del Product Backlog

__Por qué estos requerimientos:__ abarcan todo el Product Backlog pendiente\. Al ser un Sprint sin código, la complejidad radica en el diseño y no en la implementación, por lo que es viable especificarlos y diseñarlos en conjunto y reducir el riesgo antes de construirlos\.

# <a id="_Toc245118803"></a><a id="_Toc245119156"></a><a id="_Toc245130118"></a>7\. REGLAS DE NEGOCIO APLICABLES

Las reglas RN\-01 a RN\-11 se definieron en la planificación del Sprint 1\. Para los requerimientos de este Sprint se detallan las reglas RN\-12 a RN\-15 y se incorporan las reglas RN\-16 a RN\-19, derivadas de los requerimientos funcionales y no funcionales\.

__ID__

__Regla__

__Fuente__

__Evalúa__

__Historia__

RN\-12

Las actividades se clasifican diferenciando sin entrega, pendientes de calificar y completadas; los estados no se mezclan\.

RF\-06

Código

HU\-08

RN\-13

Las alertas aplican reglas configurables, muestran su evidencia y no establecen etiquetas definitivas sobre el estudiante\.

RF\-07, RNF\-08

Código y diseño

HU\-09

RN\-14

El agente de IA solo propone: usa herramientas controladas, distingue hechos, inferencias y recomendaciones, reconoce incertidumbre y la decisión pedagógica corresponde al docente\.

RF\-09, RF\-10, RNF\-08

Diseño y docente

HU\-11, HU\-12

RN\-15

La auditoría registra sincronizaciones, consultas y herramientas utilizadas sin exponer secretos\.

RF\-12, RNF\-01

Código

HU\-14

RN\-16

Toda recomendación se justifica con al menos un indicador académico; sin indicadores suficientes no se genera\.

RF\-10

Código y diseño

HU\-12

RN\-17

Todo informe incluye período, métricas, hallazgos, recomendaciones y fecha de actualización de los datos, e indica si los datos están incompletos\.

RF\-11, RNF\-03

Código

HU\-13

RN\-18

El modelo de lenguaje se ejecuta de forma local y no se envía información identificable a servicios externos sin autorización\.

RNF\-02

Diseño

HU\-11

RN\-19

El agente solo accede a los datos mediante herramientas de solo lectura y con los permisos del docente que consulta \(acceso por curso y mínimo privilegio\)\.

RF\-09, RNF\-01

Código

HU\-11

*Tabla 5\. Reglas de negocio aplicables al Sprint 2*

# <a id="_Toc245118804"></a><a id="_Toc245119157"></a><a id="_Toc245130119"></a>8\. ANÁLISIS DE CAPACIDAD DEL SPRINT 2

__Supuestos declarados__ \(no hay velocidad histórica confiable para trabajo de diseño\):

- Días de ejecución de tareas: 05, 06, 09, 10 y 11 de noviembre de 2026 \(5 días\)\. El 04/11 corresponde al inicio y a la reunión de planificación\.
- Dedicación supuesta: 4 horas diarias por integrante \(20 horas brutas\)\.
- Reserva para imprevistos: 10 % de las horas brutas \(incluye la reunión diaria de 15 minutos\)\.
- El 10/11/2026 se entrega la planificación; el 11/11/2026 se destina a ajustes finales y al ensayo de la presentación; el 12/11/2026 es la presentación\.

__Integrante__

__Responsabilidad técnica__

__Horas brutas__

__Reserva para imprevistos__

__Horas netas__

__Horas planificadas__

__Holgura__

Curillo Pilamunga Diego Alexander

Diseño técnico, arquitectura e IA

20 h

2 h

18 h

13 h

5 h

Manobanda Masabanda Luis Alfredo

Modelos de datos y diagramas

20 h

2 h

18 h

11,5 h

6,5 h

Redroban Chimbo Joselyn Amparo

Mockups y experiencia de usuario

20 h

2 h

18 h

12,5 h

5,5 h

Quispe Nasimba Daniela Alejandra

Historias, criterios y pruebas funcionales

20 h

2 h

18 h

13 h

5 h

__Total__

__80 h__

__8 h__

__72 h__

__50 h__

__22 h__

*Tabla 6\. Análisis de capacidad del equipo en el Sprint 2*

Las horas planificadas \(50 h\) no superan la capacidad neta \(72 h\) y dejan 22 horas de holgura\. Como la planificación se entrega el 10/11, las tareas de las historias se programan entre el 05 y el 10 de noviembre \(4 días, 57,6 h netas\) y se reserva el 11/11 para el ensayo\.

# <a id="_Toc245118805"></a><a id="_Toc245119158"></a><a id="_Toc245130120"></a>9\. DISEÑOS Y MODELOS TRANSVERSALES

Esta sección presenta la propuesta inicial de los diseños que cruzan varias historias\. Se refina y valida durante el Sprint \(tareas TE\-01 y TE\-03\)\.

## <a id="_Toc245118806"></a><a id="_Toc245119159"></a><a id="_Toc245130121"></a>9\.1 Arquitectura ampliada

__Componente__

__Responsabilidad__

__Estado__

React\.js

Interfaz de todas las pantallas, incluidas M\-07 a M\-14\.

Ampliado

Servicio de Identidad \(\.NET 10\)

Usuarios, roles y JWT\.

Existente \(Sprint 1\)

Servicio de Integración con Moodle \(\.NET 10\)

Conexión y consumo de los servicios web REST de Moodle\.

Existente \(Sprint 1\)

Servicio Académico \(\.NET 10\)

Cursos, estudiantes, calificaciones, indicadores, seguimiento y análisis grupal\.

Ampliado

Servicio de Alertas \(\.NET 10\)

Reglas de alerta, evaluación, alertas y evidencias\.

Nuevo \(diseño\)

Servicio de Informes \(\.NET 10\)

Generación y descarga de informes\.

Nuevo \(diseño\)

Servicio de Auditoría \(\.NET 10\)

Registro y consulta de eventos sin secretos\.

Nuevo \(diseño\)

Servicio de IA \(Python / FastAPI \+ LangGraph \+ Ollama\)

Agente con herramientas controladas y recomendaciones\.

Nuevo \(diseño\)

SQL Server

Persistencia relacional\.

Ampliado

Moodle \(externo\)

Fuente de datos académicos\.

Existente

*Tabla 7\. Componentes de la arquitectura ampliada*

  
![](imagen)

*Figura 1\. Arquitectura ampliada de EduMonitor \(diseño\)*

## <a id="_Toc245118807"></a><a id="_Toc245119160"></a><a id="_Toc245130122"></a>9\.2 Modelo de datos ampliado

Las entidades del Sprint 1 \(Usuario, Rol, ConfiguracionMoodle, Curso, Estudiante, Matricula, AsignacionDocenteCurso, ItemCalificacion, Calificacion, EstadoActividad y SincronizacionLog\) se mantienen\. Se incorporan las siguientes:

__Entidad nueva__

__Historia__

__Relación principal__

ReglaAlerta

HU\-09

Pertenece a un Curso\.

Alerta

HU\-09

Se genera para un Estudiante de un Curso a partir de una ReglaAlerta\.

EvidenciaAlerta

HU\-09

Pertenece a una Alerta y referencia un ItemCalificacion\.

ConsultaAgente

HU\-11

La realiza un Usuario sobre un Curso\.

RespuestaAgente

HU\-11

Pertenece a una ConsultaAgente\.

LlamadaHerramienta

HU\-11

Pertenece a una ConsultaAgente\.

Recomendacion

HU\-12

Pertenece a un Curso y, opcionalmente, a un Estudiante\.

Informe

HU\-13

Pertenece a un Curso\.

RegistroAuditoria

HU\-14

Se asocia a un Usuario\. Amplía SincronizacionLog\.

*Tabla 8\. Entidades nuevas del modelo de datos ampliado*

  
![](imagen)

*Figura 2\. Entidades nuevas del modelo de datos \(diseño\)*

## <a id="_Toc245118808"></a><a id="_Toc245119161"></a><a id="_Toc245130123"></a>9\.3 Flujo de una consulta al agente de IA

<a id="_Toc245119162"></a>![](imagen)

*Figura 3\. Secuencia de una consulta al agente \(diseño\)*

# <a id="_Toc245118809"></a><a id="_Toc245119163"></a><a id="_Toc245130124"></a>10\. HISTORIAS DE USUARIO DEL SPRINT 2

## <a id="_Toc245118810"></a><a id="_Toc245119164"></a><a id="_Toc245130125"></a>10\.1 Tabla resumen

__Código HU__

__Código RF__

__Nombre de la historia__

__Historia de Usuario \(Como / Quiero / Para\)__

__Prioridad__

__Estimación__

HU\-08

RF\-06

Consultar el seguimiento individual de un estudiante

__Como__ docente, __quiero__ consultar la evolución de las notas de un estudiante y el estado de sus actividades, __para__ conocer su situación sin revisar Moodle actividad por actividad\.

Alta

5 SP

HU\-09

RF\-07

Configurar reglas y consultar alertas académicas

__Como__ docente, __quiero__ configurar reglas de alerta y ver las alertas con su evidencia, __para__ actuar a tiempo sin etiquetar de forma definitiva a un estudiante\.

Alta

8 SP

HU\-10

RF\-08

Consultar el análisis grupal del curso

__Como__ docente, __quiero__ identificar las actividades con resultados bajos y la tendencia del curso, __para__ ajustar mi enseñanza\.

Media

5 SP

HU\-11

RF\-09

Consultar al agente de IA

__Como__ docente, __quiero__ hacer preguntas sobre mis cursos y recibir respuestas basadas en datos reales, __para__ obtener información sin navegar por varias pantallas\.

Alta

8 SP

HU\-12

RF\-10

Recibir recomendaciones pedagógicas justificadas

__Como__ docente, __quiero__ recibir recomendaciones de apoyo concretas y justificadas con indicadores, __para__ decidir cómo reforzar a mis estudiantes\.

Alta

5 SP

HU\-13

RF\-11

Generar y descargar el informe del curso

__Como__ docente, __quiero__ generar y descargar un informe resumen de un curso, __para__ documentar y compartir los hallazgos del período\.

Media

5 SP

HU\-14

RF\-12

Consultar el registro de auditoría

__Como__ administrador técnico, __quiero__ consultar el registro de sincronizaciones, consultas y herramientas del agente, __para__ garantizar la trazabilidad sin exponer secretos\.

Media

3 SP

__Total__

__39 SP__

*Tabla 9\. Resumen de las Historias de Usuario del Sprint 2*

## <a id="_Toc245118811"></a><a id="_Toc245119165"></a><a id="_Toc245130126"></a>10\.2 Fichas, mockups, modelos, diseños y tareas por historia

Cada historia incluye su ficha \(con un programador responsable principal\), sus mockups, el modelo y el diseño técnico previstos, y el desglose de sus tareas\. En este Sprint todas las tareas son de especificación y diseño; ninguna implica escribir código de producción\. El programador responsable de cada ficha es quien concentra la mayor carga de tareas de la historia\.

### <a id="_Toc245118812"></a><a id="_Toc245119166"></a><a id="_Toc245130127"></a>HU\-08 — Consultar el seguimiento individual de un estudiante

__Historia de Usuario__

__Número:__ 8

__Usuario:__ Docente

__Nombre historia:__ Consultar el seguimiento individual de un estudiante

__Prioridad en negocio:__ Alta

__Riesgo en desarrollo:__ Media

__Puntos estimados:__ 5 SP \(5,0 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Como docente, quiero consultar la evolución de las notas de un estudiante y el estado de sus actividades, para conocer su situación sin revisar Moodle actividad por actividad\. \(RF\-06, RN\-07, RN\-08, RN\-09, RN\-12\)

__Validación \(criterios de aceptación\):__

• CA\-08\.1: Dado un docente con un curso autorizado, cuando selecciona un estudiante, entonces ve la evolución de sus notas por ítem, ordenada por fecha y con la escala explícita\.

• CA\-08\.2: Dadas las actividades del estudiante, cuando se listan, entonces cada una aparece clasificada como Sin entrega, Pendiente de calificar o Completada, sin mezclar los estados\.

• CA\-08\.3: Dada una nota ausente en Moodle, cuando se muestra, entonces aparece como «sin dato» y no como cero\.

• CA\-08\.4: Dado un docente sin autorización sobre el curso, cuando solicita el seguimiento por identificador directo, entonces el sistema responde 403\.

__Mockups asociados:__ M\-07

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-08\.x

*Tabla 10\. Ficha de la Historia de Usuario HU\-08*

__*Mockup de HU\-08*__

__Mockup M\-07 — Seguimiento individual__ \(rol: Docente; ruta: /cursos/\{id\}/estudiantes/\{id\}\)

![](imagen)

*Figura 4\. Mockup M\-07 — Seguimiento individual*

__*Modelo y diseño — HU\-08*__

__Elemento del modelo__

__Atributos clave__

__Observación__

EstadoActividadClasificado

Valor: SinEntrega, PendienteCalificar, Completada

Se deriva de los estados importados desde Moodle en el Sprint 1\.

SeguimientoEstudianteDto

Notas ordenadas por fecha, escala, actividades clasificadas, fecha de última sincronización

Objeto de transferencia; no es una tabla nueva\.

*Tabla 11\. Modelo y diseño de HU\-08*

- Servicio: Servicio Académico \(\.NET 10, Clean Architecture\); no requiere un servicio nuevo\.
- Endpoint previsto: GET /cursos/\{cursoId\}/estudiantes/\{estudianteId\}/seguimiento, con autorización por curso \(RN\-02\)\.
- La clasificación de actividades se define en la capa de aplicación a partir del estado de finalización y de calificación importado; los casos que Moodle no permita distinguir se marcan «no disponible» \(RN\-05\)\.

__*Desglose de tareas — HU\-08*__

__Tarea__

__Número tarea:__ TA\-08\.1

__Número historia:__ HU\-08

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Redactar la historia en formato Como / Quiero / Para, refinarla con el Product Owner, definir los criterios CA\-08\.1 a CA\-08\.4 y vincular las reglas RN\-07, RN\-08, RN\-09 y RN\-12\.

*Tabla 12\. Tarea TA\-08\.1*

__Tarea__

__Número tarea:__ TA\-08\.2

__Número historia:__ HU\-08

__Nombre tarea:__ Mockup M\-07 Seguimiento individual

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Diseñar el boceto de la pantalla: evolución de notas con escala, clasificación de actividades \(sin entrega, pendiente de calificar, completada\), advertencias de datos incompletos y pie de fuente y fecha\.

*Tabla 13\. Tarea TA\-08\.2*

__Tarea__

__Número tarea:__ TA\-08\.3

__Número historia:__ HU\-08

__Nombre tarea:__ Modelo y diseño técnico del seguimiento individual

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Definir los objetos de transferencia, la regla de clasificación de estados de actividad, el endpoint previsto y el diagrama de clases o secuencia asociado\.

*Tabla 14\. Tarea TA\-08\.3*

__Tarea__

__Número tarea:__ TA\-08\.4

__Número historia:__ HU\-08

__Nombre tarea:__ Casos de prueba funcional de HU\-08

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Diseñar los casos CP\-08\.1 a CP\-08\.4 que verifican cada criterio de aceptación, con datos de prueba, pasos y resultado esperado\.

*Tabla 15\. Tarea TA\-08\.4*

### <a id="_Toc245118813"></a><a id="_Toc245119167"></a><a id="_Toc245130128"></a>HU\-09 — Configurar reglas y consultar alertas académicas

__Historia de Usuario__

__Número:__ 9

__Usuario:__ Docente

__Nombre historia:__ Configurar reglas y consultar alertas académicas

__Prioridad en negocio:__ Alta

__Riesgo en desarrollo:__ Alta

__Puntos estimados:__ 8 SP \(7,5 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Como docente, quiero configurar reglas de alerta y ver las alertas detectadas con su evidencia, para actuar a tiempo sin que el sistema etiquete de forma definitiva a un estudiante\. \(RF\-07, RN\-07, RN\-09, RN\-13\)

__Validación \(criterios de aceptación\):__

• CA\-09\.1: Dado un docente, cuando modifica el umbral o el estado de una regla de alerta y guarda, entonces el cambio rige en la siguiente evaluación sin recompilar\.

• CA\-09\.2: Dados datos sincronizados, cuando se evalúan las reglas, entonces se generan alertas por descenso en evaluaciones, por atraso y por baja finalización\.

• CA\-09\.3: Dada una alerta, cuando se abre su detalle, entonces muestra la evidencia \(ítems, valores, fechas y regla aplicada\) y su texto no usa etiquetas definitivas sobre el estudiante\.

• CA\-09\.4: Dada una sincronización incompleta, cuando se evalúan las reglas, entonces no se generan alertas basadas en datos faltantes y se informa al docente \(RN\-07\)\.

• CA\-09\.5: Dado el panel de alertas, cuando se muestra, entonces indica la fuente y la fecha de la última sincronización \(RN\-09\)\.

__Mockups asociados:__ M\-08, M\-09

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-09\.x

*Tabla 16\. Ficha de la Historia de Usuario HU\-09*

__*Mockup de HU\-09*__

__Mockup M\-08 — Reglas de alerta__ \(rol: Docente; ruta: /cursos/\{id\}/alertas/reglas\)

![](imagen)

*Figura 5\. Mockup M\-08 — Reglas de alerta*

__Mockup M\-09 — Panel de alertas__ \(rol: Docente; ruta: /cursos/\{id\}/alertas\)

![](imagen)

*Figura 6\. Mockup M\-09 — Panel de alertas*

__*Modelo y diseño — HU\-09*__

__Elemento del modelo__

__Atributos clave__

__Observación__

ReglaAlerta

Id, CursoId, Tipo \(Descenso, Atraso, BajaFinalizacion\), Umbral, Activa

Un conjunto de reglas por curso\.

Alerta

Id, CursoId, EstudianteId, ReglaId, FechaGeneracion, Estado \(Vigente, Revisada, Descartada\)

Se genera al evaluar reglas\.

EvidenciaAlerta

AlertaId, ItemId, Valor, Fecha, Descripcion

Respalda cada alerta con datos concretos\.

*Tabla 17\. Modelo y diseño de HU\-09*

- Servicio nuevo: Servicio de Alertas \(\.NET 10, Clean Architecture\) con un evaluador por tipo de regla\.
- Endpoints previstos: GET y PUT /cursos/\{id\}/alertas/reglas; GET /cursos/\{id\}/alertas; PATCH /alertas/\{id\}/estado\.
- Evaluación: se dispara al terminar una sincronización completa; si fue incompleta no se evalúan las reglas que dependan de datos faltantes\.
- Los valores por defecto de los umbrales se acuerdan con el Product Owner; el diseño no fija cifras\.

__*Desglose de tareas — HU\-09*__

__Tarea__

__Número tarea:__ TA\-09\.1

__Número historia:__ HU\-09

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Redactar la historia, definir los criterios CA\-09\.1 a CA\-09\.5 y los tipos de regla de alerta \(descenso en evaluaciones, atraso y baja finalización\), cuidando que no se usen etiquetas definitivas \(RN\-13\)\.

*Tabla 18\. Tarea TA\-09\.1*

__Tarea__

__Número tarea:__ TAREA 09\.2

__Número historia:__ HU\-09

__Nombre tarea:__ Mockups M\-08 y M\-09 \(reglas y panel de alertas\)

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Diseñar la pantalla de configuración de reglas de alerta \(M\-08\) y el panel de alertas con su evidencia \(M\-09\)\.

*Tabla 19\. Tarea TA\-09\.2*

__Tarea__

__Número tarea:__ TAREA 09\.3

__Número historia:__ HU\-09

__Nombre tarea:__ Modelo de datos y diseño del motor de reglas

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Definir las entidades ReglaAlerta, Alerta y EvidenciaAlerta, el diseño del servicio de alertas, el momento de evaluación tras una sincronización completa y el manejo de datos faltantes\.

*Tabla 20\. Tarea TA\-09\.3*

__Tarea__

__Número tarea:__ TAREA 09\.4

__Número historia:__ HU\-09

__Nombre tarea:__ Casos de prueba funcional de HU\-09

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Diseñar los casos CP\-09\.1 a CP\-09\.5 con datos de curso que generen descenso, atraso y baja finalización conocidos\.

*Tabla 21\. Tarea TA\-09\.4*

### <a id="_Toc245118814"></a><a id="_Toc245119168"></a><a id="_Toc245130129"></a>HU\-10 — Consultar el análisis grupal del curso

__Historia de Usuario__

__Número:__ 10

__Usuario:__ Docente

__Nombre historia:__ Consultar el análisis grupal del curso

__Prioridad en negocio:__ Media

__Riesgo en desarrollo:__ Media

__Puntos estimados:__ 5 SP \(5,0 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Como docente, quiero identificar las actividades con resultados bajos y la tendencia del rendimiento del curso, para ajustar mi enseñanza\. \(RF\-08, RN\-07, RN\-08, RN\-09\)

__Validación \(criterios de aceptación\):__

• CA\-10\.1: Dado un curso con datos sincronizados, cuando el docente abre el análisis grupal, entonces ve las actividades ordenadas por resultado promedio y resaltadas las que están bajo el umbral configurable\.

• CA\-10\.2: Dado el análisis, cuando se muestra la tendencia, entonces se ve la evolución del promedio del curso por fecha de evaluación con su escala\.

• CA\-10\.3: Dada una actividad con menos calificaciones que el mínimo definido, cuando se calcula su resultado, entonces se marca «datos insuficientes» y no se concluye sobre ella\.

• CA\-10\.4: Dado cualquier resultado mostrado, cuando se observa el análisis, entonces indica la fuente y la fecha de la última sincronización\.

__Mockups asociados:__ M\-10

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-10\.x

*Tabla 22\. Ficha de la Historia de Usuario HU\-10*

__*Mockup de HU\-10*__

__Mockup M\-10 — Análisis grupal__ \(rol: Docente; ruta: /cursos/\{id\}/analisis\)

![](imagen)

*Figura 7\. Mockup M\-10 — Análisis grupal*

__*Modelo y diseño — HU\-10*__

__Elemento del modelo__

__Atributos clave__

__Observación__

ResultadoActividadDto

ItemId, Nombre, Promedio, Escala, NumCalificaciones, BajoUmbral, DatosInsuficientes

Objeto de transferencia calculado con consultas agregadas\.

PuntoTendenciaDto

Fecha, PromedioCurso, Escala

Serie para el gráfico de tendencia\.

*Tabla 23\. Modelo y diseño de HU\-10*

- Servicio: Servicio Académico; las agregaciones se calculan en SQL Server sobre las calificaciones sincronizadas\.
- Endpoint previsto: GET /cursos/\{cursoId\}/analisis\-grupal, con autorización por curso\.
- El umbral de «resultado bajo» y el mínimo de calificaciones son parámetros configurables; sus valores iniciales se acuerdan con el Product Owner\.

__*Desglose de tareas — HU\-10*__

__Tarea__

__Número tarea:__ TAREA 10\.1

__Número historia:__ HU\-10

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Redactar la historia y los criterios CA\-10\.1 a CA\-10\.4; acordar con el Product Owner el umbral inicial de "resultado bajo" y el mínimo de calificaciones para concluir\.

*Tabla 24\. Tarea TA\-10\.1*

__Tarea__

__Número tarea:__ TAREA 10\.2

__Número historia:__ HU\-10

__Nombre tarea:__ Mockup M\-10 Análisis grupal

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Diseñar la pantalla con las actividades ordenadas por resultado, el resaltado de las que están bajo el umbral y la tendencia del promedio del curso\.

*Tabla 25\. Tarea TA\-10\.2*

__Tarea__

__Número tarea:__ TAREA 10\.3

__Número historia:__ HU\-10

__Nombre tarea:__ Modelo y diseño técnico del análisis grupal

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Definir las consultas agregadas por actividad y por fecha, el objeto de resultado de actividad, el endpoint previsto y el tratamiento de «datos insuficientes»\.

*Tabla 26\. Tarea TA\-10\.3*

__Tarea__

__Número tarea:__ TAREA 10\.4

__Número historia:__ HU\-10

__Nombre tarea:__ Casos de prueba funcional de HU\-10

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Diseñar los casos CP\-10\.1 a CP\-10\.4 con cursos de actividades de promedios distintos y con pocas calificaciones\.

*Tabla 27\. Tarea TA\-10\.4*

### <a id="_Toc245118815"></a><a id="_Toc245119169"></a><a id="_Toc245130130"></a>HU\-11 — Consultar al agente de IA

__Historia de Usuario__

__Número:__ 11

__Usuario:__ Docente

__Nombre historia:__ Consultar al agente de IA

__Prioridad en negocio:__ Alta

__Riesgo en desarrollo:__ Alta

__Puntos estimados:__ 8 SP \(10,5 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Como docente, quiero hacer preguntas sobre mis cursos y recibir respuestas basadas en datos reales, para obtener información sin navegar por varias pantallas\. \(RF\-09, RN\-14, RN\-18, RN\-19, RNF\-08\)

__Validación \(criterios de aceptación\):__

• CA\-11\.1: Dada una pregunta sobre un curso autorizado, cuando el agente responde, entonces la respuesta se basa en datos obtenidos con herramientas controladas del sistema y no en información inventada\.

• CA\-11\.2: Dada una respuesta, cuando se muestra, entonces distingue de forma explícita los hechos, las inferencias y las recomendaciones\.

• CA\-11\.3: Dada una pregunta sin información suficiente, cuando el agente responde, entonces reconoce la incertidumbre y no inventa datos\.

• CA\-11\.4: Dada una pregunta sobre un curso que el docente no tiene autorizado, cuando el agente intenta consultar, entonces la herramienta rechaza el acceso y el agente lo informa\.

• CA\-11\.5: Dada cualquier consulta, cuando se procesa, entonces el modelo se ejecuta localmente y no se envía información identificable a servicios externos\.

__Mockups asociados:__ M\-11

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-11\.x

*Tabla 28\. Ficha de la Historia de Usuario HU\-11*

__*Mockup de HU\-11*__

__Mockup M\-11 — Consulta al agente__ \(rol: Docente; ruta: /cursos/\{id\}/agente\)

![](imagen)

*Figura 8\. Mockup M\-11 — Consulta al agente*

__*Modelo y diseño — HU\-11*__

__Elemento del modelo__

__Atributos clave__

__Observación__

ConsultaAgente

Id, UsuarioId, CursoId, Pregunta, FechaHora

Registro de cada pregunta\.

RespuestaAgente

Id, ConsultaId, Hechos, Inferencias, Recomendaciones, Incertidumbre

Respuesta estructurada en tres bloques\.

LlamadaHerramienta

Id, ConsultaId, Herramienta, Parametros, ResultadoResumen, FechaHora

Trazabilidad de las herramientas usadas\.

*Tabla 29\. Modelo y diseño de HU\-11*

- Servicio nuevo: Servicio de IA \(Python / FastAPI\) con un grafo de LangGraph y un modelo de Ollama ejecutado en local\.
- Flujo del grafo: interpretar la pregunta → elegir herramienta → ejecutar herramienta → redactar con el modelo → separar hechos, inferencias y recomendaciones → responder\.
- Herramientas controladas de solo lectura, que llaman a los servicios \.NET con el JWT del docente: obtener\_indicadores\_curso, obtener\_seguimiento\_estudiante, listar\_alertas y obtener\_resultados\_actividades\.
- Endpoint previsto: POST /agente/consultas\. Ninguna herramienta modifica datos; el control de acceso por curso se aplica en los servicios \(RN\-19\)\.
- Privacidad: el modelo no sale de la infraestructura del proyecto \(RN\-18, RNF\-02\)\.

__*Desglose de tareas — HU\-11*__

__Tarea__

__Número tarea:__ TAREA 11\.1

__Número historia:__ HU\-11

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Redactar la historia, definir los criterios CA\-11\.1 a CA\-11\.5 y las reglas RN\-14, RN\-18 y RN\-19 sobre herramientas controladas, distinción de hechos, inferencias y recomendaciones, e incertidumbre\.

*Tabla 30\. Tarea TA\-11\.1*

__Tarea__

__Número tarea:__ TAREA 11\.2

__Número historia:__ HU\-11

__Nombre tarea:__ Mockup M\-11 Consulta al agente

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,0 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Diseñar la pantalla de consulta con la pregunta, la respuesta en bloques \(hechos, inferencias, recomendaciones\), el aviso de incertidumbre y las herramientas utilizadas\.

*Tabla 31\. Tarea TA\-11\.2*

__Tarea__

__Número tarea:__ TAREA 11\.3

__Número historia:__ HU\-11

__Nombre tarea:__ Diseño de la arquitectura de los servicios de IA

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 3,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Diseñar el servicio de IA \(Python / FastAPI\), el grafo de LangGraph, las herramientas controladas de solo lectura, el uso de Ollama en local y los límites de privacidad\.

*Tabla 32\. Tarea TA\-11\.3*

__Tarea__

__Número tarea:__ TAREA 11\.4

__Número historia:__ HU\-11

__Nombre tarea:__ Modelo de datos y diagrama de secuencia del agente

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,0 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Definir las entidades ConsultaAgente, RespuestaAgente y LlamadaHerramienta y el diagrama de secuencia de una consulta\.

*Tabla 33\. Tarea TA\-11\.4*

__Tarea__

__Número tarea:__ TAREA 11\.5

__Número historia:__ HU\-11

__Nombre tarea:__ Casos de prueba funcional de HU\-11

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Diseñar los casos CP\-11\.1 a CP\-11\.5, incluyendo verificación de hechos contra los datos, rechazo por curso no autorizado y ausencia de envío de datos identificables a servicios externos\.

*Tabla 34\. Tarea TA\-11\.5*

### <a id="_Toc245118816"></a><a id="_Toc245119170"></a><a id="_Toc245130131"></a>HU\-12 — Recibir recomendaciones pedagógicas justificadas

__Historia de Usuario__

__Número:__ 12

__Usuario:__ Docente

__Nombre historia:__ Recibir recomendaciones pedagógicas justificadas

__Prioridad en negocio:__ Alta

__Riesgo en desarrollo:__ Media

__Puntos estimados:__ 5 SP \(5,0 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Como docente, quiero recibir recomendaciones de apoyo concretas y justificadas con indicadores, para decidir cómo reforzar a mis estudiantes\. \(RF\-10, RN\-14, RN\-16\)

__Validación \(criterios de aceptación\):__

• CA\-12\.1: Dado un estudiante o curso con alertas o indicadores, cuando el docente solicita recomendaciones, entonces se proponen acciones concretas indicando qué hacer y sobre qué actividad o tema\.

• CA\-12\.2: Dada una recomendación, cuando se muestra, entonces incluye la justificación con los indicadores que la sustentan \(valor, actividad y fecha\)\.

• CA\-12\.3: Dada una recomendación propuesta, cuando el docente la revisa, entonces se identifica como propuesta del sistema y puede aceptarla, ajustarla o descartarla\.

• CA\-12\.4: Dados datos insuficientes, cuando se solicitan recomendaciones, entonces no se generan y se explica el motivo\.

__Mockups asociados:__ M\-12

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-12\.x

*Tabla 35\. Ficha de la Historia de Usuario HU\-12*

__*Mockup de HU\-12*__

__Mockup M\-12 — Recomendaciones__ \(rol: Docente; ruta: /cursos/\{id\}/recomendaciones\)

![](imagen)

*Figura 9\. Mockup M\-12 — Recomendaciones*

__*Modelo y diseño — HU\-12*__

__Elemento del modelo__

__Atributos clave__

__Observación__

Recomendacion

Id, CursoId, EstudianteId \(opcional\), Texto, Justificacion, IndicadoresRef, Estado \(Propuesta, Aceptada, Ajustada, Descartada\), Fecha

Se vincula con alertas e indicadores\.

*Tabla 36\. Modelo y diseño de HU\-12*

- Servicio: el Servicio de IA genera la propuesta usando las herramientas de HU\-11; el Servicio Académico almacena y expone el estado\.
- Endpoints previstos: POST /cursos/\{id\}/recomendaciones/generar; GET /cursos/\{id\}/recomendaciones; PATCH /recomendaciones/\{id\}/estado\.
- Cada recomendación debe referenciar al menos un indicador; si no existe, no se genera \(RN\-16\)\.

__*Desglose de tareas — HU\-12*__

__Tarea__

__Número tarea:__ TAREA 12\.1

__Número historia:__ HU\-12

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Redactar la historia y los criterios CA\-12\.1 a CA\-12\.4; vincular la regla RN\-16 \(recomendación justificada con indicadores\)\.

*Tabla 37\. Tarea TA\-12\.1*

__Tarea__

__Número tarea:__ TAREA 12\.2

__Número historia:__ HU\-12

__Nombre tarea:__ Mockup M\-12 Recomendaciones

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Diseñar la pantalla con las recomendaciones, su justificación con indicadores y las acciones aceptar, ajustar y descartar\.

*Tabla 38\. Tarea TA\-12\.2*

__Tarea__

__Número tarea:__ TAREA 12\.3

__Número historia:__ HU\-12

__Nombre tarea:__ Modelo y diseño técnico de recomendaciones

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Definir la entidad Recomendacion, su relación con alertas e indicadores, los estados y el flujo de generación por el servicio de IA\.

*Tabla 39\. Tarea TA\-12\.3*

__Tarea__

__Número tarea:__ TAREA 12\.4

__Número historia:__ HU\-12

__Nombre tarea:__ Casos de prueba funcional de HU\-12

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Diseñar los casos CP\-12\.1 a CP\-12\.4, incluido el caso de datos insuficientes\.

*Tabla 40\. Tarea TA\-12\.4*

### <a id="_Toc245118817"></a><a id="_Toc245119171"></a><a id="_Toc245130132"></a>HU\-13 — Generar y descargar el informe del curso

__Historia de Usuario__

__Número:__ 13

__Usuario:__ Docente

__Nombre historia:__ Generar y descargar el informe del curso

__Prioridad en negocio:__ Media

__Riesgo en desarrollo:__ Media

__Puntos estimados:__ 5 SP \(5,0 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Como docente, quiero generar y descargar un informe resumen de un curso, para documentar y compartir los hallazgos del período\. \(RF\-11, RN\-07, RN\-09, RN\-17\)

__Validación \(criterios de aceptación\):__

• CA\-13\.1: Dado un curso y un período, cuando el docente genera el informe, entonces este incluye el período, las métricas, los hallazgos, las recomendaciones y la fecha de actualización de los datos\.

• CA\-13\.2: Dado un informe generado, cuando el docente lo consulta, entonces puede verlo en el sistema y descargarlo en el formato acordado con el Product Owner\.

• CA\-13\.3: Dado un curso con sincronización incompleta, cuando se genera el informe, entonces este indica expresamente que los datos están incompletos\.

• CA\-13\.4: Dado un docente sin autorización sobre el curso, cuando solicita su informe, entonces el sistema responde 403\.

__Mockups asociados:__ M\-13

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-13\.x

*Tabla 41\. Ficha de la Historia de Usuario HU\-13*

__*Mockup de HU\-13*__

__Mockup M\-13 — Informe__ \(rol: Docente; ruta: /cursos/\{id\}/informes\)

![](imagen)

*Figura 10\. Mockup M\-13 — Informe*

__*Modelo y diseño — HU\-13*__

__Elemento del modelo__

__Atributos clave__

__Observación__

Informe

Id, CursoId, EstudianteId \(opcional\), Periodo, FechaGeneracion, FechaActualizacionDatos, Contenido, Formato

Contenido estructurado en las cinco secciones mínimas\.

*Tabla 42\. Modelo y diseño de HU\-13*

- Servicio nuevo: Servicio de Informes \(\.NET 10, Clean Architecture\) que consume indicadores, alertas y recomendaciones de los otros servicios\.
- Endpoints previstos: POST /cursos/\{id\}/informes; GET /informes/\{id\}; GET /informes/\{id\}/descarga\.
- Formato descargable propuesto: PDF; se confirma con el Product Owner en la reunión de planificación\.

__*Desglose de tareas — HU\-13*__

__Tarea__

__Número tarea:__ TAREA 13\.1

__Número historia:__ HU\-13

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Redactar la historia y los criterios CA\-13\.1 a CA\-13\.4; definir con el Product Owner el formato descargable \(propuesta: PDF\) y el contenido mínimo del informe \(RN\-17\)\.

*Tabla 43\. Tarea TA\-13\.1*

__Tarea__

__Número tarea:__ TAREA 13\.2

__Número historia:__ HU\-13

__Nombre tarea:__ Mockup M\-13 Informe

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Diseñar la pantalla de generación y consulta de informes con vista previa, descarga y listado de informes generados\.

*Tabla 44\. Tarea TA\-13\.2*

__Tarea__

__Número tarea:__ TAREA 13\.3

__Número historia:__ HU\-13

__Nombre tarea:__ Modelo y diseño técnico del informe

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 06/11/2026

__Fecha fin:__ 06/11/2026

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Definir la entidad Informe, la estructura de contenido, el servicio de informes y el endpoint de descarga\.

*Tabla 45\. Tarea TA\-13\.3*

__Tarea__

__Número tarea:__ TAREA 13\.4

__Número historia:__ HU\-13

__Nombre tarea:__ Casos de prueba funcional de HU\-13

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Diseñar los casos CP\-13\.1 a CP\-13\.4\.

*Tabla 46\. Tarea TA\-13\.4*

### <a id="_Toc245118818"></a><a id="_Toc245119172"></a><a id="_Toc245130133"></a>HU\-14 — Consultar el registro de auditoría

__Historia de Usuario__

__Número:__ 14

__Usuario:__ Administrador técnico

__Nombre historia:__ Consultar el registro de auditoría

__Prioridad en negocio:__ Media

__Riesgo en desarrollo:__ Baja

__Puntos estimados:__ 3 SP \(4,0 h de tareas\)

__Iteración asignada:__ 2 \(especificación y diseño\)

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Como administrador técnico, quiero consultar el registro de sincronizaciones, consultas y herramientas usadas por el agente, para garantizar la trazabilidad sin exponer secretos\. \(RF\-12, RN\-15\)

__Validación \(criterios de aceptación\):__

• CA\-14\.1: Dada una sincronización, una consulta al agente o el uso de una herramienta, cuando ocurre, entonces se registra con el usuario, la fecha y el resultado\.

• CA\-14\.2: Dado el registro de auditoría, cuando se revisa su contenido, entonces no incluye credenciales, tokens ni contraseñas\.

• CA\-14\.3: Dado un administrador técnico, cuando filtra por tipo y por rango de fechas, entonces obtiene los registros coincidentes de forma paginada\.

• CA\-14\.4: Dado un usuario con rol Docente, cuando intenta consultar el registro de auditoría, entonces el sistema responde 403\.

__Mockups asociados:__ M\-14

__Entregables del Sprint 2:__ historia refinada, criterios, mockups, modelo, diseño técnico y casos de prueba CP\-14\.x

*Tabla 47\. Ficha de la Historia de Usuario HU\-14*

__*Mockup de HU\-14*__

__Mockup M\-14 — Consulta de auditoría__ \(rol: Administrador técnico; ruta: /auditoria\)

![](imagen)

*Figura 11\. Mockup M\-14 — Consulta de auditoría*

__*Modelo y diseño — HU\-14*__

__Elemento del modelo__

__Atributos clave__

__Observación__

RegistroAuditoria

Id, FechaHora, UsuarioId, Tipo \(Sincronizacion, ConsultaAgente, Herramienta\), Detalle, Resultado

Integra y amplía la bitácora de sincronización del Sprint 1\.

*Tabla 48\. Modelo y diseño de HU\-14*

- Servicio nuevo: Servicio de Auditoría \(\.NET 10\) que recibe eventos de los demás servicios\.
- Endpoint previsto: GET /auditoria con filtros por tipo y fechas y paginación \(RN\-11\)\.
- Antes de registrar un evento se eliminan los campos sensibles \(credenciales, tokens, contraseñas\)\.

__*Desglose de tareas — HU\-14*__

__Tarea__

__Número tarea:__ TAREA 14\.1

__Número historia:__ HU\-14

__Nombre tarea:__ Especificación de la historia y criterios de aceptación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 0,5 h

__Fecha inicio:__ 05/11/2026

__Fecha fin:__ 05/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Quispe Nasimba Daniela Alejandra

__Descripción:__ Redactar la historia y los criterios CA\-14\.1 a CA\-14\.4; vincular la regla RN\-15\.

*Tabla 49\. Tarea TA\-14\.1*

__Tarea__

__Número tarea:__ TAREA 14\.2

__Número historia:__ HU\-14

__Nombre tarea:__ Mockup M\-14 Consulta de auditoría

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 10/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Diseñar la pantalla con filtros por tipo y fecha, tabla paginada y aviso de que no se muestran secretos\.

*Tabla 50\. Tarea TA\-14\.2*

__Tarea__

__Número tarea:__ TAREA 14\.3

__Número historia:__ HU\-14

__Nombre tarea:__ Modelo y diseño técnico de auditoría

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 09/11/2026

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Definir la entidad RegistroAuditoria, los eventos que cada servicio notifica y la depuración de secretos antes de registrar\.

*Tabla 51\. Tarea TA\-14\.3*

__Tarea__

__Número tarea:__ TAREA 14\.4

__Número historia:__ HU\-14

__Nombre tarea:__ Casos de prueba funcional de HU\-14

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,0 h

__Fecha inicio:__ 10/11/2026

__Fecha fin:__ 10/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Diseñar los casos CP\-14\.1 a CP\-14\.4\.

*Tabla 52\. Tarea TA\-14\.4*

### <a id="_Toc245118819"></a><a id="_Toc245119173"></a><a id="_Toc245130134"></a>Tareas habilitadoras para el Sprint 2

__Tarea__

__Número tarea:__ TareaHabilitadora\-01

__Número historia:__ Tarea habilitadora

__Nombre tarea:__ Modelo de datos y arquitectura ampliados \(consolidación\)

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,5 h

__Fecha inicio:__ 09/11/2026

__Fecha fin:__ 10/11/2026

__Programador responsable:__ Manobanda Masabanda Luis Alfredo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Integrar en un solo modelo de datos y en un diagrama de componentes los diseños de HU\-08 a HU\-14, verificando la coherencia con el modelo del Sprint 1\.

*Tabla 53\. Tarea habilitadora TE\-01*

__Tarea__

__Número tarea:__ TareaHabilitadora\-02

__Número historia:__ Tarea habilitadora

__Nombre tarea:__ Matriz de trazabilidad y estrategia de pruebas funcionales

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,0 h

__Fecha inicio:__ 10/11/2026

__Fecha fin:__ 10/11/2026

__Programador responsable:__ Quispe Nasimba Daniela Alejandra

__Colaborador:__ Manobanda Masabanda Luis Alfredo

__Descripción:__ Elaborar la matriz RF – HU – criterio – caso de prueba – mockup y consolidar la estrategia de pruebas funcionales\.

*Tabla 54\. Tarea habilitadora TE\-02*

__Tarea__

__Número tarea:__ TareaHabilitadora\-03

__Número historia:__ Tarea habilitadora

__Nombre tarea:__ Consolidación y revisión del documento de planificación

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 2,0 h

__Fecha inicio:__ 10/11/2026

__Fecha fin:__ 10/11/2026

__Programador responsable:__ Curillo Pilamunga Diego Alexander

__Colaborador:__ Redroban Chimbo Joselyn Amparo

__Descripción:__ Unificar el documento, revisar la coherencia entre historias, mockups, modelos y pruebas, y entregarlo al Product Owner el 10/11/2026\.

*Tabla 55\. Tarea habilitadora TE\-03*

__Tarea__

__Número tarea:__ TareaHabilitadora\-04

__Número historia:__ Tarea habilitadora

__Nombre tarea:__ Preparación y ensayo de la presentación del Sprint 2

__Tipo de tarea:__ ☐ Desarrollo ☐ Corrección ☐ Mejora ☒ Otra \(diseño y documentación\)

__Horas estimadas:__ 1,5 h

__Fecha inicio:__ 11/11/2026

__Fecha fin:__ 11/11/2026

__Programador responsable:__ Redroban Chimbo Joselyn Amparo

__Colaborador:__ Curillo Pilamunga Diego Alexander

__Descripción:__ Preparar el guion y el material de la presentación del 12/11/2026 y ensayarla con todo el equipo\.

*Tabla 56\. Tarea habilitadora TE\-04*

# <a id="_Toc245118820"></a><a id="_Toc245119174"></a><a id="_Toc245130135"></a>11\. PLAN DE PRUEBAS FUNCIONALES

## <a id="_Toc245118821"></a><a id="_Toc245119175"></a><a id="_Toc245130136"></a>11\.1 Estrategia

Las pruebas funcionales verifican que cada criterio de aceptación se cumpla desde el punto de vista del usuario\. En el Sprint 2 __se diseñan__ los casos; su ejecución corresponde a los Sprints en que se construyan las funcionalidades\.

__Aspecto__

__Definición__

Qué se evalúa

Cada criterio de aceptación \(CA\) de HU\-08 a HU\-14: resultado visible, restricciones de acceso, manejo de datos incompletos y mensajes\.

Técnica

Pruebas de caja negra con partición por equivalencia y valores límite \(por ejemplo, actividades con pocas calificaciones o con nota ausente\)\.

Roles de prueba

Docente A \(con cursos asignados\), Docente B \(sin asignación al curso evaluado\) y Administrador técnico\.

Datos de prueba

Cursos de una instancia Moodle de pruebas con datos anonimizados \(RNF\-02\): curso completo, curso con sincronización incompleta, curso con notas ausentes y curso con pocas calificaciones\.

Pruebas del agente de IA

Al no ser determinista, se evalúa la estructura de la respuesta \(hechos, inferencias, recomendaciones\), que los hechos coincidan con los datos y que se reconozca la incertidumbre; no se compara texto exacto\.

Criterio de entrada

Historia implementada, datos de prueba cargados y usuarios de prueba disponibles\.

Criterio de salida

Todos los casos ejecutados con resultado registrado y sin defectos críticos abiertos\.

Registro

Cada caso registra: código, fecha, ejecutor, resultado \(Exitoso / Fallido\) y evidencia\.

*Tabla 57\. Estrategia de pruebas funcionales*

## <a id="_Toc245118822"></a><a id="_Toc245119176"></a><a id="_Toc245130137"></a>11\.2 Casos de prueba por criterio de aceptación

### <a id="_Toc245118823"></a><a id="_Toc245119177"></a><a id="_Toc245130138"></a>HU\-08 — Consultar el seguimiento individual de un estudiante

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-08\.1

CA\-08\.1

Docente A con curso asignado; estudiante con tres notas

Abrir el seguimiento del estudiante

Notas ordenadas por fecha, cada una con su escala

CP\-08\.2

CA\-08\.2

Estudiante con actividades en los tres estados

Abrir la lista de actividades

Cada actividad en su estado y contadores correctos

CP\-08\.3

CA\-08\.3

Nota ausente en Moodle

Abrir el seguimiento

Se muestra «sin dato», no cero

CP\-08\.4

CA\-08\.4

Docente B sin asignación al curso

Solicitar el seguimiento por URL directa

Respuesta 403

*Tabla 58\. Casos de prueba de HU\-08*

### <a id="_Toc245118824"></a><a id="_Toc245119178"></a><a id="_Toc245130139"></a>HU\-09 — Configurar reglas y consultar alertas académicas

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-09\.1

CA\-09\.1

Docente A con reglas activas

Cambiar un umbral, guardar y ejecutar la evaluación

El nuevo umbral se aplica y queda persistido, sin recompilar

CP\-09\.2

CA\-09\.2

Curso con descenso, atraso y baja finalización conocidos

Ejecutar la evaluación de reglas

Se generan las tres alertas esperadas y ninguna otra

CP\-09\.3

CA\-09\.3

Alerta generada

Abrir el detalle

Evidencia con ítems, valores, fechas y regla; texto sin etiquetas definitivas

CP\-09\.4

CA\-09\.4

Sincronización marcada «Incompleta»

Ejecutar la evaluación

No se generan alertas por datos faltantes y se muestra un aviso

CP\-09\.5

CA\-09\.5

Panel de alertas con datos

Abrir el panel

Se muestran la fuente y la fecha de sincronización

*Tabla 59\. Casos de prueba de HU\-09*

### <a id="_Toc245118825"></a><a id="_Toc245119179"></a><a id="_Toc245130140"></a>HU\-10 — Consultar el análisis grupal del curso

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-10\.1

CA\-10\.1

Curso con actividades de promedios distintos

Abrir el análisis grupal

Actividades ordenadas por promedio; las inferiores al umbral resaltadas

CP\-10\.2

CA\-10\.2

Curso con varias fechas de evaluación

Ver la tendencia

Evolución del promedio con su escala

CP\-10\.3

CA\-10\.3

Actividad con menos calificaciones que el mínimo

Abrir el análisis

Se marca «datos insuficientes»

CP\-10\.4

CA\-10\.4

Análisis mostrado

Observar el pie de la pantalla

Se indican fuente y fecha de sincronización

*Tabla 60\. Casos de prueba de HU\-10*

### <a id="_Toc245118826"></a><a id="_Toc245119180"></a><a id="_Toc245130141"></a>HU\-11 — Consultar al agente de IA

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-11\.1

CA\-11\.1

Curso autorizado con datos conocidos

Preguntar por el promedio del curso

La respuesta coincide con el panel y se registra la herramienta usada

CP\-11\.2

CA\-11\.2

Pregunta que requiere interpretación

Preguntar qué estudiantes presentan dificultades

Respuesta con hechos, inferencias y recomendaciones diferenciados

CP\-11\.3

CA\-11\.3

Pregunta sobre una actividad inexistente

Enviar la pregunta

El agente reconoce que no hay información y no inventa valores

CP\-11\.4

CA\-11\.4

Docente B sin asignación al curso

Preguntar por ese curso

La herramienta rechaza el acceso y el agente informa que no tiene acceso

CP\-11\.5

CA\-11\.5

Entorno de pruebas con monitoreo de red

Realizar una consulta

Ninguna petición con datos identificables sale a servicios externos

*Tabla 61\. Casos de prueba de HU\-11*

### <a id="_Toc245118827"></a><a id="_Toc245119181"></a><a id="_Toc245130142"></a>HU\-12 — Recibir recomendaciones pedagógicas justificadas

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-12\.1

CA\-12\.1

Estudiante con alertas vigentes

Solicitar recomendaciones

Acciones concretas indicando qué hacer y sobre qué actividad o tema

CP\-12\.2

CA\-12\.2

Recomendación generada

Abrir su detalle

Justificación con indicador, actividad y fecha

CP\-12\.3

CA\-12\.3

Recomendación en estado Propuesta

Aceptar una, ajustar otra y descartar otra

Los estados cambian y se conservan; se identifican como propuestas del sistema

CP\-12\.4

CA\-12\.4

Curso con datos insuficientes

Solicitar recomendaciones

No se generan y se explica el motivo

*Tabla 62\. Casos de prueba de HU\-12*

### <a id="_Toc245118828"></a><a id="_Toc245119182"></a><a id="_Toc245130143"></a>HU\-13 — Generar y descargar el informe del curso

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-13\.1

CA\-13\.1

Curso y período con datos

Generar el informe

Incluye período, métricas, hallazgos, recomendaciones y fecha de actualización

CP\-13\.2

CA\-13\.2

Informe generado

Consultarlo y descargarlo

Se visualiza y se descarga en el formato acordado

CP\-13\.3

CA\-13\.3

Curso con sincronización incompleta

Generar el informe

El informe indica que los datos están incompletos

CP\-13\.4

CA\-13\.4

Docente B sin asignación al curso

Solicitar el informe del curso

Respuesta 403

*Tabla 63\. Casos de prueba de HU\-13*

### <a id="_Toc245118829"></a><a id="_Toc245119183"></a><a id="_Toc245130144"></a>HU\-14 — Consultar el registro de auditoría

__Código CP__

__CA evaluado__

__Precondición y datos__

__Pasos__

__Resultado esperado__

CP\-14\.1

CA\-14\.1

Una sincronización, una consulta al agente y un uso de herramienta realizados

Consultar la auditoría

Tres registros con usuario, fecha y resultado

CP\-14\.2

CA\-14\.2

Registros existentes

Revisar el contenido y las respuestas de la API

No aparecen credenciales, tokens ni contraseñas

CP\-14\.3

CA\-14\.3

Administrador técnico con varios registros

Filtrar por tipo y rango de fechas

Resultados filtrados y paginados

CP\-14\.4

CA\-14\.4

Usuario con rol Docente

Intentar abrir la auditoría

Respuesta 403

*Tabla 64\. Casos de prueba de HU\-14*

__Total de casos de prueba funcional: 30__ \(uno por cada criterio de aceptación\)\.

## <a id="_Toc245118830"></a><a id="_Toc245119184"></a><a id="_Toc245130145"></a>11\.3 Matriz de trazabilidad

__RF__

__Historia__

__Criterios de aceptación__

__Casos de prueba__

__Mockups__

__Modelo y diseño__

RF\-06

HU\-08

CA\-08\.1 a CA\-08\.4

CP\-08\.1 a CP\-08\.4

M\-07

EstadoActividadClasificado, SeguimientoEstudianteDto

RF\-07

HU\-09

CA\-09\.1 a CA\-09\.5

CP\-09\.1 a CP\-09\.5

M\-08, M\-09

ReglaAlerta, Alerta, EvidenciaAlerta

RF\-08

HU\-10

CA\-10\.1 a CA\-10\.4

CP\-10\.1 a CP\-10\.4

M\-10

ResultadoActividadDto, PuntoTendenciaDto

RF\-09

HU\-11

CA\-11\.1 a CA\-11\.5

CP\-11\.1 a CP\-11\.5

M\-11

ConsultaAgente, RespuestaAgente, LlamadaHerramienta

RF\-10

HU\-12

CA\-12\.1 a CA\-12\.4

CP\-12\.1 a CP\-12\.4

M\-12

Recomendacion

RF\-11

HU\-13

CA\-13\.1 a CA\-13\.4

CP\-13\.1 a CP\-13\.4

M\-13

Informe

RF\-12

HU\-14

CA\-14\.1 a CA\-14\.4

CP\-14\.1 a CP\-14\.4

M\-14

RegistroAuditoria

__Total__

__30 criterios__

__30 casos__

__8 mockups__

*Tabla 65\. Matriz de trazabilidad \(RF, historia, criterios, casos, mockups y modelo\)*

# <a id="_Toc245118831"></a><a id="_Toc245119185"></a><a id="_Toc245130146"></a>12\. ESTIMACIÓN DE LAS HISTORIAS DE USUARIO

Los puntos de historia miden la complejidad y el esfuerzo relativo del trabajo de especificación y diseño \(1, 2, 3, 5 u 8\); no equivalen a horas exactas\. No se utiliza una velocidad histórica del equipo\.

__Código HU__

__Historia de Usuario__

__Complejidad__

__Puntos de historia__

__Justificación__

HU\-08

Consultar el seguimiento individual de un estudiante

Considerable

5

Reutiliza datos del Sprint 1; requiere definir la clasificación de estados de actividad y un mockup\.

HU\-09

Configurar reglas y consultar alertas académicas

Alta

8

Dos mockups, tres entidades nuevas y un motor de reglas con manejo de datos incompletos y redacción sin etiquetas definitivas\.

HU\-10

Consultar el análisis grupal del curso

Considerable

5

Agregaciones nuevas y parámetros configurables por acordar con el Product Owner\.

HU\-11

Consultar al agente de IA

Alta

8

Mayor incertidumbre: diseño del servicio de IA, herramientas controladas, privacidad y pruebas de un componente no determinista\.

HU\-12

Recibir recomendaciones pedagógicas justificadas

Considerable

5

Depende del agente y de las alertas; exige justificación con indicadores\.

HU\-13

Generar y descargar el informe del curso

Considerable

5

Estructura del informe y formato descargable por confirmar\.

HU\-14

Consultar el registro de auditoría

Moderada

3

Una entidad y una pantalla; la dificultad principal es depurar secretos y recolectar eventos\.

__Total__

__39__

*Tabla 66\. Estimación de las Historias de Usuario en puntos de historia*

# <a id="_Toc245118832"></a><a id="_Toc245119186"></a><a id="_Toc245130147"></a>13\. DESCOMPOSICIÓN EN TAREAS 

Resumen de las tareas del Sprint 2\. El detalle de cada una \(tipo, fechas y descripción\) está en las fichas de la sección 10\.2\.

__Código tarea__

__Código HU relacionada__

__Descripción de la tarea__

__Responsable__

__Colaborador__

__Estimación \(horas\)__

TA\-08\.1

HU\-08

Especificación de la historia y criterios de aceptación

Quispe

Redroban

1,0

TA\-08\.2

HU\-08

Mockup M\-07 Seguimiento individual

Redroban

Quispe

1,5

TA\-08\.3

HU\-08

Modelo y diseño técnico del seguimiento individual

Manobanda

Curillo

1,5

TA\-08\.4

HU\-08

Casos de prueba funcional de HU\-08

Quispe

Manobanda

1,0

TA\-09\.1

HU\-09

Especificación de la historia y criterios de aceptación

Quispe

Curillo

1,5

TA\-09\.2

HU\-09

Mockups M\-08 y M\-09 \(reglas y panel de alertas\)

Redroban

Quispe

2,5

TA\-09\.3

HU\-09

Modelo de datos y diseño del motor de reglas

Manobanda

Curillo

2,5

TA\-09\.4

HU\-09

Casos de prueba funcional de HU\-09

Quispe

Redroban

1,0

TA\-10\.1

HU\-10

Especificación de la historia y criterios de aceptación

Curillo

Quispe

1,0

TA\-10\.2

HU\-10

Mockup M\-10 Análisis grupal

Redroban

Manobanda

1,5

TA\-10\.3

HU\-10

Modelo y diseño técnico del análisis grupal

Curillo

Manobanda

1,5

TA\-10\.4

HU\-10

Casos de prueba funcional de HU\-10

Quispe

Redroban

1,0

TA\-11\.1

HU\-11

Especificación de la historia y criterios de aceptación

Curillo

Quispe

1,5

TA\-11\.2

HU\-11

Mockup M\-11 Consulta al agente

Redroban

Curillo

2,0

TA\-11\.3

HU\-11

Diseño de la arquitectura de los servicios de IA

Curillo

Manobanda

3,5

TA\-11\.4

HU\-11

Modelo de datos y diagrama de secuencia del agente

Manobanda

Curillo

2,0

TA\-11\.5

HU\-11

Casos de prueba funcional de HU\-11

Quispe

Curillo

1,5

TA\-12\.1

HU\-12

Especificación de la historia y criterios de aceptación

Quispe

Redroban

1,0

TA\-12\.2

HU\-12

Mockup M\-12 Recomendaciones

Redroban

Quispe

1,5

TA\-12\.3

HU\-12

Modelo y diseño técnico de recomendaciones

Curillo

Manobanda

1,5

TA\-12\.4

HU\-12

Casos de prueba funcional de HU\-12

Quispe

Curillo

1,0

TA\-13\.1

HU\-13

Especificación de la historia y criterios de aceptación

Redroban

Quispe

1,0

TA\-13\.2

HU\-13

Mockup M\-13 Informe

Redroban

Quispe

1,5

TA\-13\.3

HU\-13

Modelo y diseño técnico del informe

Manobanda

Curillo

1,5

TA\-13\.4

HU\-13

Casos de prueba funcional de HU\-13

Quispe

Manobanda

1,0

TA\-14\.1

HU\-14

Especificación de la historia y criterios de aceptación

Curillo

Quispe

0,5

TA\-14\.2

HU\-14

Mockup M\-14 Consulta de auditoría

Redroban

Curillo

1,0

TA\-14\.3

HU\-14

Modelo y diseño técnico de auditoría

Manobanda

Curillo

1,5

TA\-14\.4

HU\-14

Casos de prueba funcional de HU\-14

Quispe

Curillo

1,0

TE\-01

Habilitadora

Modelo de datos y arquitectura ampliados \(consolidación\)

Manobanda

Curillo

2,5

TE\-02

Habilitadora

Matriz de trazabilidad y estrategia de pruebas funcionales

Quispe

Manobanda

2,0

TE\-03

Habilitadora

Consolidación y revisión del documento de planificación

Curillo

Redroban

2,0

TE\-04

Habilitadora

Preparación y ensayo de la presentación del Sprint 2

Curillo

Redroban

1,5

__Total: 33 tareas__

__50,0__

*Tabla 67\. Descomposición consolidada en tareas*

# <a id="_Toc245118833"></a><a id="_Toc245119187"></a><a id="_Toc245130148"></a>14\. SPRINT BACKLOG

## <a id="_Toc245118834"></a><a id="_Toc245119188"></a><a id="_Toc245130149"></a>14\.1 Tablero Scrum \(estado inicial\)

Todas las tareas están en «Tareas No Iniciadas»; las otras dos columnas se actualizan durante el Sprint\.

__Ítems del Backlog comprometidos__

__Tareas No Iniciadas__

__Tareas En Progreso__

__Tareas Completadas__

__HU\-08__ Consultar el seguimiento individual de un estudiante \(5 SP\)

TA\-08\.1 \(Quispe\)

TA\-08\.2 \(Redroban\)

TA\-08\.3 \(Manobanda\)

TA\-08\.4 \(Quispe\)

__HU\-09__ Configurar reglas y consultar alertas académicas \(8 SP\)

TA\-09\.1 \(Quispe\)

TA\-09\.2 \(Redroban\)

TA\-09\.3 \(Manobanda\)

TA\-09\.4 \(Quispe\)

__HU\-10__ Consultar el análisis grupal del curso \(5 SP\)

TA\-10\.1 \(Curillo\)

TA\-10\.2 \(Redroban\)

TA\-10\.3 \(Curillo\)

TA\-10\.4 \(Quispe\)

__HU\-11__ Consultar al agente de IA \(8 SP\)

TA\-11\.1 \(Curillo\)

TA\-11\.2 \(Redroban\)

TA\-11\.3 \(Curillo\)

TA\-11\.4 \(Manobanda\)

TA\-11\.5 \(Quispe\)

__HU\-12__ Recibir recomendaciones pedagógicas justificadas \(5 SP\)

TA\-12\.1 \(Quispe\)

TA\-12\.2 \(Redroban\)

TA\-12\.3 \(Curillo\)

TA\-12\.4 \(Quispe\)

__HU\-13__ Generar y descargar el informe del curso \(5 SP\)

TA\-13\.1 \(Redroban\)

TA\-13\.2 \(Redroban\)

TA\-13\.3 \(Manobanda\)

TA\-13\.4 \(Quispe\)

__HU\-14__ Consultar el registro de auditoría \(3 SP\)

TA\-14\.1 \(Curillo\)

TA\-14\.2 \(Redroban\)

TA\-14\.3 \(Manobanda\)

TA\-14\.4 \(Quispe\)

__Tareas habilitadoras__

TE\-01 \(Manobanda\)

TE\-02 \(Quispe\)

TE\-03 \(Curillo\)

TE\-04 \(Curillo\)

*Tabla 68\. Tablero Scrum \(estado inicial\)*

## <a id="_Toc245118835"></a><a id="_Toc245119189"></a><a id="_Toc245130150"></a>14\.2 Resumen del Sprint Backlog

__ID__

__Historia de Usuario__

__Responsable principal__

__Participan en tareas__

__SP__

__Horas__

__Prioridad__

HU\-08

Consultar el seguimiento individual de un estudiante

Quispe

Redroban, Manobanda

5

5,0 h

Alta

HU\-09

Configurar reglas y consultar alertas académicas

Redroban

Quispe, Manobanda

8

7,5 h

Alta

HU\-10

Consultar el análisis grupal del curso

Curillo

Redroban, Quispe

5

5,0 h

Media

HU\-11

Consultar al agente de IA

Curillo

Redroban, Manobanda, Quispe

8

10,5 h

Alta

HU\-12

Recibir recomendaciones pedagógicas justificadas

Quispe

Redroban, Curillo

5

5,0 h

Alta

HU\-13

Generar y descargar el informe del curso

Redroban

Manobanda, Quispe

5

5,0 h

Media

HU\-14

Consultar el registro de auditoría

Manobanda

Curillo, Redroban, Quispe

3

4,0 h

Media

TE

Tareas habilitadoras

Los 4 integrantes

–

8,0 h

Alta

__Total__

__7 historias · 33 tareas__

__39__

__50,0 h__

*Tabla 69\. Resumen del Sprint Backlog*

# <a id="_Toc245118836"></a><a id="_Toc245119190"></a><a id="_Toc245130151"></a>15\. DISTRIBUCIÓN DE RESPONSABILIDADES

__Integrante__

__Responsabilidades principales__

__Historias de Usuario relacionadas__

__Tareas asignadas__

Curillo Pilamunga Diego Alexander

Diseño técnico y arquitectura ampliada, diseño del servicio de IA, análisis grupal, coordinación de la consolidación y de la presentación\.

HU\-10, HU\-11, HU\-12, HU\-14

TA\-10\.1, TA\-10\.3, TA\-11\.1, TA\-11\.3, TA\-12\.3, TA\-14\.1, TE\-03, TE\-04 \(8 tareas · 13,0 h\)

Manobanda Masabanda Luis Alfredo

Modelos de datos, diagramas de clases y de secuencia, modelo ampliado consolidado\.

HU\-08, HU\-09, HU\-11, HU\-13, HU\-14

TA\-08\.3, TA\-09\.3, TA\-11\.4, TA\-13\.3, TA\-14\.3, TE\-01 \(6 tareas · 11,5 h\)

Redroban Chimbo Joselyn Amparo

Mockups M\-07 a M\-14 y especificación de la historia de informes\.

HU\-08, HU\-09, HU\-10, HU\-11, HU\-12, HU\-13, HU\-14

TA\-08\.2, TA\-09\.2, TA\-10\.2, TA\-11\.2, TA\-12\.2, TA\-13\.1, TA\-13\.2, TA\-14\.2 \(8 tareas · 12,5 h\)

Quispe Nasimba Daniela Alejandra

Especificación de historias y criterios de aceptación, casos de prueba funcional y matriz de trazabilidad\.

HU\-08, HU\-09, HU\-10, HU\-11, HU\-12, HU\-13, HU\-14

TA\-08\.1, TA\-08\.4, TA\-09\.1, TA\-09\.4, TA\-10\.4, TA\-11\.5, TA\-12\.1, TA\-12\.4, TA\-13\.4, TA\-14\.4, TE\-02 \(11 tareas · 13,0 h\)

__Total__

__33 tareas · 50,0 h__

*Tabla 70\. Distribución de responsabilidades del equipo*

El Product Owner no figura como responsable de tareas; su participación consiste en validar el alcance, los criterios y los entregables\.

# <a id="_Toc245118837"></a><a id="_Toc245119191"></a><a id="_Toc245130152"></a>16\. PLAN DE TRABAJO DEL SPRINT 2

## <a id="_Toc245118838"></a><a id="_Toc245119192"></a><a id="_Toc245130153"></a>16\.1 Orden de elaboración y dependencias

1. __Especificación primero \(05/11\):__ las tareas de especificación \(\.1\) de cada historia desbloquean mockups, modelos y casos de prueba\.
2. __Diseño en paralelo \(05/11 al 09/11\):__ mockups, modelos y diseños técnicos se elaboran en paralelo; el diseño del servicio de IA \(TA\-11\.3\) inicia el primer día por ser el de mayor riesgo\.
3. __Dependencias funcionales:__ HU\-08 y HU\-10 alimentan las herramientas del agente \(HU\-11\); HU\-09 y HU\-11 alimentan las recomendaciones \(HU\-12\); HU\-12 y HU\-10 alimentan el informe \(HU\-13\); HU\-09, HU\-11 y la sincronización alimentan la auditoría \(HU\-14\)\.
4. __Casos de prueba después de los mockups:__ cada caso \(\.4\) se redacta cuando existe la especificación y el mockup correspondiente\.
5. __Consolidación \(09/11 y 10/11\):__ TE\-01 integra modelos y arquitectura, TE\-02 elabora la matriz de trazabilidad y TE\-03 unifica y revisa el documento para su entrega el 10/11/2026\.
6. __Presentación \(11/11 y 12/11\):__ TE\-04 prepara y ensaya la presentación; el 12/11/2026 se presenta el Sprint 2\.

## <a id="_Toc245118839"></a><a id="_Toc245119193"></a><a id="_Toc245130154"></a>16\.2 Cronograma diario por integrante

Ningún integrante supera las 4 horas diarias de tareas\. La reunión diaria de 15 minutos se cubre con la reserva\.

__Día__

__Curillo__

__Manobanda__

__Redroban__

__Quispe__

Mié 04/11

Inicio del Sprint 2 y reunión de planificación

Inicio del Sprint 2 y reunión de planificación

Inicio del Sprint 2 y reunión de planificación

Inicio del Sprint 2 y reunión de planificación

Jue 05/11

TA\-11\.1 \(1,5 h\)

TA\-10\.1 \(1 h\)

TA\-14\.1 \(0,5 h\)

TA\-11\.3 \(1 h\)

TA\-08\.3 \(1,5 h\)

TA\-09\.3 \(2 h\)

TA\-13\.1 \(1 h\)

TA\-08\.2 \(1,5 h\)

TA\-09\.2 \(1,5 h\)

TA\-08\.1 \(1 h\)

TA\-09\.1 \(1,5 h\)

TA\-12\.1 \(1 h\)

Vie 06/11

TA\-11\.3 \(2,5 h\)

TA\-10\.3 \(1,5 h\)

TA\-09\.3 \(0,5 h\)

TA\-11\.4 \(2 h\)

TA\-13\.3 \(1,5 h\)

TA\-09\.2 \(1 h\)

TA\-10\.2 \(1,5 h\)

TA\-11\.2 \(1,5 h\)

TA\-08\.4 \(1 h\)

TA\-10\.4 \(1 h\)

TA\-11\.5 \(1,5 h\)

Lun 09/11

TA\-12\.3 \(1,5 h\)

TA\-14\.3 \(1,5 h\)

TE\-01 \(1,5 h\)

TA\-11\.2 \(0,5 h\)

TA\-12\.2 \(1,5 h\)

TA\-13\.2 \(1,5 h\)

TA\-14\.2 \(0,5 h\)

TA\-09\.4 \(1 h\)

TA\-12\.4 \(1 h\)

TA\-13\.4 \(1 h\)

Mar 10/11

TE\-03 \(2 h\) — __entrega de la planificación__

TE\-01 \(1 h\)

TA\-14\.2 \(0,5 h\)

TA\-14\.4 \(1 h\)

TE\-02 \(2 h\)

Mié 11/11

TE\-04 \(1,5 h\) y ensayo de la presentación

Ajustes finales y ensayo

Ajustes finales y ensayo

Ajustes finales y ensayo

Jue 12/11

__Presentación del Sprint 2__

__Presentación del Sprint 2__

__Presentación del Sprint 2__

__Presentación del Sprint 2__

*Tabla 71\. Cronograma diario por integrante*

__Verificación de horas__

__Curillo__

__Manobanda__

__Redroban__

__Quispe__

Horas de tareas en el cronograma

13 h

11,5 h

12,5 h

13 h

Coincide con la tabla de capacidad

Sí

Sí

Sí

Sí

*Tabla 72\. Verificación de horas por integrante*

## <a id="_Toc245118840"></a><a id="_Toc245119194"></a><a id="_Toc245130155"></a>16\.3 Riesgos e impedimentos

Son riesgos potenciales identificados durante la planificación; ninguno se presenta como ocurrido\.

__N\.º__

__Riesgo o impedimento__

__Posible impacto__

__Probabilidad__

__Medida de mitigación__

1

Plazo corto: 4 días de trabajo hasta la entrega del 10/11\.

No terminar algún entregable a tiempo\.

Alta

Trabajar en paralelo, priorizar la especificación el primer día, usar la holgura de 22 h y dejar el 11/11 para ajustes\.

2

Definiciones pendientes del Product Owner \(formato del informe, umbrales iniciales, alcance del agente\)\.

Rehacer criterios, mockups o pruebas\.

Media

Plantear las preguntas en la reunión del 04/11 y registrar los supuestos en cada ficha\.

3

Complejidad del diseño del agente de IA \(herramientas controladas, privacidad y pruebas\)\.

Diseño incompleto o ambiguo del componente más riesgoso\.

Media

Iniciar TA\-11\.3 el 05/11, limitar las herramientas a solo lectura y revisar el diseño con el Product Owner\.

4

Inconsistencias entre historias, mockups, modelos y pruebas\.

Retrabajo y criterios no verificables\.

Media

Usar la matriz de trazabilidad \(TE\-02\) y revisar la coherencia en TE\-03\.

5

Resultados del Sprint 1 aún no consolidados \(insumo de los modelos\)\.

Modelos desalineados con lo implementado\.

Media

Basar los diseños en el modelo previsto del Sprint 1 y ajustar al cierre\.

6

Datos reales de Moodle insuficientes para definir umbrales y casos de prueba\.

Casos de prueba poco representativos\.

Media

Definir casos con datos anonimizados de ejemplo y marcar los umbrales como parámetros configurables\.

7

Coordinación del equipo y revisión del Product Owner a tiempo\.

Entrega sin validación\.

Media

Reunión diaria de 15 minutos, tablero actualizado y revisión del PO el 10/11\.

8

Ambigüedad en las fechas \(inicio 03/11 o 04/11\)\.

Error en la capacidad calculada\.

Baja

Confirmar con el Product Owner; el plan usa 04/11 y tiene holgura suficiente\.

*Tabla 73\. Riesgos e impedimentos del Sprint 2*

## <a id="_Toc245118841"></a><a id="_Toc245119195"></a><a id="_Toc245130156"></a>16\.4 Criterios de finalización del Sprint 2

- Las siete Historias de Usuario tienen ficha, criterios de aceptación verificables, mockup, modelo, diseño técnico y casos de prueba\.
- La matriz de trazabilidad no deja criterios sin caso de prueba \(30 criterios y 30 casos\)\.
- El modelo de datos ampliado y la arquitectura ampliada son coherentes con el modelo del Sprint 1 y con las historias\.
- El Product Owner recibe la planificación el 10/11/2026 y valida el paquete\.
- El equipo presenta el Sprint 2 el 12/11/2026\.
- No se incluye código de producción en el paquete\.
- Las tareas están cerradas en el tablero Scrum\.

# <a id="_Toc245118843"></a><a id="_Toc245119197"></a><a id="_Toc245130158"></a>17\. COMPROMISO DEL EQUIPO

El equipo de desarrollo, conformado por Curillo Pilamunga Diego Alexander, Manobanda Masabanda Luis Alfredo, Redroban Chimbo Joselyn Amparo y Quispe Nasimba Daniela Alejandra, se compromete a completar las siete Historias de Usuario del Sprint 2 \(39 puntos de historia y 50 horas de trabajo estimado\), a entregar la planificación el 10/11/2026 y a presentar el Sprint el 12/11/2026\.

Este compromiso:

- Fue definido conjuntamente con el Product Owner, Ing\. Hernán Naranjo, Mg\., en la reunión de planificación del 04/11/2026\.
- Se basa en la capacidad del equipo \(72 horas netas disponibles frente a 50 horas planificadas\) y en el tiempo disponible\.
- Implica que el equipo asume la responsabilidad de organizar y ejecutar las tareas del Sprint Backlog\.
- Establece que el alcance se mantiene durante el Sprint; cualquier cambio se gestiona en colaboración con el Product Owner\.

__Integrante__

__Compromiso__

Curillo Pilamunga Diego Alexander

☐ Aceptado

Manobanda Masabanda Luis Alfredo

☐ Aceptado

Redroban Chimbo Joselyn Amparo

☐ Aceptado

Quispe Nasimba Daniela Alejandra

☐ Aceptado

Ing\. Hernán Naranjo, Mg\. \(Product Owner\)

☐ Conforme con el alcance

*Tabla 74\. Compromiso del equipo*

# <a id="_Toc245118844"></a><a id="_Toc245119198"></a><a id="_Toc245130159"></a>18\. CONCLUSIONES

1. __Un Sprint de diseño reduce el riesgo\.__ Especificar y diseñar RF\-06 a RF\-12 antes de construirlos permite resolver ambigüedades, sobre todo en el agente de IA, que es el componente de mayor incertidumbre\.
2. __Trazabilidad completa\.__ Cada criterio de aceptación tiene un caso de prueba funcional y está ligado a una historia, un mockup y un modelo, lo que facilita validar el trabajo y planificar su construcción\.
3. __Privacidad y control de acceso desde el diseño\.__ Las reglas RN\-18 y RN\-19 y las herramientas de solo lectura del agente garantizan, desde el diseño, el cumplimiento de RNF\-01 y RNF\-02\.
4. __Distribución equilibrada\.__ Las 33 tareas se reparten entre los cuatro integrantes \(11,5 a 13 horas cada uno\), según sus responsabilidades técnicas, sin asignar tareas al Product Owner\.
5. __Planificación realista\.__ Con 50 horas planificadas sobre 72 horas netas y un cronograma que culmina el 10/11 con la entrega y el 12/11 con la presentación, el equipo cuenta con holgura para imprevistos\.
6. __Valor del incremento\.__ Al finalizar el Sprint, el proyecto dispone de un paquete de especificación y diseño validado, listo para ser construido en los siguientes incrementos\.

