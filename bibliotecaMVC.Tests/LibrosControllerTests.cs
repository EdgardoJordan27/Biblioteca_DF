using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using bibliotecaMVC.Controllers;
using bibliotecaMVC.Models;
using bibliotecaMVC.Services;
using Xunit;

namespace bibliotecaMVC.Tests
{
    // Pruebas de LibrosController. El servicio de libros se simula con Moq, por lo que
    // ninguna prueba toca la base de datos. Cada prueba crea su propia instancia de la clase
    // (comportamiento de xUnit), asi que los mocks no se comparten entre pruebas.
    public class LibrosControllerTests : IDisposable
    {
        private readonly Mock<ILibroService> _servicio = new Mock<ILibroService>();
        private readonly Mock<IWebHostEnvironment> _entorno = new Mock<IWebHostEnvironment>();
        private readonly string _raizTemporal;

        public LibrosControllerTests()
        {
            // Carpeta temporal para las pruebas que guardan imagenes, se borra en Dispose
            _raizTemporal = Path.Combine(Path.GetTempPath(), "biblioteca-tests-" + Guid.NewGuid());
            Directory.CreateDirectory(_raizTemporal);
            _entorno.Setup(e => e.WebRootPath).Returns(_raizTemporal);
        }

        public void Dispose()
        {
            if (Directory.Exists(_raizTemporal))
            {
                Directory.Delete(_raizTemporal, recursive: true);
            }
        }

        private LibrosController CrearControlador()
        {
            var httpContext = new DefaultHttpContext();
            return new LibrosController(_entorno.Object, _servicio.Object, NullLogger<LibrosController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            };
        }

        private static Libro NuevoLibro(int id = 1, string titulo = "Cien años de soledad") => new Libro
        {
            Id = id,
            Titulo = titulo,
            Autor = "Gabriel García Márquez",
            Genero = "Novela",
            AnioPublicacion = 1967,
            Disponible = true
        };

        private static Mock<IFormFile> CrearImagenSimulada(string nombreArchivo)
        {
            var archivo = new Mock<IFormFile>();
            archivo.Setup(f => f.Length).Returns(3);
            archivo.Setup(f => f.FileName).Returns(nombreArchivo);
            archivo.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                   .Returns((Stream destino, CancellationToken _) => destino.WriteAsync(new byte[] { 1, 2, 3 }).AsTask());
            return archivo;
        }

        // Consulta

