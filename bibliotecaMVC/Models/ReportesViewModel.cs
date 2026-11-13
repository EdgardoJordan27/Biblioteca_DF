namespace bibliotecaMVC.Models
{
    // Datos que necesita la vista de reportes: los filtros disponibles, el filtro
    // seleccionado y los libros que cumplen con ese filtro.
    public class ReportesViewModel
    {
        public string? Autor { get; set; }

        public string? Categoria { get; set; }

        public List<string> Autores { get; set; } = new();

        public List<string> Categorias { get; set; } = new();

        public List<Libro> Libros { get; set; } = new();
    }
}
