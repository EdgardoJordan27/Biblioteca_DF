using System;
using System.Collections.Generic;
using System.Linq;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    public class AutorService : IAutorService
    {
        // Lista estática para simular una base de datos en memoria.
        // Esta lógica antes vivía dentro del controlador; ahora es responsabilidad del servicio.
        private static List<Autor> Autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiano", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
            new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
            new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruano", FechaNacimiento = new DateTime(1936, 3, 28), Activo = false },
            new Autor { Id = 4, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentino", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
            new Autor { Id = 5, Nombre = "Pablo", Apellido = "Neruda", Nacionalidad = "Chileno", FechaNacimiento = new DateTime(1904, 7, 12), Activo = false }
        };

        public IEnumerable<Autor> ObtenerTodos()
        {
            return Autores;
        }

        public Autor? ObtenerPorId(int id)
        {
            return Autores.FirstOrDefault(a => a.Id == id);
        }

        public bool Actualizar(Autor autorEditado)
        {
            var autor = ObtenerPorId(autorEditado.Id);
            if (autor == null)
            {
                return false;
            }

            autor.Nombre = autorEditado.Nombre;
            autor.Apellido = autorEditado.Apellido;
            autor.Nacionalidad = autorEditado.Nacionalidad;
            autor.FechaNacimiento = autorEditado.FechaNacimiento;
            autor.Activo = autorEditado.Activo;

            return true;
        }

        public bool Eliminar(int id)
        {
            var autor = ObtenerPorId(id);
            if (autor == null)
            {
                return false;
            }

            Autores.Remove(autor);
            return true;
        }
    }
}