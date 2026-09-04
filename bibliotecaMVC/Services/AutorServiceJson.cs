using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    // Segunda implementación de IAutorService (Actividad 5 - Reto).
    // En lugar de una lista estática en memoria, los autores se cargan a partir
    // de un JSON. El AutoresController sigue funcionando sin cambios porque solo
    // conoce la interfaz IAutorService, no esta clase.
    public class AutorServiceJson : IAutorService
    {
        private readonly List<Autor> _autores;

        private const string AutoresJson = @"
        [
            { ""Id"": 1, ""Nombre"": ""Claudia"", ""Apellido"": ""Lars"", ""Nacionalidad"": ""Salvadoreña"", ""FechaNacimiento"": ""1899-01-01"", ""Activo"": true },
            { ""Id"": 2, ""Nombre"": ""Roque"", ""Apellido"": ""Dalton"", ""Nacionalidad"": ""Salvadoreño"", ""FechaNacimiento"": ""1935-05-14"", ""Activo"": true },
            { ""Id"": 3, ""Nombre"": ""Manlio"", ""Apellido"": ""Argueta"", ""Nacionalidad"": ""Salvadoreño"", ""FechaNacimiento"": ""1935-11-24"", ""Activo"": true }
        ]";

        public AutorServiceJson()
        {
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _autores = JsonSerializer.Deserialize<List<Autor>>(AutoresJson, opciones) ?? new List<Autor>();
        }

        public IEnumerable<Autor> ObtenerTodos()
        {
            return _autores;
        }

        public Autor? ObtenerPorId(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
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

            _autores.Remove(autor);
            return true;
        }
    }
}
