using bibliotecaMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bibliotecaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var identificador = model.UsuarioOCorreo.Trim();
            var usuario = await _userManager.FindByEmailAsync(identificador)
                          ?? await _userManager.FindByNameAsync(identificador);

            if (usuario != null)
            {
                var resultado = await _signInManager.PasswordSignInAsync(
                    usuario, model.Password, model.Recordarme, lockoutOnFailure: true);

                if (resultado.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }

                if (resultado.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Demasiados intentos fallidos. Intenta de nuevo en unos minutos.");
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new IdentityUser
            {
                UserName = model.NombreUsuario.Trim(),
                Email = model.Email.Trim()
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {
                TempData["Registro"] = "Cuenta creada correctamente. Ya puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, Traducir(error));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private string Traducir(IdentityError error)
        {
            var minimo = _userManager.Options.Password.RequiredLength;

            return error.Code switch
            {
                "DuplicateUserName" => "Ese nombre de usuario ya está en uso.",
                "DuplicateEmail" => "Ese correo electrónico ya está registrado.",
                "InvalidUserName" => "El nombre de usuario contiene caracteres no permitidos.",
                "InvalidEmail" => "El correo electrónico no es válido.",
                "PasswordTooShort" => $"La contraseña debe tener al menos {minimo} caracteres.",
                "PasswordRequiresDigit" => "La contraseña debe incluir al menos un número.",
                "PasswordRequiresLower" => "La contraseña debe incluir al menos una letra minúscula.",
                "PasswordRequiresUpper" => "La contraseña debe incluir al menos una letra mayúscula.",
                "PasswordRequiresNonAlphanumeric" => "La contraseña debe incluir al menos un símbolo.",
                _ => error.Description
            };
        }
    }
}