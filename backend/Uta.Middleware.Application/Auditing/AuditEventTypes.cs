namespace Uta.Middleware.Application.Auditing;

public static class AuditEventTypes
{
    public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.Ordinal)
    {
        "Sincronizacion",
        "ConsultaAgente",
        "UsoHerramienta",
        "AnalisisGrupal",
        "RecomendacionGenerada",
        "EstadoRecomendacionActualizado",
        "ReglasAlertaActualizadas",
        "EstadoAlertaActualizado",
        "InformeGenerado",
        "InformeDescargado"
    };
}
