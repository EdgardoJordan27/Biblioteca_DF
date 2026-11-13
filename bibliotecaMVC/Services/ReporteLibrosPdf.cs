using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    // Documento PDF del reporte de libros, construido con QuestPDF.
    // La categoria del reporte corresponde al campo Genero del libro.
    public class ReporteLibrosPdf : IDocument
    {
        private static readonly Color Vino = Color.FromHex("#7B2D3F");
        private static readonly Color GrisOscuro = Color.FromHex("#3A3A3A");
        private static readonly Color GrisClaro = Color.FromHex("#F3F3F3");
        private static readonly Color GrisBorde = Color.FromHex("#C9C9C9");

        private readonly List<Libro> _libros;
        private readonly string? _autor;
        private readonly string? _categoria;
        private readonly DateTime _fechaGeneracion;

        public ReporteLibrosPdf(List<Libro> libros, string? autor, string? categoria, DateTime fechaGeneracion)
        {
            _libros = libros;
            _autor = autor;
            _categoria = categoria;
            _fechaGeneracion = fechaGeneracion;
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata
        {
            Title = "Reporte de Libros",
            Author = "BibliotecaMVC"
        };

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken4));

                page.Header().Element(ComponerEncabezado);
                page.Content().Element(ComponerContenido);
                page.Footer().Element(ComponerPie);
            });
        }

        private void ComponerEncabezado(IContainer container)
        {
            container
                .PaddingBottom(8)
                .BorderBottom(1.5f)
                .BorderColor(Vino)
                .Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("BibliotecaMVC").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken1);
                        col.Item().Text("Reporte de Libros").FontSize(20).Bold().FontColor(Vino);
                    });

                    row.ConstantItem(170).AlignRight().AlignBottom().Column(col =>
                    {
                        col.Item().AlignRight().Text("Fecha de generación").FontSize(8).FontColor(Colors.Grey.Darken1);
                        col.Item().AlignRight().Text(_fechaGeneracion.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    });
                });
        }

        private void ComponerContenido(IContainer container)
        {
            container.PaddingTop(12).Column(col =>
            {
                col.Spacing(12);
                col.Item().Element(ComponerResumen);

                if (_libros.Count == 0)
                {
                    col.Item().PaddingTop(20).AlignCenter()
                        .Text("No hay libros que coincidan con los filtros seleccionados.")
                        .Italic().FontColor(Colors.Grey.Darken1);
                    return;
                }

                col.Item().Element(ComponerTabla);
            });
        }

        private void ComponerResumen(IContainer container)
        {
            int disponibles = _libros.Count(l => l.Disponible);
            int noDisponibles = _libros.Count - disponibles;

            container
                .Background(GrisClaro)
                .Border(0.5f)
                .BorderColor(GrisBorde)
                .Padding(8)
                .Column(col =>
                {
                    col.Spacing(3);
                    col.Item().Text(t =>
                    {
                        t.Span("Autor: ").SemiBold();
                        t.Span(string.IsNullOrWhiteSpace(_autor) ? "Todos" : _autor);
                        t.Span("     Categoría: ").SemiBold();
                        t.Span(string.IsNullOrWhiteSpace(_categoria) ? "Todas" : _categoria);
                    });
                    col.Item().Text(t =>
                    {
                        t.Span("Total de libros: ").SemiBold();
                        t.Span(_libros.Count.ToString());
                        t.Span("     Disponibles: ").SemiBold();
                        t.Span(disponibles.ToString());
                        t.Span("     No disponibles: ").SemiBold();
                        t.Span(noDisponibles.ToString());
                    });
                });
        }

        private void ComponerTabla(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columnas =>
                {
                    columnas.ConstantColumn(34);
                    columnas.RelativeColumn(3.2f);
                    columnas.RelativeColumn(2.2f);
                    columnas.RelativeColumn(1.8f);
                    columnas.ConstantColumn(40);
                    columnas.ConstantColumn(68);
                });

                table.Header(header =>
                {
                    header.Cell().Element(EstiloEncabezado).AlignCenter().Text("ID");
                    header.Cell().Element(EstiloEncabezado).Text("Libro");
                    header.Cell().Element(EstiloEncabezado).Text("Autor");
                    header.Cell().Element(EstiloEncabezado).Text("Categoría");
                    header.Cell().Element(EstiloEncabezado).AlignCenter().Text("Año");
                    header.Cell().Element(EstiloEncabezado).AlignCenter().Text("Estado");
                });

                for (int i = 0; i < _libros.Count; i++)
                {
                    var libro = _libros[i];
                    bool filaPar = i % 2 == 1;

                    table.Cell().Element(c => EstiloCelda(c, filaPar)).AlignCenter().Text(libro.Id.ToString());
                    table.Cell().Element(c => EstiloCelda(c, filaPar)).Text(libro.Titulo);
                    table.Cell().Element(c => EstiloCelda(c, filaPar)).Text(libro.Autor);
                    table.Cell().Element(c => EstiloCelda(c, filaPar)).Text(libro.Genero);
                    table.Cell().Element(c => EstiloCelda(c, filaPar)).AlignCenter().Text(libro.AnioPublicacion.ToString());
                    table.Cell().Element(c => EstiloCelda(c, filaPar)).AlignCenter()
                        .Text(libro.Disponible ? "Disponible" : "No disponible")
                        .FontColor(libro.Disponible ? Colors.Grey.Darken4 : Vino);
                }
            });
        }

        private static IContainer EstiloEncabezado(IContainer container)
        {
            return container
                .Background(GrisOscuro)
                .Padding(5)
                .DefaultTextStyle(x => x.Bold().FontColor(Colors.White).FontSize(9));
        }

        private static IContainer EstiloCelda(IContainer container, bool filaPar)
        {
            return container
                .Background(filaPar ? GrisClaro : Colors.White)
                .BorderBottom(0.5f)
                .BorderColor(GrisBorde)
                .PaddingVertical(4)
                .PaddingHorizontal(5);
        }

        private void ComponerPie(IContainer container)
        {
            container
                .BorderTop(0.5f)
                .BorderColor(GrisBorde)
                .PaddingTop(5)
                .Row(row =>
                {
                    row.RelativeItem().Text("BibliotecaMVC - Reporte de Libros").FontSize(8).FontColor(Colors.Grey.Darken1);
                    row.RelativeItem().AlignRight().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                        t.Span("Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
        }
    }
}
