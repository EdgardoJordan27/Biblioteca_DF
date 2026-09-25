using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Models;
using bibliotecaMVC.Data;
using System.IO;
using System.Threading.Tasks;
using System;

namespace bibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IWebHostEnvironment _entorno;
        private readonly BibliotecaContext _context;

        public LibrosController(IWebHostEnvironment entorno, BibliotecaContext context)
        {
            _entorno = entorno;
            _context = context;
        }

        // GET: Libros
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
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

            if (Imagen != null && Imagen.Length > 0)
            {
                libro.ImagenNombre = await GuardarImagenAsync(Imagen);
            }

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
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

            var libro = await _context.Libros.FindAsync(id);
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

            _context.Libros.Update(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
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