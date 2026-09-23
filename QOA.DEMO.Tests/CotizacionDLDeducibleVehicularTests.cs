using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QOA.DEMO.AccesoDatos;
using QOA.DEMO.Entidades;

namespace QOA.DEMO.Tests
{
    [TestClass]
    public class CotizacionDLDeducibleVehicularTests
    {
        private readonly CotizacionDL cotizacionDL = new CotizacionDL();

        [DataTestMethod]
        [DataRow(1, 0.04)]
        [DataRow(2, 0.045)]
        [DataRow(3, 0.05)]
        [DataRow(4, 0.055)]
        [DataRow(5, 0.06)]
        public void CalcularDeducibleVehicular_AplicaElPorcentajeEsperadoSegunElOrdenDeLaOferta(int orden, double porcentajeEsperado)
        {
            double valorComercial = 50000;

            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(valorComercial, orden);

            double montoEsperado = Math.Round(valorComercial * porcentajeEsperado, 2);
            Assert.AreEqual(montoEsperado, deducible.monto, 0.001,
                $"El deducible de la oferta {orden} deberia ser {porcentajeEsperado * 100}% del valor comercial.");
        }

        [TestMethod]
        public void CalcularDeducibleVehicular_PrimeraOferta_EmpiezaEnCuatroPorciento()
        {
            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(50000, 1);

            Assert.AreEqual(2000.00, deducible.monto, 0.001);
        }

        [TestMethod]
        public void CalcularDeducibleVehicular_CadaOfertaSiguienteSubeMedioPuntoPorcentual()
        {
            double valorComercial = 50000;

            double porcentajeOferta1 = ObtenerPorcentaje(cotizacionDL.CalcularDeducibleVehicular(valorComercial, 1), valorComercial);
            double porcentajeOferta2 = ObtenerPorcentaje(cotizacionDL.CalcularDeducibleVehicular(valorComercial, 2), valorComercial);
            double porcentajeOferta3 = ObtenerPorcentaje(cotizacionDL.CalcularDeducibleVehicular(valorComercial, 3), valorComercial);
            double porcentajeOferta4 = ObtenerPorcentaje(cotizacionDL.CalcularDeducibleVehicular(valorComercial, 4), valorComercial);

            Assert.AreEqual(0.5, porcentajeOferta2 - porcentajeOferta1, 0.001);
            Assert.AreEqual(0.5, porcentajeOferta3 - porcentajeOferta2, 0.001);
            Assert.AreEqual(0.5, porcentajeOferta4 - porcentajeOferta3, 0.001);
        }

        [TestMethod]
        public void CalcularDeducibleVehicular_ValorComercialCero_DeducibleEsCero()
        {
            // Caso borde: un valor comercial de cero no deberia generar un deducible negativo ni una excepcion.
            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(0, 1);

            Assert.AreEqual(0, deducible.monto);
        }

        [TestMethod]
        public void CalcularDeducibleVehicular_OrdenCero_NoValidaElRangoYAplicaTresPuntoCincoPorciento()
        {
            // Caso borde: el metodo no valida que orden sea >= 1; se documenta el comportamiento actual (3.5%)
            // para que un cambio futuro en la regla sea una decision explicita y no un efecto colateral.
            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(10000, 0);

            Assert.AreEqual(350.00, deducible.monto, 0.001);
        }

        [TestMethod]
        public void CalcularDeducibleVehicular_QuintaOferta_AplicaSeisPorciento()
        {
            // Caso borde real: con 5 aseguradoras activas (Rimac, Pacifico, MAPFRE, La Positiva, Andina)
            // la quinta oferta de una cotizacion vehicular ocurre en producción, no solo en teoria.
            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(50000, 5);

            Assert.AreEqual(3000.00, deducible.monto, 0.001);
        }

        [TestMethod]
        public void CalcularDeducibleVehicular_RedondeaElMontoADosDecimales()
        {
            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(33333.33, 1);

            Assert.AreEqual(1333.33, deducible.monto, 0.0001);
        }

        [DataTestMethod]
        [DataRow(1, 4.0)]
        [DataRow(2, 4.5)]
        [DataRow(3, 5.0)]
        [DataRow(4, 5.5)]
        [DataRow(5, 6.0)]
        public void CalcularDeducibleVehicular_TextoDescriptivoEsConsistenteConElPorcentajeCalculado(int orden, double porcentajeEsperado)
        {
            double valorComercial = 50000;

            DeducibleVehicularBE deducible = cotizacionDL.CalcularDeducibleVehicular(valorComercial, orden);

            double porcentajeCalculado = Math.Round(deducible.monto / valorComercial * 100, 1);
            double porcentajeEnTexto = ExtraerPorcentajeDelTexto(deducible.descripcion);

            Assert.AreEqual(porcentajeEsperado, porcentajeCalculado, 0.001,
                "El porcentaje calculado no respeta la regla de negocio (4%, 4.5%, 5%, 5.5%, ...).");
            Assert.AreEqual(porcentajeCalculado, porcentajeEnTexto, 0.001,
                $"El texto descriptivo dice '{deducible.descripcion}' pero el monto calculado corresponde a {porcentajeCalculado}%.");
        }

        private static double ObtenerPorcentaje(DeducibleVehicularBE deducible, double valorComercial)
        {
            return Math.Round(deducible.monto / valorComercial * 100, 2);
        }

        private static double ExtraerPorcentajeDelTexto(string descripcion)
        {
            string numero = descripcion.Substring(0, descripcion.IndexOf('%'));
            return double.Parse(numero, CultureInfo.InvariantCulture);
        }
    }
}
