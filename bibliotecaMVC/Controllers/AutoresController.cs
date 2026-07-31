using Microsoft.AspNetCore.Mvc;
using bibliotecaMVC.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace bibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        // Lista estática para simular una base de datos en memoria.
        // Al ser "static", los datos se mantienen mientras la aplicación esté corriendo.
        private static List<Autor> Autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiano", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
            new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
            new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruano", FechaNacimiento = new DateTime(1936, 3, 28), Activo = false },
            new Autor { Id = 4, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentino", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
            new Autor { Id = 5, Nombre = "Pablo", Apellido = "Neruda", Nacionalidad = "Chileno", FechaNacimiento = new DateTime(1904, 7, 12), Activo = false }
        };

        // GET: Autores
        public IActionResult Index()
        {
            return View(Autores);
        }

        // GET: Autores/Edit/5
        public IActionResult Edit(int id)
        {
            var autor = Autores.FirstOrDefault(a => a.Id == id);
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

            var autor = Autores.FirstOrDefault(a => a.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            // Actualizamos los datos del autor existente en la lista
            autor.Nombre = autorEditado.Nombre;
            autor.Apellido = autorEditado.Apellido;
            autor.Nacionalidad = autorEditado.Nacionalidad;
            autor.FechaNacimiento = autorEditado.FechaNacimiento;
            autor.Activo = autorEditado.Activo;

            return RedirectToAction(nameof(Index));
        }

        // GET: Autores/Delete/5
        public IActionResult Delete(int id)
        {
            var autor = Autores.FirstOrDefault(a => a.Id == id);
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
            var autor = Autores.FirstOrDefault(a => a.Id == id);
            if (autor != null)
            {
                Autores.Remove(autor);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
