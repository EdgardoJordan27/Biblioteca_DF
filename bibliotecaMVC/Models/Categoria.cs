using System.ComponentModel.DataAnnotations;

namespace bibliotecaMVC.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La descripción no puede exceder 250 caracteres")]
        public string? Descripcion { get; set; }
    }
}