        [Fact]
        public async Task Index_ConLibrosRegistrados_DevuelveLaListaEnLaVista()
        {
            var libros = new List<Libro> { NuevoLibro(1), NuevoLibro(2, "La casa de los espíritus") };
            _servicio.Setup(s => s.ObtenerTodosAsync()).ReturnsAsync(libros);
            var controlador = CrearControlador();

            var resultado = await controlador.Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IEnumerable<Libro>>(vista.Model);
            Assert.Equal(2, modelo.Count());
            _servicio.Verify(s => s.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task Index_SinLibrosRegistrados_DevuelveVistaConListaVacia()
        {
            _servicio.Setup(s => s.ObtenerTodosAsync()).ReturnsAsync(new List<Libro>());
            var controlador = CrearControlador();

            var resultado = await controlador.Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IEnumerable<Libro>>(vista.Model);
            Assert.Empty(modelo);
        }

        [Fact]
        public async Task Details_LibroExistente_DevuelveLaVistaConElLibro()
        {
            var libro = NuevoLibro(5, "Rayuela");
            _servicio.Setup(s => s.ObtenerPorIdAsync(5)).ReturnsAsync(libro);
            var controlador = CrearControlador();

            var resultado = await controlador.Details(5);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Libro>(vista.Model);
            Assert.NotNull(modelo);
            Assert.Equal("Rayuela", modelo.Titulo);
            Assert.Equal(5, modelo.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(9999)]
        public async Task Details_LibroInexistente_DevuelveNotFound(int idInexistente)
        {
            _servicio.Setup(s => s.ObtenerPorIdAsync(It.IsAny<int>())).ReturnsAsync((Libro?)null);
            var controlador = CrearControlador();

            var resultado = await controlador.Details(idInexistente);

            Assert.IsType<NotFoundResult>(resultado);
            _servicio.Verify(s => s.ObtenerPorIdAsync(idInexistente), Times.Once);
        }

        // Registro

        [Fact]
        public void CreateGet_DevuelveLaVistaDelFormulario()
        {
            var controlador = CrearControlador();

            var resultado = controlador.Create();

            Assert.IsType<ViewResult>(resultado);
        }

        [Fact]
        public async Task CreatePost_ConDatosValidos_GuardaElLibroYRedirigeAlListado()
        {
            var libro = NuevoLibro(0, "Ficciones");
            _servicio.Setup(s => s.AgregarAsync(It.IsAny<Libro>())).Returns(Task.CompletedTask);
            var controlador = CrearControlador();

            var resultado = await controlador.Create(libro, null);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            _servicio.Verify(s => s.AgregarAsync(libro), Times.Once);
        }

        [Fact]
        public async Task CreatePost_ConModeloInvalido_NoGuardaYRegresaLaMismaVista()
        {
            var libro = NuevoLibro();
            libro.Titulo = string.Empty;
            var controlador = CrearControlador();
            controlador.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio");

            var resultado = await controlador.Create(libro, null);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(libro, vista.Model);
            Assert.False(controlador.ModelState.IsValid);
            _servicio.Verify(s => s.AgregarAsync(It.IsAny<Libro>()), Times.Never);
        }

        [Fact]
        public async Task CreatePost_ConImagen_GuardaElArchivoYAsignaElNombreAlLibro()
        {
            Libro? guardado = null;
            _servicio.Setup(s => s.AgregarAsync(It.IsAny<Libro>()))
                     .Callback<Libro>(l => guardado = l)
                     .Returns(Task.CompletedTask);
            var imagen = CrearImagenSimulada("portada.png");
            var controlador = CrearControlador();

            var resultado = await controlador.Create(NuevoLibro(0), imagen.Object);

            Assert.IsType<RedirectToActionResult>(resultado);
            Assert.NotNull(guardado);
            Assert.NotNull(guardado!.ImagenNombre);
            Assert.EndsWith(".png", guardado.ImagenNombre);
            string rutaEsperada = Path.Combine(_raizTemporal, "images", guardado.ImagenNombre!);
            Assert.True(File.Exists(rutaEsperada));
        }

        [Fact]
        public async Task CreatePost_SinImagen_DejaImagenNombreEnNull()
        {
            Libro? guardado = null;
            _servicio.Setup(s => s.AgregarAsync(It.IsAny<Libro>()))
                     .Callback<Libro>(l => guardado = l)
                     .Returns(Task.CompletedTask);
            var controlador = CrearControlador();

            await controlador.Create(NuevoLibro(0), null);

            Assert.NotNull(guardado);
            Assert.Null(guardado!.ImagenNombre);
        }

        [Fact]
        public async Task CreatePost_CuandoFallaLaBaseDeDatos_MuestraMensajeYRegresaLaVista()
        {
            _servicio.Setup(s => s.AgregarAsync(It.IsAny<Libro>()))
                     .ThrowsAsync(new DbUpdateException("Fallo simulado", new Exception("detalle")));
            var controlador = CrearControlador();

            var resultado = await controlador.Create(NuevoLibro(0), null);

            Assert.IsType<ViewResult>(resultado);
            Assert.True(controlador.TempData.ContainsKey("Error"));
        }

        // Edicion

        [Fact]
        public async Task EditGet_LibroExistente_DevuelveLaVistaConElLibro()
        {
            var libro = NuevoLibro(3);
            _servicio.Setup(s => s.ObtenerPorIdAsync(3)).ReturnsAsync(libro);
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(3);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(libro, vista.Model);
        }

        [Fact]
        public async Task EditGet_LibroInexistente_DevuelveNotFound()
        {
            _servicio.Setup(s => s.ObtenerPorIdAsync(It.IsAny<int>())).ReturnsAsync((Libro?)null);
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(77);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task EditPost_ConDatosValidos_ActualizaYRedirigeAlListado()
        {
            var libro = NuevoLibro(4, "Pedro Páramo");
            _servicio.Setup(s => s.ObtenerPorIdAsync(4)).ReturnsAsync(NuevoLibro(4));
            _servicio.Setup(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>())).ReturnsAsync(true);
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(4, libro, null);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            _servicio.Verify(s => s.ActualizarAsync(libro, null), Times.Once);
        }

        [Fact]
        public async Task EditPost_ConImagenNueva_PasaElNombreDelArchivoAlServicio()
        {
            string? nombreRecibido = null;
            _servicio.Setup(s => s.ObtenerPorIdAsync(4)).ReturnsAsync(NuevoLibro(4));
            _servicio.Setup(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>()))
                     .Callback<Libro, string?>((_, nombre) => nombreRecibido = nombre)
                     .ReturnsAsync(true);
            var imagen = CrearImagenSimulada("nueva.jpg");
            var controlador = CrearControlador();

            await controlador.Edit(4, NuevoLibro(4), imagen.Object);

            Assert.NotNull(nombreRecibido);
            Assert.EndsWith(".jpg", nombreRecibido);
            Assert.True(File.Exists(Path.Combine(_raizTemporal, "images", nombreRecibido!)));
        }

        [Fact]
        public async Task EditPost_ConIdDistintoAlDelFormulario_DevuelveNotFoundSinActualizar()
        {
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(10, NuevoLibro(11), null);

            Assert.IsType<NotFoundResult>(resultado);
            _servicio.Verify(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_ConModeloInvalido_NoActualizaYRegresaLaVista()
        {
            var libro = NuevoLibro(4);
            var controlador = CrearControlador();
            controlador.ModelState.AddModelError(nameof(Libro.AnioPublicacion), "Ingrese un año válido");

            var resultado = await controlador.Edit(4, libro, null);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(libro, vista.Model);
            _servicio.Verify(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_LibroInexistente_DevuelveNotFoundSinActualizar()
        {
            _servicio.Setup(s => s.ObtenerPorIdAsync(It.IsAny<int>())).ReturnsAsync((Libro?)null);
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(50, NuevoLibro(50), null);

            Assert.IsType<NotFoundResult>(resultado);
            _servicio.Verify(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_CuandoElServicioNoEncuentraElLibro_DevuelveNotFound()
        {
            _servicio.Setup(s => s.ObtenerPorIdAsync(8)).ReturnsAsync(NuevoLibro(8));
            _servicio.Setup(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>())).ReturnsAsync(false);
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(8, NuevoLibro(8), null);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task EditPost_CuandoFallaLaBaseDeDatos_MuestraMensajeYRegresaLaVista()
        {
            _servicio.Setup(s => s.ObtenerPorIdAsync(4)).ReturnsAsync(NuevoLibro(4));
            _servicio.Setup(s => s.ActualizarAsync(It.IsAny<Libro>(), It.IsAny<string?>()))
                     .ThrowsAsync(new DbUpdateException("Fallo simulado", new Exception("detalle")));
            var controlador = CrearControlador();

            var resultado = await controlador.Edit(4, NuevoLibro(4), null);

            Assert.IsType<ViewResult>(resultado);
            Assert.True(controlador.TempData.ContainsKey("Error"));
        }

        // Eliminacion

        [Fact]
        public async Task DeleteGet_LibroExistente_DevuelveLaVistaDeConfirmacion()
        {
            var libro = NuevoLibro(6);
            _servicio.Setup(s => s.ObtenerPorIdAsync(6)).ReturnsAsync(libro);
            var controlador = CrearControlador();

            var resultado = await controlador.Delete(6);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(libro, vista.Model);
        }

        [Fact]
        public async Task DeleteGet_LibroInexistente_DevuelveNotFound()
        {
            _servicio.Setup(s => s.ObtenerPorIdAsync(It.IsAny<int>())).ReturnsAsync((Libro?)null);
            var controlador = CrearControlador();

            var resultado = await controlador.Delete(123);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task DeleteConfirmed_LibroExistente_EliminaYRedirigeAlListado()
        {
            _servicio.Setup(s => s.EliminarAsync(6)).ReturnsAsync(true);
            var controlador = CrearControlador();

            var resultado = await controlador.DeleteConfirmed(6);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.False(controlador.TempData.ContainsKey("Error"));
            _servicio.Verify(s => s.EliminarAsync(6), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_LibroInexistente_RedirigeAlListadoSinMostrarError()
        {
            _servicio.Setup(s => s.EliminarAsync(It.IsAny<int>())).ReturnsAsync(false);
            var controlador = CrearControlador();

            var resultado = await controlador.DeleteConfirmed(999);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.False(controlador.TempData.ContainsKey("Error"));
        }

        [Fact]
        public async Task DeleteConfirmed_CuandoFallaLaBaseDeDatos_MuestraMensajeYRedirige()
        {
            _servicio.Setup(s => s.EliminarAsync(It.IsAny<int>()))
                     .ThrowsAsync(new DbUpdateException("Fallo simulado", new Exception("detalle")));
            var controlador = CrearControlador();

            var resultado = await controlador.DeleteConfirmed(6);

            Assert.IsType<RedirectToActionResult>(resultado);
            Assert.True(controlador.TempData.ContainsKey("Error"));
        }
    }
}
