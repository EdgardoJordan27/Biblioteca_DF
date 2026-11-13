using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using bibliotecaMVC.Data;
using bibliotecaMVC.Models;
using bibliotecaMVC.Services;

namespace bibliotecaMVC.Controllers
{
    // Reportes del modulo Libros. Igual que el catalogo, requiere haber iniciado sesion.
    [Authorize]
    public class ReportesController : Controller
    {
        private const string TipoExcel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly BibliotecaContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(BibliotecaContext context, ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Reportes
        public async Task<IActionResult> Index(string? autor, string? categoria)
        {
            var modelo = new ReportesViewModel
            {
                Autor = autor,
                Categoria = categoria,
                Autores = await _context.Libros.AsNoTracking()
                    .Select(l => l.Autor).Distinct().OrderBy(a => a).ToListAsync(),
                Categorias = await _context.Libros.AsNoTracking()
                    .Select(l => l.Genero).Distinct().OrderBy(g => g).ToListAsync(),
                Libros = await ConsultarLibrosAsync(autor, categoria)
            };

            return View(modelo);
        }

        // GET: Reportes/GenerarPdf
        public async Task<IActionResult> GenerarPdf(string? autor, string? categoria)
        {
            var libros = await ConsultarLibrosAsync(autor, categoria);
            var documento = new ReporteLibrosPdf(libros, autor, categoria, DateTime.Now);
            byte[] contenido = documento.GeneratePdf();

            _logger.LogInformation("Reporte PDF generado por {Usuario} con {Total} libros", User.Identity?.Name, libros.Count);
            return File(contenido, "application/pdf", "ReporteLibros.pdf");
        }

        // GET: Reportes/ExportarExcel
        public async Task<IActionResult> ExportarExcel(string? autor, string? categoria)
        {
            var libros = await ConsultarLibrosAsync(autor, categoria);
            byte[] contenido = ReporteLibrosExcel.Generar(libros, autor, categoria, DateTime.Now);

            _logger.LogInformation("Reporte Excel generado por {Usuario} con {Total} libros", User.Identity?.Name, libros.Count);
            return File(contenido, TipoExcel, "ReporteLibros.xlsx");
        }

        // Consulta comun a la vista previa, el PDF y el Excel, para que los tres coincidan siempre
        private async Task<List<Libro>> ConsultarLibrosAsync(string? autor, string? categoria)
        {
            IQueryable<Libro> consulta = _context.Libros.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(autor))
            {
                consulta = consulta.Where(l => l.Autor == autor);
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                consulta = consulta.Where(l => l.Genero == categoria);
            }

            return await consulta.OrderBy(l => l.Id).ToListAsync();
        }
    }
}
