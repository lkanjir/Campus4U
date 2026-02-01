using Client.Domain.Fault;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Client.Presentation.Services
{
    public static class FaultStatisticsPdfExporter
    {
        private static readonly string PrimaryColor = "#1e3a5f";
        private static readonly string MutedColor = "#64748b";

        static FaultStatisticsPdfExporter()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static void Export(FaultStatistics stats, string filePath, byte[]? logoBytes = null)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => ComposeHeader(c, logoBytes));
                    page.Content().Element(c => ComposeContent(c, stats));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf(filePath);
        }

        public static byte[]? LoadLogoFromResources()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Images/Logo/Emblem.png");
                var streamInfo = System.Windows.Application.GetResourceStream(uri);
                if (streamInfo == null) return null;

                using var ms = new MemoryStream();
                streamInfo.Stream.CopyTo(ms);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private static void ComposeHeader(IContainer container, byte[]? logoBytes)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    if (logoBytes != null)
                    {
                        row.ConstantItem(40).Height(40).Image(logoBytes);
                        row.ConstantItem(10);
                    }

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Campus4U").FontSize(20).Bold().FontColor(PrimaryColor);
                        col.Item().Text("Statistika kvarova").FontSize(14).FontColor(MutedColor);
                    });
                });

                column.Item().PaddingTop(8).Text($"Generirano: {DateTime.Now:d.M.yyyy. HH:mm}").FontSize(9).FontColor(MutedColor);
                column.Item().PaddingTop(12).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            });
        }

        private static void ComposeContent(IContainer container, FaultStatistics stats)
        {
            container.PaddingTop(20).Column(column =>
            {
                column.Item().Element(c => ComposeOverview(c, stats));
                column.Item().PaddingTop(30).Element(c => ComposeCategoryTable(c, "Po vrsti kvara", stats.PoVrstiKvara));
                column.Item().PaddingTop(25).Element(c => ComposeCategoryTable(c, "Po prostoru", stats.PoProstoru));
                column.Item().PaddingTop(25).Element(c => ComposeMonthlyTable(c, stats.PoMjesecu));
            });
        }

        private static void ComposeOverview(IContainer container, FaultStatistics stats)
        {
            container.Column(column =>
            {
                column.Item().Text("Pregled").FontSize(14).Bold().FontColor(PrimaryColor);
                column.Item().PaddingTop(12);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeStatBox(c, "Ukupno", stats.UkupnoKvarova));
                    row.ConstantItem(20);
                    row.RelativeItem().Element(c => ComposeStatBox(c, "Aktivnih", stats.AktivnihKvarova));
                    row.ConstantItem(20);
                    row.RelativeItem().Element(c => ComposeStatBox(c, "U obradi", stats.UObradiKvarova));
                    row.ConstantItem(20);
                    row.RelativeItem().Element(c => ComposeStatBox(c, "Riješenih", stats.RijesenihKvarova));
                });
            });
        }

        private static void ComposeStatBox(IContainer container, string label, int value)
        {
            container.Border(0.5f).BorderColor(Colors.Grey.Lighten1)
                .Padding(12)
                .Column(column =>
                {
                    column.Item().AlignCenter().Text(value.ToString()).FontSize(24).Bold().FontColor(PrimaryColor);
                    column.Item().AlignCenter().PaddingTop(2).Text(label).FontSize(9).FontColor(MutedColor);
                });
        }

        private static void ComposeCategoryTable(IContainer container, string title, List<StatistikaPoKategoriji> data)
        {
            container.Column(column =>
            {
                column.Item().Text(title).FontSize(14).Bold().FontColor(PrimaryColor);
                column.Item().PaddingTop(8);

                if (data.Count == 0)
                {
                    column.Item().Text("Nema podataka").Italic().FontColor(MutedColor);
                    return;
                }

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().BorderBottom(1).BorderColor(PrimaryColor).PaddingBottom(6)
                            .Text("Naziv").Bold().FontColor(PrimaryColor);
                        header.Cell().BorderBottom(1).BorderColor(PrimaryColor).PaddingBottom(6)
                            .Text("Broj").Bold().FontColor(PrimaryColor).AlignCenter();
                        header.Cell().BorderBottom(1).BorderColor(PrimaryColor).PaddingBottom(6)
                            .Text("%").Bold().FontColor(PrimaryColor).AlignCenter();
                    });

                    foreach (var item in data)
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6)
                            .Text(item.Naziv);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6)
                            .Text(item.BrojKvarova.ToString()).AlignCenter();
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6)
                            .Text($"{item.Postotak:0.0}").AlignCenter();
                    }
                });
            });
        }

        private static void ComposeMonthlyTable(IContainer container, List<StatistikaPoMjesecu> data)
        {
            container.Column(column =>
            {
                column.Item().Text("Po mjesecu").FontSize(14).Bold().FontColor(PrimaryColor);
                column.Item().PaddingTop(8);

                if (data.Count == 0)
                {
                    column.Item().Text("Nema podataka").Italic().FontColor(MutedColor);
                    return;
                }

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
                        header.Cell().BorderBottom(1).BorderColor(PrimaryColor).PaddingBottom(6)
                            .Text("Mjesec").Bold().FontColor(PrimaryColor);
                        header.Cell().BorderBottom(1).BorderColor(PrimaryColor).PaddingBottom(6)
                            .Text("Godina").Bold().FontColor(PrimaryColor).AlignCenter();
                        header.Cell().BorderBottom(1).BorderColor(PrimaryColor).PaddingBottom(6)
                            .Text("Broj").Bold().FontColor(PrimaryColor).AlignCenter();
                    });

                    foreach (var item in data)
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6)
                            .Text(item.NazivMjeseca);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6)
                            .Text(item.Godina.ToString()).AlignCenter();
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6)
                            .Text(item.BrojKvarova.ToString()).AlignCenter();
                    }
                });
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text($"Campus4U © {DateTime.Now.Year}").FontSize(8).FontColor(MutedColor);
        }
    }
}
