namespace bibliotecaMVC.Models
{
    // Datos que usa la vista parcial _EstadoPagina (error, acceso denegado, 404)
    public class EstadoPaginaViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Referencia { get; set; }

        // "candado", "libro" o "alerta"
        public string Icono { get; set; } = "alerta";

        // "error", "aviso" o "permiso"; define el color de acento
        public string Tono { get; set; } = "error";
    }
}
