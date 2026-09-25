using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QOA.DEMO.AccesoDatos;
using QOA.DEMO.Entidades;

namespace QOA.DEMO.Tests
{
    // El campo real en el codigo es PolizaBE.autoReemplazo (equivalente al "incluyeAutoReemplazo" de la historia de usuario).
    [TestClass]
    public class CotizacionDLAutoReemplazoTests
    {
        private const string TEXTO_BENEFICIO = "Auto de Reemplazo";
        private const double COSTO_ESPERADO = 45.00;

        private readonly CotizacionDL cotizacionDL = new CotizacionDL();

        // CalcularBaseProducto y ObtenerBeneficios son privados: se invocan por reflexion para
        // probar la regla de negocio de forma aislada, rapida y sin los Thread.Sleep de cotizar().
        private double InvocarCalcularBaseProducto(PolizaBE p)
        {
            MethodInfo metodo = typeof(CotizacionDL).GetMethod("CalcularBaseProducto", BindingFlags.NonPublic | BindingFlags.Instance);
            return (double)metodo.Invoke(cotizacionDL, new object[] { p });
        }

        private string InvocarObtenerBeneficios(string producto, int idCompania, bool autoReemplazo)
        {
            MethodInfo metodo = typeof(CotizacionDL).GetMethod("ObtenerBeneficios", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)metodo.Invoke(cotizacionDL, new object[] { producto, idCompania, autoReemplazo });
        }

        private static PolizaBE NuevoVehicular(bool autoReemplazo)
        {
            return new PolizaBE { tipoProducto = "VEHICULAR", valorComercial = 50000, usoVehiculo = "PARTICULAR", autoReemplazo = autoReemplazo };
        }

        private static PolizaBE NuevaTarjeta(bool autoReemplazo)
        {
            return new PolizaBE { tipoProducto = "TARJETA", planTarjeta = "PLAN 2", autoReemplazo = autoReemplazo };
        }

        // --- Costo adicional (CalcularBaseProducto): solo VEHICULAR + autoReemplazo=true ---

        [TestMethod]
        public void CalcularBaseProducto_Vehicular_SinAutoReemplazo_NoSumaCosto()
        {
            double baseSinBeneficio = InvocarCalcularBaseProducto(NuevoVehicular(false));

            Assert.AreEqual(50000 * 0.028, baseSinBeneficio, 0.001);
        }

        [TestMethod]
        public void CalcularBaseProducto_Vehicular_ConAutoReemplazo_SumaElCostoFijo()
        {
            double baseConBeneficio = InvocarCalcularBaseProducto(NuevoVehicular(true));
            double baseSinBeneficio = InvocarCalcularBaseProducto(NuevoVehicular(false));

            Assert.AreEqual(COSTO_ESPERADO, baseConBeneficio - baseSinBeneficio, 0.001,
                "El costo adicional de Auto de Reemplazo deberia ser un monto fijo de S/ 45.00 sumado antes del factor de tarifa.");
        }

        [TestMethod]
        public void CalcularBaseProducto_Tarjeta_AunqueAutoReemplazoLleguesEnVerdadero_NoCambiaLaBase()
        {
            // Caso borde de US-010 Escenario 4: si el flag llegara en true por error, Tarjeta no debe verse afectada.
            double baseConFlagIncorrecto = InvocarCalcularBaseProducto(NuevaTarjeta(true));
            double baseNormal = InvocarCalcularBaseProducto(NuevaTarjeta(false));

            Assert.AreEqual(baseNormal, baseConFlagIncorrecto, 0.001);
            Assert.AreEqual(49.90, baseConFlagIncorrecto, 0.001);
        }

        // --- Texto del beneficio (ObtenerBeneficios): aparece en las 5 aseguradoras solo si VEHICULAR + autoReemplazo=true ---

        [DataTestMethod]
        [DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)]
        public void ObtenerBeneficios_Vehicular_ConAutoReemplazo_LoIncluyeParaLasCincoAseguradoras(int idCompania)
        {
            string beneficios = InvocarObtenerBeneficios("VEHICULAR", idCompania, true);

            StringAssert.Contains(beneficios, TEXTO_BENEFICIO,
                $"La aseguradora con idCompania={idCompania} deberia incluir el beneficio cuando fue seleccionado.");
        }

