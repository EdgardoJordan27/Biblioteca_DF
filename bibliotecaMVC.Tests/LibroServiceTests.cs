using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Data;
using bibliotecaMVC.Models;
using bibliotecaMVC.Services;
using Xunit;

namespace bibliotecaMVC.Tests
{
    // Pruebas de LibroService con el proveedor InMemory de EF Core, que no usa SQL Server.
    // Cada prueba trabaja con una base en memoria distinta (nombre con Guid), asi que no
    // dependen del orden de ejecucion ni de datos que dejen otras pruebas.
    public class LibroServiceTests
    {
        private static DbContextOptions<BibliotecaContext> NuevasOpciones()
        {
            return new DbContextOptionsBuilder<BibliotecaContext>()
                .UseInMemoryDatabase("libros-" + Guid.NewGuid())
                .Options;
        }

        private static Libro NuevoLibro(int id, string titulo = "Cien años de soledad") => new Libro
        {
            Id = id,
            Titulo = titulo,
            Autor = "Gabriel García Márquez",
            Genero = "Novela",
            AnioPublicacion = 1967,
            Disponible = true,
            ImagenNombre = "original.jpg"
        };

        // Prepara la base con los libros indicados usando un contexto aparte del que usa el servicio
        private static void Sembrar(DbContextOptions<BibliotecaContext> opciones, params Libro[] libros)
        {
            using var contexto = new BibliotecaContext(opciones);
            contexto.Libros.AddRange(libros);
            contexto.SaveChanges();
        }

        [Fact]
        public async Task ObtenerTodos_SinRegistros_DevuelveListaVacia()
        {
            var opciones = NuevasOpciones();
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var libros = await servicio.ObtenerTodosAsync();

            Assert.NotNull(libros);
            Assert.Empty(libros);
        }

        [Fact]
        public async Task ObtenerTodos_ConRegistros_DevuelveTodosLosLibros()
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1), NuevoLibro(2, "Ficciones"), NuevoLibro(3, "Rayuela"));
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var libros = await servicio.ObtenerTodosAsync();
            
            
            Assert.Equal(3, libros.Count);
            Assert.Contains(libros, l => l.Titulo == "Ficciones");
        }

        [Fact]
        public async Task ObtenerPorId_LibroExistente_DevuelveElLibro()
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1), NuevoLibro(2, "Ficciones"));
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var libro = await servicio.ObtenerPorIdAsync(2);

            Assert.NotNull(libro);
            Assert.Equal("Ficciones", libro!.Titulo);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(500)]
        public async Task ObtenerPorId_LibroInexistente_DevuelveNull(int idInexistente)
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1));
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var libro = await servicio.ObtenerPorIdAsync(idInexistente);

            Assert.Null(libro);
        }

        [Fact]
        public async Task Agregar_LibroNuevo_QuedaGuardadoConUnIdAsignado()
        {
            var opciones = NuevasOpciones();
            using (var contexto = new BibliotecaContext(opciones))
            {
                var servicio = new LibroService(contexto);
                var nuevo = NuevoLibro(0, "El Aleph");

                await servicio.AgregarAsync(nuevo);

                Assert.True(nuevo.Id > 0);
            }

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = await contextoVerificacion.Libros.SingleAsync();
            Assert.Equal("El Aleph", guardado.Titulo);
        }

        [Fact]
        public async Task Actualizar_LibroExistente_CambiaLosCampos()
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1));
            using (var contexto = new BibliotecaContext(opciones))
            {
                var servicio = new LibroService(contexto);
                var editado = new Libro
                {
                    Id = 1,
                    Titulo = "El amor en los tiempos del cólera",
                    Autor = "Gabriel García Márquez",
                    Genero = "Romance",
                    AnioPublicacion = 1985,
                    Disponible = false
                };

                var resultado = await servicio.ActualizarAsync(editado, null);

                Assert.True(resultado);
            }

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = await contextoVerificacion.Libros.FindAsync(1);
            Assert.NotNull(guardado);
            Assert.Equal("El amor en los tiempos del cólera", guardado!.Titulo);
            Assert.Equal("Romance", guardado.Genero);
            Assert.Equal(1985, guardado.AnioPublicacion);
            Assert.False(guardado.Disponible);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Actualizar_SinImagenNueva_ConservaLaImagenActual(string? imagenNueva)
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1));
            using (var contexto = new BibliotecaContext(opciones))
            {
                var servicio = new LibroService(contexto);

                await servicio.ActualizarAsync(NuevoLibro(1, "Título editado"), imagenNueva);
            }

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = await contextoVerificacion.Libros.FindAsync(1);
            Assert.Equal("original.jpg", guardado!.ImagenNombre);
        }

        [Fact]
        public async Task Actualizar_ConImagenNueva_ReemplazaElNombreDeLaImagen()
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1));
            using (var contexto = new BibliotecaContext(opciones))
            {
                var servicio = new LibroService(contexto);

                await servicio.ActualizarAsync(NuevoLibro(1), "nueva.png");
            }

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = await contextoVerificacion.Libros.FindAsync(1);
            Assert.Equal("nueva.png", guardado!.ImagenNombre);
        }

        [Fact]
        public async Task Actualizar_LibroInexistente_DevuelveFalseYNoCreaRegistros()
        {
            var opciones = NuevasOpciones();
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var resultado = await servicio.ActualizarAsync(NuevoLibro(42), null);

            Assert.False(resultado);
            Assert.Empty(await contexto.Libros.ToListAsync());
        }

        [Fact]
        public async Task Eliminar_LibroExistente_DevuelveTrueYLoQuitaDeLaBase()
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1), NuevoLibro(2, "Ficciones"));
            using (var contexto = new BibliotecaContext(opciones))
            {
                var servicio = new LibroService(contexto);

                var resultado = await servicio.EliminarAsync(1);

                Assert.True(resultado);
            }

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var restantes = await contextoVerificacion.Libros.ToListAsync();
            Assert.Single(restantes);
            Assert.Equal(2, restantes[0].Id);
        }

        [Fact]
        public async Task Eliminar_LibroInexistente_DevuelveFalseYNoAfectaOtrosLibros()
        {
            var opciones = NuevasOpciones();
            Sembrar(opciones, NuevoLibro(1));
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var resultado = await servicio.EliminarAsync(999);

            Assert.False(resultado);
            Assert.Single(await contexto.Libros.ToListAsync());
        }
    }
}
