using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(150, ErrorMessage = "El título no puede exceder 150 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100, ErrorMessage = "El autor no puede exceder 100 caracteres")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(50, ErrorMessage = "El género no puede exceder 50 caracteres")]
        public string Genero { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "Ingrese un año válido")]
        [DisplayName("Año de publicación")]
        public int AnioPublicacion { get; set; } = DateTime.Now.Year;

        [Display(Name = "¿Disponible?")]
        public bool Disponible { get; set; } = true;

        // Guarda solo el nombre del archivo de imagen (ubicado en wwwroot/images)
        [DisplayName("Imagen")]
        public string? ImagenNombre { get; set; }
    }
}
