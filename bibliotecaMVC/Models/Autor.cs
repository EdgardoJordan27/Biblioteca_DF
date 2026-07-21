using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bibliotecaMVC.Models
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nacionalidad es obligatoria")]
        [StringLength(100, ErrorMessage = "La nacionalidad no puede exceder 100 caracteres")]
        public string Nacionalidad { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha de nacimiento")]
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;

        [Display(Name = "¿Está activo?")]
        public bool Activo { get; set; } = true;
    }
}
