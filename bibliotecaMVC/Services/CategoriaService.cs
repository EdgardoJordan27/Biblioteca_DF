using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Services
{
    // Servicio de Categorías implementado con ADO.NET puro (sin Entity Framework Core).
    // Toda la comunicación con SQL Server se realiza mediante SqlConnection/SqlCommand
    // y consultas parametrizadas, tal como lo exige la actividad.
    public class CategoriaService : ICategoriaService
    {
        private readonly string _connectionString;

        public CategoriaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BibliotecaConnection'.");
        }

        // Mostrar: obtiene todas las categorías registradas en la base de datos.
        public IEnumerable<Categoria> ObtenerTodos()
        {
            var categorias = new List<Categoria>();

            const string query = "SELECT ID, Nombre, Descripcion FROM Categorias ORDER BY Nombre;";

            using var conexion = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(query, conexion);

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                categorias.Add(MapearCategoria(lector));
            }

            return categorias;
        }

        // Obtiene una categoría puntual, utilizada tanto por Editar como por Eliminar.
        public Categoria? ObtenerPorId(int id)
        {
            const string query = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @Id;";

            using var conexion = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Id", id);

            conexion.Open();
            using var lector = comando.ExecuteReader();

            return lector.Read() ? MapearCategoria(lector) : null;
        }

        // Agregar: inserta una nueva categoría (funcionalidad ya vista en clase).
        public void Agregar(Categoria categoria)
        {
            const string query = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion);";

            using var conexion = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            comando.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        // Editar: ejecuta un UPDATE parametrizado. Devuelve false si el ID no existe.
        public bool Actualizar(Categoria categoriaEditada)
        {
            const string query = @"UPDATE Categorias
                                    SET Nombre = @Nombre,
                                        Descripcion = @Descripcion
                                    WHERE ID = @Id;";

            using var conexion = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Nombre", categoriaEditada.Nombre);
            comando.Parameters.AddWithValue("@Descripcion", (object?)categoriaEditada.Descripcion ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Id", categoriaEditada.Id);

            conexion.Open();
            int filasAfectadas = comando.ExecuteNonQuery();

            return filasAfectadas > 0;
        }

        // Eliminar: ejecuta un DELETE parametrizado. Devuelve false si el ID no existe.
        public bool Eliminar(int id)
        {
            const string query = "DELETE FROM Categorias WHERE ID = @Id;";

            using var conexion = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Id", id);

            conexion.Open();
            int filasAfectadas = comando.ExecuteNonQuery();

            return filasAfectadas > 0;
        }

        private static Categoria MapearCategoria(SqlDataReader lector)
        {
            return new Categoria
            {
                Id = lector.GetInt32(lector.GetOrdinal("ID")),
                Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
                Descripcion = lector.IsDBNull(lector.GetOrdinal("Descripcion"))
                    ? null
                    : lector.GetString(lector.GetOrdinal("Descripcion"))
            };
        }
    }
}