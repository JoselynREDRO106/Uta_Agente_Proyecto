import asyncio
import unittest
from unittest.mock import patch

from fastapi.testclient import TestClient
from pydantic import ValidationError

import app
from app import AskRequest, CourseIndicators, GroupAnalysis, QuestionIntent, create_agent_graph


def context() -> tuple[CourseIndicators, GroupAnalysis]:
    indicators = CourseIndicators.model_validate({
        "cursoId": 104,
        "estudiantes": 18,
        "promedio": 72.5,
        "notaMaxima": 100,
        "actividadesEvaluadas": 4,
        "actividadesTotales": 6,
        "progresoPorcentaje": 55.5,
        "ultimaSincronizacionUtc": "2026-11-11T12:00:00Z",
        "estadoSincronizacion": "Completa",
        "datosDisponibles": True,
        "distribucion": [{"rango": "70-84", "cantidad": 6}],
    })
    analysis = GroupAnalysis.model_validate({
        "cursoId": 104,
        "umbralBajo": 6,
        "minimoCalificaciones": 7,
        "fuente": "Moodle",
        "ultimaSincronizacionUtc": "2026-11-11T12:00:00Z",
        "actividades": [
            {
                "itemId": 7,
                "nombre": "Tarea A",
                "promedio": 5.5,
                "escala": 10,
                "numCalificaciones": 8,
                "bajoUmbral": True,
                "datosInsuficientes": False,
            },
            {
                "itemId": 8,
                "nombre": "Tarea B",
                "promedio": 4,
                "escala": 10,
                "numCalificaciones": 3,
                "bajoUmbral": False,
                "datosInsuficientes": True,
            },
        ],
        "tendencia": [
            {
                "fecha": "2026-11-01",
                "promedioCurso": 5.5,
                "escala": 10,
                "numCalificaciones": 8,
                "datosInsuficientes": False,
            }
        ],
    })
    return indicators, analysis


class AgentApiTests(unittest.TestCase):
    def run_graph(self, intent: str) -> app.AgentAnswer:
        indicators, analysis = context()

        async def fake_classify(question: str) -> QuestionIntent:
            return QuestionIntent(intent=intent)

        result = asyncio.run(
            create_agent_graph(fake_classify).ainvoke({
                "pregunta": "Consulta del docente",
                "indicadores": indicators,
                "analisis_grupal": analysis,
            })
        )
        return result["respuesta"]

    def test_request_rejects_empty_question_and_personal_data(self) -> None:
        indicators, analysis = context()
        payload = {
            "pregunta": "   ",
            "indicadores": indicators.model_dump(by_alias=True),
            "analisisGrupal": analysis.model_dump(by_alias=True),
        }
        with self.assertRaises(ValidationError):
            AskRequest.model_validate(payload)

        payload["pregunta"] = "¿Cuál es el promedio?"
        payload["nombresEstudiantes"] = ["Estudiante de prueba"]
        with self.assertRaises(ValidationError):
            AskRequest.model_validate(payload)

    def test_graph_uses_aggregates_and_reports_small_samples_as_insufficient(self) -> None:
        indicators, analysis = context()
        observed: dict[str, object] = {}

        async def fake_classify(question: str) -> QuestionIntent:
            observed["question"] = question
            return QuestionIntent(intent="low_activities")

        result = asyncio.run(
            create_agent_graph(fake_classify).ainvoke({
                "pregunta": "¿Qué actividades están bajas?",
                "indicadores": indicators,
                "analisis_grupal": analysis,
            })
        )

        answer = result["respuesta"]
        self.assertEqual(observed["question"], "¿Qué actividades están bajas?")
        self.assertIn("Tarea A (5.5/10)", answer.respuesta)
        self.assertNotIn("Tarea B", answer.respuesta)
        self.assertTrue(any("Tarea B: datos insuficientes" in fact for fact in answer.hechos))
        self.assertTrue(any("no evidencia de su causa" in item for item in answer.inferencias))

    def test_sync_status_intent_does_not_return_last_distribution_band(self) -> None:
        answer = self.run_graph("sync_status")

        self.assertEqual(answer.respuesta, "Estado de sincronización: Completa.")

    def test_agent_api_authenticates_and_returns_structured_answer(self) -> None:
        indicators, analysis = context()
        original_key = app.AGENT_API_KEY
        original_graph = app.create_agent_graph
        app.AGENT_API_KEY = "x" * 32

        async def fake_classify(question: str) -> QuestionIntent:
            return QuestionIntent(intent="average")

        app.create_agent_graph = lambda: create_agent_graph(fake_classify)
        payload = {
            "pregunta": "¿Cuál es el promedio?",
            "indicadores": indicators.model_dump(by_alias=True),
            "analisisGrupal": analysis.model_dump(by_alias=True),
        }
        try:
            with TestClient(app.app) as client:
                unauthorized = client.post("/api/agent/ask", json=payload)
                self.assertEqual(unauthorized.status_code, 401)

                response = client.post(
                    "/api/agent/ask",
                    json=payload,
                    headers={"X-Agent-Api-Key": "x" * 32},
                )
                self.assertEqual(response.status_code, 200)
                self.assertEqual(
                    set(response.json()),
                    {"respuesta", "hechos", "inferencias", "recomendaciones", "incertidumbre"},
                )
                self.assertIn("72.5", response.json()["respuesta"])
        finally:
            app.AGENT_API_KEY = original_key
            app.create_agent_graph = original_graph

    def test_ollama_url_is_limited_to_local_or_configured_docker_host(self) -> None:
        self.assertEqual(
            app.validate_ollama_base_url("http://localhost:11434/"),
            "http://localhost:11434",
        )
        self.assertEqual(
            app.validate_ollama_base_url("http://ollama:11434", "ollama"),
            "http://ollama:11434",
        )
        for value in ("https://example.com", "http://user:pass@localhost:11434", "http://example.com"):
            with self.assertRaises(ValueError):
                app.validate_ollama_base_url(value)

    def test_ollama_classifier_receives_only_the_question(self) -> None:
        expected = QuestionIntent(intent="average")

        class FakeResponse:
            def raise_for_status(self) -> None:
                pass

            def json(self) -> dict[str, object]:
                return {"message": {"content": expected.model_dump_json()}}

        class FakeClient:
            async def __aenter__(self) -> "FakeClient":
                return self

            async def __aexit__(self, *args: object) -> None:
                return None

            async def post(self, url: str, json: dict[str, object]) -> FakeResponse:
                self.url = url
                self.request = json
                return FakeResponse()

        client = FakeClient()
        with patch("app.httpx.AsyncClient", return_value=client):
            intent = asyncio.run(app.query_ollama("¿Cuál es el promedio?"))

        self.assertEqual(client.url, f"{app.OLLAMA_BASE_URL}/api/chat")
        self.assertEqual(client.request["messages"][1]["content"], "¿Cuál es el promedio?")
        self.assertEqual(intent.intent, "average")
        self.assertNotIn("indicadores", str(client.request))


if __name__ == "__main__":
    unittest.main()
