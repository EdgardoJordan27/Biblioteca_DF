using System.ComponentModel.DataAnnotations;
using bibliotecaMVC.Models;
using Xunit;

namespace bibliotecaMVC.Tests
{
    // Valida las reglas de DataAnnotations del modelo Libro, que son las que
    // evalua ModelState.IsValid en el controlador.
    public class LibroModeloTests
    {
        private static Libro LibroValido() => new Libro
        {
            Id = 1,
            Titulo = "El Aleph",
            Autor = "Jorge Luis Borges",
            Genero = "Cuento",
            AnioPublicacion = 1949,
            Disponible = true
        };

        private static List<ValidationResult> Validar(Libro libro)
        {
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(libro, new ValidationContext(libro), resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void Libro_ConDatosCorrectos_PasaLaValidacion()
        {
            var resultados = Validar(LibroValido());

            Assert.Empty(resultados);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Titulo_VacioOSoloEspacios_FallaLaValidacion(string titulo)
        {
            var libro = LibroValido();
            libro.Titulo = titulo;

            var resultados = Validar(libro);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Libro.Titulo)));
        }

        [Fact]
        public void Titulo_ConMasDe150Caracteres_FallaLaValidacion()
        {
            var libro = LibroValido();
            libro.Titulo = new string('a', 151);

            var resultados = Validar(libro);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Libro.Titulo)));
        }

        [Fact]
        public void Titulo_Con150Caracteres_PasaLaValidacion()
        {
            var libro = LibroValido();
            libro.Titulo = new string('a', 150);

            Assert.Empty(Validar(libro));
        }

        [Fact]
        public void Autor_Vacio_FallaLaValidacion()
        {
            var libro = LibroValido();
            libro.Autor = string.Empty;

            var resultados = Validar(libro);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Libro.Autor)));
        }

        [Fact]
        public void Genero_ConMasDe50Caracteres_FallaLaValidacion()
        {
            var libro = LibroValido();
            libro.Genero = new string('g', 51);

            var resultados = Validar(libro);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Libro.Genero)));
        }

        [Theory]
        [InlineData(999, false)]
        [InlineData(1000, true)]
        [InlineData(1949, true)]
        [InlineData(2100, true)]
        [InlineData(2101, false)]
        [InlineData(-5, false)]
        public void AnioPublicacion_SoloEsValidoDentroDelRango(int anio, bool esperadoValido)
        {
            var libro = LibroValido();
            libro.AnioPublicacion = anio;

            var resultados = Validar(libro);
            bool esValido = !resultados.Any(r => r.MemberNames.Contains(nameof(Libro.AnioPublicacion)));

            Assert.Equal(esperadoValido, esValido);
        }

        [Fact]
        public void LibroNuevo_TieneValoresPorDefecto()
        {
            var libro = new Libro();

            Assert.True(libro.Disponible);
            Assert.Equal(DateTime.Now.Year, libro.AnioPublicacion);
            Assert.Null(libro.ImagenNombre);
        }
    }
}
