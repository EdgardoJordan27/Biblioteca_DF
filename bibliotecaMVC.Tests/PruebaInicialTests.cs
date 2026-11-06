using Xunit;

namespace bibliotecaMVC.Tests
{
    // Prueba de humo para confirmar que el proyecto de pruebas se configuro bien
    public class PruebaInicialTests
    {
        [Fact]
        public void Entorno_DePruebas_Funciona()
        {
            int resultado = 2 + 2;

            Assert.Equal(4, resultado);
        }
    }
}
