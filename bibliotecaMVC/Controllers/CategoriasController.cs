using Microsoft.AspNetCore.Mvc;
using bibliotecaMVC.Models;
using bibliotecaMVC.Services;

namespace bibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categorias
        public IActionResult Index()
        {
            var categorias = _categoriaService.ObtenerTodos();
            return View(categorias);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            _categoriaService.Agregar(categoria);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Edit/5
        public IActionResult Edit(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Categoria categoriaEditada)
        {
            if (id != categoriaEditada.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(categoriaEditada);
            }

            var actualizado = _categoriaService.Actualizar(categoriaEditada);
            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Delete/5
        public IActionResult Delete(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var eliminado = _categoriaService.Eliminar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}