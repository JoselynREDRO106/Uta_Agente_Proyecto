# Servicio local del agente académico

El servicio FastAPI usa LangGraph para clasificar la pregunta y seleccionar herramientas de solo lectura sobre los indicadores y el análisis grupal recibidos desde la API .NET. Ollama clasifica únicamente la pregunta; la respuesta se construye con los datos validados y no entrega datos individuales al modelo.

## Ejecución local

Desde este directorio, instala las dependencias y configura variables de entorno fuera del repositorio:

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
python -m pip install -r requirements.txt
$env:AGENT_API_KEY = 'CLAVE_ALEATORIA_DE_AL_MENOS_32_BYTES'
$env:OLLAMA_BASE_URL = 'http://127.0.0.1:11434'
$env:OLLAMA_ALLOWED_HOST = 'ollama'
$env:OLLAMA_MODEL = 'llama3.2'
uvicorn app:app --host 127.0.0.1 --port 8010
```

Ollama debe estar iniciado y el modelo configurado debe existir localmente. El agente requiere una clave interna de al menos 32 bytes y valida `X-Agent-Api-Key`. `/health` comprueba disponibilidad del proceso; la disponibilidad del modelo se valida al realizar una consulta.

La API .NET se configura con `AcademicAgent__BaseUrl=http://localhost:8010` y la misma `AcademicAgent__ApiKey`. El servicio solo recibe agregados del curso, sin nombres, correos ni notas individuales.

## Pruebas

```powershell
python -m unittest discover -s tests -v
```
