using System.Collections.Generic;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerTodos();

        Autor? ObtenerPorId(int id);

        bool Actualizar(Autor autorEditado);

        bool Eliminar(int id);
    }
}