using Microsoft.AspNetCore.Mvc;
using bibliotecaMVC.Models;
using bibliotecaMVC.Services;

namespace bibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        // GET: Autores
        public IActionResult Index()
        {
            var autores = _autorService.ObtenerTodos();
            return View(autores);
        }

        // GET: Autores/Edit/5
        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        // POST: Autores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Autor autorEditado)
        {
            if (id != autorEditado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(autorEditado);
            }

            var actualizado = _autorService.Actualizar(autorEditado);
            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Autores/Delete/5
        public IActionResult Delete(int id)
        {
            var autor = _autorService.ObtenerPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        // POST: Autores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _autorService.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}