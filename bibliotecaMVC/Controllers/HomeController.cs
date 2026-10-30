using bibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace bibliotecaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Libros()
        {
            return View();
        }
        public IActionResult Categorias()
        {
            return View();
        }
        public IActionResult Usuarios()
        {
            return View();
        }

        public IActionResult Prestamos()
        {
            return View();
        }

        public IActionResult Acerca()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Se mostro la pagina de error al usuario. RequestId: {RequestId}", requestId);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View(new ErrorViewModel { RequestId = requestId });
        }

        // Paginas para 404 y otros codigos de estado sin contenido
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult EstadoHttp(int codigo)
        {
            _logger.LogWarning("Respuesta HTTP {Codigo} para la ruta solicitada", codigo);
            ViewData["Codigo"] = codigo;
            return View();
        }

        // Solo para la prueba de la pagina de error (Actividad 4)
        [Authorize(Roles = Roles.Admin)]
        public IActionResult ProbarError()
        {
            throw new InvalidOperationException("Excepcion de prueba lanzada a proposito.");
        }
    }
}
