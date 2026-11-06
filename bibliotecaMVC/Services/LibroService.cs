using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Data;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    // Esta logica antes vivia dentro de LibrosController; se separa para poder probarla
    // de forma aislada y para poder simular el servicio con Moq en las pruebas del controlador.
    public class LibroService : ILibroService
    {
        private readonly BibliotecaContext _context;

        public LibroService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<List<Libro>> ObtenerTodosAsync()
        {
            return await _context.Libros.ToListAsync();
        }

        public async Task<Libro?> ObtenerPorIdAsync(int id)
        {
            return await _context.Libros.FindAsync(id);
        }

        public async Task AgregarAsync(Libro libro)
        {
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ActualizarAsync(Libro libroEditado, string? nuevaImagen)
        {
            var libro = await _context.Libros.FindAsync(libroEditado.Id);
            if (libro == null)
            {
                return false;
            }

            libro.Titulo = libroEditado.Titulo;
            libro.Autor = libroEditado.Autor;
            libro.Genero = libroEditado.Genero;
            libro.AnioPublicacion = libroEditado.AnioPublicacion;
            libro.Disponible = libroEditado.Disponible;

            // Solo se reemplaza la imagen si el usuario sube una nueva
            if (!string.IsNullOrEmpty(nuevaImagen))
            {
                libro.ImagenNombre = nuevaImagen;
            }

            _context.Libros.Update(libro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return false;
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
