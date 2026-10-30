using Microsoft.AspNetCore.Mvc;
using bibliotecaMVC.Models;
using bibliotecaMVC.Services;
using Microsoft.Data.SqlClient;

namespace bibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        // GET: Categorias
        public IActionResult Index()
        {
            _logger.LogInformation("Consulta del listado de categorías");
            var categorias = _categoriaService.ObtenerTodos();
            return View(categorias);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Apertura del formulario para crear una categoría");
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation("Inicio de creación de categoría");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Creación de categoría rechazada por datos no válidos");
                return View(categoria);
            }

            try
            {
                _categoriaService.Agregar(categoria);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error de base de datos al crear una categoría");
                TempData["Error"] = "No se pudo guardar la categoría. Intenta de nuevo en unos minutos.";
                return View(categoria);
            }

            _logger.LogInformation("Categoría creada correctamente");
            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Edit/5
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Apertura del formulario para editar la categoría {CategoriaId}", id);
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null)
            {
                _logger.LogWarning("Edición solicitada de una categoría inexistente (Id {CategoriaId})", id);
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Categoria categoriaEditada)
        {
            _logger.LogInformation("Inicio de edición de la categoría {CategoriaId}", id);

            if (id != categoriaEditada.Id)
            {
                _logger.LogWarning("El Id de la ruta ({IdRuta}) no coincide con el del formulario ({IdForm})", id, categoriaEditada.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edición de la categoría {CategoriaId} rechazada por datos no válidos", id);
                return View(categoriaEditada);
            }

            bool actualizado;
            try
            {
                actualizado = _categoriaService.Actualizar(categoriaEditada);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error de base de datos al actualizar la categoría {CategoriaId}", id);
                TempData["Error"] = "No se pudo actualizar la categoría. Intenta de nuevo en unos minutos.";
                return View(categoriaEditada);
            }

            if (!actualizado)
            {
                _logger.LogWarning("Edición de una categoría inexistente (Id {CategoriaId})", id);
                return NotFound();
            }

            _logger.LogInformation("Categoría {CategoriaId} actualizada correctamente", id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Delete/5
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Apertura de la confirmación para eliminar la categoría {CategoriaId}", id);
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null)
            {
                _logger.LogWarning("Eliminación solicitada de una categoría inexistente (Id {CategoriaId})", id);
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Inicio de eliminación de la categoría {CategoriaId}", id);

            bool eliminado;
            try
            {
                eliminado = _categoriaService.Eliminar(id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar la categoría {CategoriaId}", id);
                TempData["Error"] = "No se pudo eliminar la categoría. Puede estar en uso o la base de datos no responde.";
                return RedirectToAction(nameof(Index));
            }

            if (!eliminado)
            {
                _logger.LogWarning("Se intentó eliminar una categoría inexistente (Id {CategoriaId})", id);
                return NotFound();
            }

            _logger.LogInformation("Categoría {CategoriaId} eliminada correctamente", id);
            return RedirectToAction(nameof(Index));
        }
    }
}