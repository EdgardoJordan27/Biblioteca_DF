using System.Collections.Generic;
using System.Threading.Tasks;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    public interface ILibroService
    {
        Task<List<Libro>> ObtenerTodosAsync();

        Task<Libro?> ObtenerPorIdAsync(int id);

        Task AgregarAsync(Libro libro);

        // Si nuevaImagen es null o vacia se conserva la imagen actual del libro
        Task<bool> ActualizarAsync(Libro libroEditado, string? nuevaImagen);

        Task<bool> EliminarAsync(int id);
    }
}
