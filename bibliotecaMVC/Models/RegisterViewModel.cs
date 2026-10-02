using System.ComponentModel.DataAnnotations;

namespace bibliotecaMVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 30 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Solo se permiten letras, números, punto, guion y guion bajo")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}