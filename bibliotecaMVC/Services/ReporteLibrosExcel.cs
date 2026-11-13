using ClosedXML.Excel;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    // Genera el libro de Excel (.xlsx) del reporte de libros con ClosedXML.
    // La categoria del reporte corresponde al campo Genero del libro.
    public static class ReporteLibrosExcel
    {
        private const int TotalColumnas = 6;
        private const int FilaEncabezado = 7;

        public static byte[] Generar(List<Libro> libros, string? autor, string? categoria, DateTime fechaGeneracion)
        {
            using var libroExcel = new XLWorkbook();
            var hoja = libroExcel.Worksheets.Add("Reporte de Libros");

            int disponibles = libros.Count(l => l.Disponible);

            hoja.Cell(1, 1).Value = "Reporte de Libros";
            hoja.Range(1, 1, 1, TotalColumnas).Merge();
            hoja.Cell(1, 1).Style.Font.FontSize = 16;
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#7B2D3F");

            hoja.Cell(2, 1).Value = "Sistema: BibliotecaMVC";
            hoja.Cell(3, 1).Value = "Fecha de generación: " + fechaGeneracion.ToString("dd/MM/yyyy HH:mm");
            hoja.Cell(4, 1).Value = "Autor: " + (string.IsNullOrWhiteSpace(autor) ? "Todos" : autor)
                + "   Categoría: " + (string.IsNullOrWhiteSpace(categoria) ? "Todas" : categoria);
            hoja.Cell(5, 1).Value = "Total de libros: " + libros.Count
                + "   Disponibles: " + disponibles
                + "   No disponibles: " + (libros.Count - disponibles);
            hoja.Range(2, 1, 5, 1).Style.Font.FontColor = XLColor.FromHtml("#3A3A3A");
            hoja.Cell(5, 1).Style.Font.Bold = true;

            string[] columnas = { "ID", "Libro", "Autor", "Categoría", "Año", "Estado" };
            for (int c = 0; c < columnas.Length; c++)
            {
                hoja.Cell(FilaEncabezado, c + 1).Value = columnas[c];
            }

            int fila = FilaEncabezado + 1;
            foreach (var libro in libros)
            {
                hoja.Cell(fila, 1).Value = libro.Id;
                hoja.Cell(fila, 2).Value = libro.Titulo;
                hoja.Cell(fila, 3).Value = libro.Autor;
                hoja.Cell(fila, 4).Value = libro.Genero;
                hoja.Cell(fila, 5).Value = libro.AnioPublicacion;
                hoja.Cell(fila, 6).Value = libro.Disponible ? "Disponible" : "No disponible";

                // Las filas pares llevan un gris suave para facilitar la lectura
                if ((fila - FilaEncabezado) % 2 == 0)
                {
                    hoja.Range(fila, 1, fila, TotalColumnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F3F3");
                }
                fila++;
            }

            int ultimaFila = fila - 1;
            var tabla = hoja.Range(FilaEncabezado, 1, ultimaFila, TotalColumnas);
            tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tabla.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
            tabla.Style.Border.OutsideBorderColor = XLColor.FromHtml("#9A9A9A");
            tabla.Style.Border.InsideBorderColor = XLColor.FromHtml("#C9C9C9");

            var encabezado = hoja.Range(FilaEncabezado, 1, FilaEncabezado, TotalColumnas);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#3A3A3A");
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            encabezado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            hoja.Range(FilaEncabezado, 1, ultimaFila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Range(FilaEncabezado, 5, ultimaFila, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            hoja.SheetView.FreezeRows(FilaEncabezado);
            hoja.Columns(1, TotalColumnas).AdjustToContents(FilaEncabezado, ultimaFila);

            using var memoria = new MemoryStream();
            libroExcel.SaveAs(memoria);
            return memoria.ToArray();
        }
    }
}
