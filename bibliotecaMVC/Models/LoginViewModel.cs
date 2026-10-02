using System.ComponentModel.DataAnnotations;

namespace bibliotecaMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingresa tu usuario o correo electrónico")]
        [Display(Name = "Usuario o correo electrónico")]
        public string UsuarioOCorreo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa tu contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Mantener sesión iniciada")]
        public bool Recordarme { get; set; }
    }
}