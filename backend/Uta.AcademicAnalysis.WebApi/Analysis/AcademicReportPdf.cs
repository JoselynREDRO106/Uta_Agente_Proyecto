using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Uta.Middleware.Application.DTOs;

namespace Uta.AcademicAnalysis.WebApi.Analysis;

public static class AcademicReportPdf
{
    private static readonly CultureInfo EcuadorCulture = CultureInfo.GetCultureInfo("es-EC");
    private static readonly TimeZoneInfo EcuadorTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");

    public static byte[] Render(InformeDto report)
    {
        var content = report.Contenido;
        var indicators = content.Indicadores;

        return Document.Create(document =>
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(style => style.FontSize(10).FontColor(Colors.Grey.Darken3));
                page.Header().Column(header =>
                {
                    header.Item().Text("EduMonitor AI").FontSize(9).SemiBold().FontColor(Colors.Blue.Medium);
                    header.Item().PaddingTop(4).Text($"Informe académico: {content.Curso}")
                        .FontSize(19).Bold().FontColor(Colors.Grey.Darken4);
                    header.Item().PaddingTop(5).Text(
                        $"Período: {content.Periodo}  |  Generado: {LocalDate(report.FechaGeneracionUtc)}  |  Datos: {LocalDate(report.FechaActualizacionDatosUtc)}");
                });
                page.Content().PaddingVertical(18).Column(column =>
                {
                    column.Spacing(14);

                    if (content.Aviso is not null)
                    {
                        column.Item().Background(Colors.Orange.Lighten4).Padding(9)
                            .Text(content.Aviso).FontColor(Colors.Orange.Darken4);
                    }

                    column.Item().Element(SectionHeading).Text("Métricas");
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });
                        AddMetric(table, "Estudiantes", indicators.Estudiantes.ToString(EcuadorCulture));
                        AddMetric(table, "Promedio", Format(indicators.Promedio));
                        AddMetric(
                            table,
                            "Actividades evaluadas",
                            $"{indicators.ActividadesEvaluadas} de {indicators.ActividadesTotales}");
                    });

                    column.Item().Element(SectionHeading).Text("Hallazgos");
                    if (content.Hallazgos.Count == 0)
                    {
                        column.Item().Text("Sin hallazgos registrados.");
                    }
                    else
                    {
                        foreach (var finding in content.Hallazgos)
                        {
                            column.Item().Row(row =>
                            {
                                row.ConstantItem(14).Text("•").FontColor(Colors.Blue.Medium);
                                row.RelativeItem().Text(finding);
                            });
                        }
                    }

                    column.Item().Element(SectionHeading).Text("Actividades bajo el umbral");
                    if (content.ActividadesBajoUmbral.Count == 0)
                    {
                        column.Item().Text("Ninguna.");
                    }
                    else
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(header =>
                            {
                                AddHeaderCell(header, "Actividad");
                                AddHeaderCell(header, "Promedio / 10");
                                AddHeaderCell(header, "Calificaciones");
                            });
                            foreach (var activity in content.ActividadesBajoUmbral)
                            {
                                AddCell(table, activity.Nombre);
                                AddCell(table, Format(activity.Promedio));
                                AddCell(table, activity.NumCalificaciones.ToString(EcuadorCulture));
                            }
                        });
                    }

                    column.Item().Element(SectionHeading).Text("Recomendaciones");
                    if (content.Recomendaciones.Count == 0)
                    {
                        column.Item().Text("Sin recomendaciones registradas.");
                    }
                    else
                    {
                        foreach (var recommendation in content.Recomendaciones)
                        {
                            column.Item().Row(row =>
                            {
                                row.ConstantItem(14).Text("•").FontColor(Colors.Blue.Medium);
                                row.RelativeItem().Text(recommendation);
                            });
                        }
                    }
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            })).GeneratePdf();
    }

    private static IContainer SectionHeading(IContainer container) =>
        container.PaddingTop(4).BorderBottom(1).BorderColor(Colors.Blue.Lighten2).PaddingBottom(4);

    private static IContainer TableCell(IContainer container) =>
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).PaddingHorizontal(5);

    private static void AddMetric(TableDescriptor table, string name, string value)
    {
        AddCell(table, name);
        AddCell(table, value);
    }

    private static void AddCell(TableDescriptor table, string value) =>
        table.Cell().Element(TableCell).Text(value);

    private static void AddHeaderCell(TableCellDescriptor header, string value) =>
        header.Cell().Element(TableCell).Text(value).SemiBold().FontColor(Colors.Grey.Darken3);

    private static string Format(decimal? value) =>
        value?.ToString("0.00", EcuadorCulture) ?? "Sin datos";

    private static string LocalDate(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(utc, EcuadorTimeZone).ToString("yyyy-MM-dd HH:mm", EcuadorCulture);
}
