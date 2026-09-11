using System.Collections.Generic;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    public interface ICategoriaService
    {
        IEnumerable<Categoria> ObtenerTodos();

        Categoria? ObtenerPorId(int id);

        void Agregar(Categoria categoria);

        bool Actualizar(Categoria categoriaEditada);

        bool Eliminar(int id);
    }
}