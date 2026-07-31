using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using bibliotecaMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System;

namespace bibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IWebHostEnvironment _entorno;

        public LibrosController(IWebHostEnvironment entorno)
        {
            _entorno = entorno;
        }

        // Lista estática para simular una base de datos en memoria.
        private static List<Libro> Libros = new List<Libro>
        {
            new Libro { Id = 1, Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Realismo mágico", AnioPublicacion = 1967, Disponible = true, ImagenNombre = null },
            new Libro { Id = 2, Titulo = "La casa de los espíritus", Autor = "Isabel Allende", Genero = "Novela", AnioPublicacion = 1982, Disponible = true, ImagenNombre = null },
            new Libro { Id = 3, Titulo = "La ciudad y los perros", Autor = "Mario Vargas Llosa", Genero = "Novela", AnioPublicacion = 1963, Disponible = false, ImagenNombre = null }
        };

        private static int siguienteId = 4;

        // GET: Libros
        public IActionResult Index()
        {
            return View(Libros);
        }

        // GET: Libros/Details/5
        public IActionResult Details(int id)
        {
            var libro = Libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro, IFormFile? Imagen)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            libro.Id = siguienteId++;

            if (Imagen != null && Imagen.Length > 0)
            {
                libro.ImagenNombre = await GuardarImagenAsync(Imagen);
            }

            Libros.Add(libro);
            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Edit/5
        public IActionResult Edit(int id)
        {
            var libro = Libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libroEditado, IFormFile? Imagen)
        {
            if (id != libroEditado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(libroEditado);
            }

            var libro = Libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            libro.Titulo = libroEditado.Titulo;
            libro.Autor = libroEditado.Autor;
            libro.Genero = libroEditado.Genero;
            libro.AnioPublicacion = libroEditado.AnioPublicacion;
            libro.Disponible = libroEditado.Disponible;

            // Solo se reemplaza la imagen si el usuario sube una nueva
            if (Imagen != null && Imagen.Length > 0)
            {
                libro.ImagenNombre = await GuardarImagenAsync(Imagen);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Delete/5
        public IActionResult Delete(int id)
        {
            var libro = Libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = Libros.FirstOrDefault(l => l.Id == id);
            if (libro != null)
            {
                Libros.Remove(libro);
            }
            return RedirectToAction(nameof(Index));
        }

        // Guarda la imagen subida en wwwroot/images y devuelve el nombre del archivo generado
        private async Task<string> GuardarImagenAsync(IFormFile imagen)
        {
            string carpetaImagenes = Path.Combine(_entorno.WebRootPath, "images");

            if (!Directory.Exists(carpetaImagenes))
            {
                Directory.CreateDirectory(carpetaImagenes);
            }

            // Se genera un nombre único para evitar sobreescribir archivos con el mismo nombre
            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCompleta = Path.Combine(carpetaImagenes, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return nombreArchivo;
        }
    }
}
