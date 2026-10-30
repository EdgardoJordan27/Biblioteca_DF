using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Models;
using bibliotecaMVC.Data;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace bibliotecaMVC.Controllers
{
    // Consultar el catalogo requiere haber iniciado sesion
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly IWebHostEnvironment _entorno;
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(IWebHostEnvironment entorno, BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _entorno = entorno;
            _context = context;
            _logger = logger;
        }

        // GET: Libros
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consulta del listado de libros por {Usuario}", User.Identity?.Name);
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consulta del detalle del libro {LibroId}", id);
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Detalle solicitado de un libro inexistente (Id {LibroId})", id);
                return NotFound();
            }
            return View(libro);
        }

        // GET: Libros/Create
        [Authorize(Roles = Roles.GestionLibros)]
        public IActionResult Create()
        {
            _logger.LogInformation("Apertura del formulario para crear un libro por {Usuario}", User.Identity?.Name);
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.GestionLibros)]
        public async Task<IActionResult> Create(Libro libro, IFormFile? Imagen)
        {
            _logger.LogInformation("Inicio de creación de libro por {Usuario}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Creación de libro rechazada por datos no válidos");
                return View(libro);
            }

            try
            {
                if (Imagen != null && Imagen.Length > 0)
                {
                    libro.ImagenNombre = await GuardarImagenAsync(Imagen);
                }

                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is DbUpdateException || ex is IOException)
            {
                _logger.LogError(ex, "Error al guardar un libro nuevo");
                TempData["Error"] = "No se pudo guardar el libro. Intenta de nuevo en unos minutos.";
                return View(libro);
            }

            _logger.LogInformation("Libro {LibroId} creado correctamente", libro.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Edit/5
        [Authorize(Roles = Roles.GestionLibros)]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Apertura del formulario para editar el libro {LibroId}", id);
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Edición solicitada de un libro inexistente (Id {LibroId})", id);
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.GestionLibros)]
        public async Task<IActionResult> Edit(int id, Libro libroEditado, IFormFile? Imagen)
        {
            _logger.LogInformation("Inicio de edición del libro {LibroId} por {Usuario}", id, User.Identity?.Name);

            if (id != libroEditado.Id)
            {
                _logger.LogWarning("El Id de la ruta ({IdRuta}) no coincide con el del formulario ({IdForm})", id, libroEditado.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edición del libro {LibroId} rechazada por datos no válidos", id);
                return View(libroEditado);
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Edición de un libro inexistente (Id {LibroId})", id);
                return NotFound();
            }

            libro.Titulo = libroEditado.Titulo;
            libro.Autor = libroEditado.Autor;
            libro.Genero = libroEditado.Genero;
            libro.AnioPublicacion = libroEditado.AnioPublicacion;
            libro.Disponible = libroEditado.Disponible;

            try
            {
                // Solo se reemplaza la imagen si el usuario sube una nueva
                if (Imagen != null && Imagen.Length > 0)
                {
                    libro.ImagenNombre = await GuardarImagenAsync(Imagen);
                }

                _context.Libros.Update(libro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is DbUpdateException || ex is IOException)
            {
                _logger.LogError(ex, "Error al actualizar el libro {LibroId}", id);
                TempData["Error"] = "No se pudo actualizar el libro. Intenta de nuevo en unos minutos.";
                return View(libroEditado);
            }

            _logger.LogInformation("Libro {LibroId} actualizado correctamente", id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Delete/5
        [Authorize(Roles = Roles.GestionLibros)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Apertura de la confirmación para eliminar el libro {LibroId}", id);
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Eliminación solicitada de un libro inexistente (Id {LibroId})", id);
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.GestionLibros)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Inicio de eliminación del libro {LibroId} por {Usuario}", id, User.Identity?.Name);

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Se intentó eliminar un libro que ya no existe (Id {LibroId})", id);
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al eliminar el libro {LibroId}", id);
                TempData["Error"] = "No se pudo eliminar el libro. Intenta de nuevo en unos minutos.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Libro {LibroId} eliminado correctamente", id);
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