        [DataTestMethod]
        [DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)]
        public void ObtenerBeneficios_Vehicular_SinAutoReemplazo_NoLoMenciona(int idCompania)
        {
            string beneficios = InvocarObtenerBeneficios("VEHICULAR", idCompania, false);

            Assert.IsFalse(beneficios.Contains(TEXTO_BENEFICIO),
                $"La aseguradora con idCompania={idCompania} no deberia mencionar el beneficio si no fue seleccionado.");
        }

        [DataTestMethod]
        [DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)]
        public void ObtenerBeneficios_Tarjeta_AunqueAutoReemplazoLleguesEnVerdadero_NoLoMencionaYTextoEsIdentico(int idCompania)
        {
            // Caso borde de US-010 Escenario 4: Proteccion de Tarjeta nunca debe mostrar el beneficio,
            // incluso si el flag llegara en true por error del llamador.
            string beneficiosConFlagIncorrecto = InvocarObtenerBeneficios("TARJETA", idCompania, true);
            string beneficiosNormal = InvocarObtenerBeneficios("TARJETA", idCompania, false);

            Assert.IsFalse(beneficiosConFlagIncorrecto.Contains(TEXTO_BENEFICIO));
            Assert.AreEqual(beneficiosNormal, beneficiosConFlagIncorrecto);
        }

        // --- Integracion via el metodo publico cotizar(), en modo simulado (sin SQL Server) ---

        [TestMethod]
        public void Cotizar_Vehicular_ConAutoReemplazo_LasCincoOfertasLoIncluyenConSuCosto()
        {
            CotizacionBE cotizacion = cotizacionDL.cotizar(NuevoVehicular(true));

            Assert.AreEqual(5, cotizacion.ofertas.Count, "Deberian generarse ofertas de las 5 aseguradoras (Rimac, Pacifico, MAPFRE, La Positiva, Andina).");
            foreach (OfertaCotizacionBE oferta in cotizacion.ofertas)
            {
                Assert.IsTrue(oferta.autoReemplazo, $"La oferta de la compania {oferta.compania.nombre} deberia quedar marcada con el beneficio.");
                Assert.AreEqual(COSTO_ESPERADO, oferta.costoAutoReemplazo, 0.001,
                    $"La oferta de la compania {oferta.compania.nombre} deberia registrar el costo adicional.");
                StringAssert.Contains(oferta.beneficios, TEXTO_BENEFICIO,
                    $"La oferta de la compania {oferta.compania.nombre} deberia listar el beneficio entre sus coberturas.");
            }
        }

        [TestMethod]
        public void Cotizar_Vehicular_SinAutoReemplazo_NingunaOfertaLoIncluye()
        {
            CotizacionBE cotizacion = cotizacionDL.cotizar(NuevoVehicular(false));

            Assert.AreEqual(5, cotizacion.ofertas.Count);
            foreach (OfertaCotizacionBE oferta in cotizacion.ofertas)
            {
                Assert.IsFalse(oferta.autoReemplazo);
                Assert.AreEqual(0, oferta.costoAutoReemplazo, 0.001);
                Assert.IsFalse(oferta.beneficios.Contains(TEXTO_BENEFICIO));
            }
        }

        [TestMethod]
        public void Cotizar_Tarjeta_AunqueAutoReemplazoLleguesEnVerdadero_NingunaOfertaCambia()
        {
            // Caso borde de US-010 Escenario 4: una cotizacion de Proteccion de Tarjeta no debe
            // verse afectada en absoluto aunque el campo llegue en true por error.
            CotizacionBE cotizacion = cotizacionDL.cotizar(NuevaTarjeta(true));

            Assert.AreEqual(5, cotizacion.ofertas.Count);
            foreach (OfertaCotizacionBE oferta in cotizacion.ofertas)
            {
                Assert.IsFalse(oferta.autoReemplazo, $"La oferta de la compania {oferta.compania.nombre} de Tarjeta no deberia marcarse con el beneficio.");
                Assert.AreEqual(0, oferta.costoAutoReemplazo, 0.001);
                Assert.IsFalse(oferta.beneficios.Contains(TEXTO_BENEFICIO),
                    $"La oferta de la compania {oferta.compania.nombre} de Tarjeta no deberia mencionar el beneficio.");
            }
        }
    }
}
