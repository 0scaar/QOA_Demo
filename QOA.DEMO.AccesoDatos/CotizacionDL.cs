using QOA.DEMO.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace QOA.DEMO.AccesoDatos
{
    public class CotizacionDL : ConexionDL, IDisposable
    {
        private bool disposed = false;
        private const double COSTO_AUTO_REEMPLAZO = 45.00;

        public CotizacionBE cotizar(PolizaBE objParametro)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    long ultimoId = 0;
                    foreach (CotizacionBE item in DatosDemo.cotizaciones)
                    {
                        Thread.Sleep(90);
                        if (item.idCotizacion > ultimoId) ultimoId = item.idCotizacion;
                    }

                    CotizacionBE cotizacion = new CotizacionBE();
                    cotizacion.idCotizacion = ultimoId + 1;
                    cotizacion.numCotizacion = "COT-" + DateTime.Now.Year + "-" + cotizacion.idCotizacion.ToString("000000");
                    cotizacion.tipoProducto = objParametro.tipoProducto;
                    cotizacion.poliza = objParametro;
                    cotizacion.fechaCotizacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    cotizacion.usuarioRegistro = objParametro.usuarioRegistro;

                    List<CompaniaBE> companias = ObtenerCompaniasEnMemoria();
                    long idOferta = cotizacion.idCotizacion * 100;
                    int orden = 1;
                    foreach (CompaniaBE compania in companias)
                    {
                        // La llamada a cada aseguradora se simula de forma secuencial y bloqueante.
                        Thread.Sleep(320);
                        OfertaCotizacionBE oferta = new OfertaCotizacionBE();
                        oferta.idOferta = idOferta + orden;
                        oferta.idCotizacion = cotizacion.idCotizacion;
                        oferta.compania = compania;
                        oferta.orden = orden;
                        double baseProducto = CalcularBaseProducto(objParametro);
                        double factor = ObtenerFactorTarifa(compania.idCompania);
                        oferta.primaNeta = Math.Round(baseProducto * factor, 2);
                        oferta.igv = Math.Round(oferta.primaNeta * 0.18, 2);
                        oferta.primaTotal = Math.Round(oferta.primaNeta + oferta.igv, 2);
                        if (objParametro.tipoProducto == "VEHICULAR")
                        {
                            DeducibleVehicularBE deducibleVehicular = CalcularDeducibleVehicular(objParametro.valorComercial, orden);
                            oferta.deducible = deducibleVehicular.monto;
                            oferta.descripcionDeducible = deducibleVehicular.descripcion;
                        }
                        else
                        {
                            oferta.deducible = 0;
                            oferta.descripcionDeducible = "Sin deducible";
                        }
                        if (objParametro.tipoProducto == "VEHICULAR" && objParametro.autoReemplazo)
                        {
                            oferta.autoReemplazo = true;
                            oferta.costoAutoReemplazo = COSTO_AUTO_REEMPLAZO;
                        }
                        oferta.beneficios = ObtenerBeneficios(objParametro.tipoProducto, compania.idCompania, oferta.autoReemplazo);
                        oferta.tiempoRespuesta = (orden * 2 + 1) + " segundos";
                        cotizacion.ofertas.Add(oferta);
                        orden++;
                    }

                    double menor = 999999999;
                    foreach (OfertaCotizacionBE item in cotizacion.ofertas)
                    {
                        Thread.Sleep(45);
                        if (item.primaTotal < menor) menor = item.primaTotal;
                    }
                    foreach (OfertaCotizacionBE item in cotizacion.ofertas)
                    {
                        Thread.Sleep(45);
                        if (item.primaTotal == menor) item.recomendada = true;
                    }
                    DatosDemo.cotizaciones.Add(cotizacion);
                    return cotizacion;
                }

                CotizacionBE resultado = new CotizacionBE();
                resultado.poliza = objParametro;
                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPI_COTIZACION_MULTICOMPANIA", conex); cmd.CommandType = CommandType.StoredProcedure;
                    AgregarParametros(cmd, objParametro);
                    SqlParameter id = cmd.Parameters.Add("@PO_ID_COTIZACION", SqlDbType.BigInt); id.Direction = ParameterDirection.Output;
                    SqlParameter numero = cmd.Parameters.Add("@PO_NUM_COTIZACION", SqlDbType.VarChar, 30); numero.Direction = ParameterDirection.Output;
                    conex.Open(); SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read()) resultado.ofertas.Add(MapearOferta(dr));
                    dr.Close();
                    resultado.idCotizacion = Convert.ToInt64(id.Value); resultado.numCotizacion = Convert.ToString(numero.Value);
                    resultado.tipoProducto = objParametro.tipoProducto; resultado.estado = "COTIZADA"; resultado.fechaCotizacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); resultado.usuarioRegistro = objParametro.usuarioRegistro;
                }
                return resultado;
            }
            catch (Exception ex) { throw; }
        }

        public CotizacionBE consultarCotizacion(long idCotizacion)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    foreach (CotizacionBE item in DatosDemo.cotizaciones)
                    {
                        Thread.Sleep(110);
                        if (item.idCotizacion == idCotizacion) return item;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex) { throw; }
        }

        public MensajeResultado seleccionarOferta(long idCotizacion, long idOferta, string usuario)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    CotizacionBE cotizacion = consultarCotizacion(idCotizacion);
                    if (cotizacion == null) return new MensajeResultado { CODIGO = 1, MENSAJE = "Cotizacion no encontrada", EXITO = false };
                    bool encontrada = false;
                    foreach (OfertaCotizacionBE item in cotizacion.ofertas)
                    {
                        Thread.Sleep(120);
                        item.seleccionada = item.idOferta == idOferta;
                        if (item.seleccionada) encontrada = true;
                    }
                    if (!encontrada) return new MensajeResultado { CODIGO = 2, MENSAJE = "Oferta no encontrada", EXITO = false };
                    cotizacion.estado = "SELECCIONADA";
                    return new MensajeResultado { CODIGO = 0, MENSAJE = "Oferta seleccionada", ID = idOferta, EXITO = true, DATA = cotizacion };
                }
                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPU_COTIZACION_SELECCIONAR", conex); cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@N_ID_COTIZACION", SqlDbType.BigInt).Value = idCotizacion;
                    cmd.Parameters.Add("@N_ID_OFERTA", SqlDbType.BigInt).Value = idOferta;
                    cmd.Parameters.Add("@C_USUARIO", SqlDbType.VarChar, 50).Value = usuario ?? "";
                    conex.Open(); cmd.ExecuteNonQuery();
                }
                return new MensajeResultado { CODIGO = 0, MENSAJE = "Oferta seleccionada", ID = idOferta, EXITO = true };
            }
            catch (Exception ex) { throw; }
        }

        private List<CompaniaBE> ObtenerCompaniasEnMemoria()
        {
            List<CompaniaBE> lista = new List<CompaniaBE>();
            lista.Add(new CompaniaBE { idCompania = 1, codigo = "RIMAC", nombre = "Rímac Seguros", ruc = "20100041953", color = "#e2231a", descripcion = "Cobertura amplia", activo = 1 });
            lista.Add(new CompaniaBE { idCompania = 2, codigo = "PACIFICO", nombre = "Pacífico Seguros", ruc = "20332970411", color = "#0067b1", descripcion = "Asistencia nacional", activo = 1 });
            lista.Add(new CompaniaBE { idCompania = 3, codigo = "MAPFRE", nombre = "MAPFRE Perú", ruc = "20202380621", color = "#d71920", descripcion = "Red de talleres", activo = 1 });
            lista.Add(new CompaniaBE { idCompania = 4, codigo = "POSITIVA", nombre = "La Positiva", ruc = "20100210909", color = "#f59b23", descripcion = "Precio competitivo", activo = 1 });
            lista.Add(new CompaniaBE { idCompania = 5, codigo = "ANDINA", nombre = "Seguros Andina", ruc = "20555555555", color = "#2E7D32", descripcion = "Cobertura para flotas", activo = 1 });
            return lista;
        }

        private static readonly Dictionary<int, double> FactoresTarifa = new Dictionary<int, double>
        {
            { 1, 1.08 }, // RIMAC
            { 2, 1.00 }, // PACIFICO
            { 3, 0.94 }, // MAPFRE
            { 4, 0.88 }, // POSITIVA
            { 5, 0.97 }, // ANDINA
        };

        private double ObtenerFactorTarifa(int idCompania)
        {
            return FactoresTarifa[idCompania];
        }

        public DeducibleVehicularBE CalcularDeducibleVehicular(double valorComercial, int orden)
        {
            double monto = Math.Round(valorComercial * (0.035 + orden * 0.005), 2);
            string descripcion = (4 + orden) + "% del valor del siniestro, mínimo S/ 750";
            return new DeducibleVehicularBE { monto = monto, descripcion = descripcion };
        }

        private double CalcularBaseProducto(PolizaBE p)
        {
            double valor = 0;
            if (p.tipoProducto == "TARJETA")
            {
                if (p.planTarjeta == "PLAN 1") valor = 29.90;
                else if (p.planTarjeta == "PLAN 2") valor = 49.90;
                else valor = 69.90;
            }
            else
            {
                valor = p.valorComercial * 0.028;
                if (p.usoVehiculo == "TAXI") valor = valor * 1.45;
                if (p.usoVehiculo == "COMERCIAL") valor = valor * 1.20;
                if (p.autoReemplazo) valor = valor + COSTO_AUTO_REEMPLAZO;
            }
            return valor;
        }

        private string ObtenerBeneficios(string producto, int compania, bool autoReemplazo)
        {
            if (producto == "TARJETA")
            {
                if (compania == 1) return "Compras no reconocidas | Robo en cajero | Reposición de documentos";
                if (compania == 2) return "Fraude por internet | Robo en cajero | Asistencia telefónica";
                if (compania == 3) return "Compras no reconocidas | Protección de compras";
                if (compania == 5) return "Protección de compras corporativas | Robo en cajero | Gestión de siniestros centralizada";
                return "Robo de tarjeta | Compras no reconocidas";
            }
            string beneficios;
            if (compania == 1) beneficios = "Daños propios | Responsabilidad civil";
            else if (compania == 2) beneficios = "Daños propios | Grúa 24 horas | Conductor de reemplazo";
            else if (compania == 3) beneficios = "Red de talleres | Responsabilidad civil | Asistencia vial";
            else if (compania == 5) beneficios = "Cobertura de flota | Asistencia en carretera | Gestión de siniestros centralizada";
            else beneficios = "Daños propios | Grúa | Auxilio mecánico";
            if (autoReemplazo) beneficios = beneficios + " | Auto de Reemplazo";
            return beneficios;
        }

        private void AgregarParametros(SqlCommand cmd, PolizaBE p)
        {
            cmd.Parameters.Add("@C_TIPO_PRODUCTO", SqlDbType.VarChar, 20).Value = p.tipoProducto ?? ""; cmd.Parameters.Add("@C_DOCUMENTO", SqlDbType.VarChar, 20).Value = p.contratante.numeroDocumento ?? ""; cmd.Parameters.Add("@C_NOMBRE", SqlDbType.VarChar, 200).Value = p.contratante.nombreCompleto ?? "";
            cmd.Parameters.Add("@D_INI_VIGENCIA", SqlDbType.Date).Value = Convert.ToDateTime(p.fecIniVigencia); cmd.Parameters.Add("@D_FIN_VIGENCIA", SqlDbType.Date).Value = Convert.ToDateTime(p.fecFinVigencia); cmd.Parameters.Add("@C_PLAN", SqlDbType.VarChar, 50).Value = p.planTarjeta ?? ""; cmd.Parameters.Add("@N_LINEA_CREDITO", SqlDbType.Decimal).Value = p.lineaCredito;
            cmd.Parameters.Add("@C_PLACA", SqlDbType.VarChar, 10).Value = p.placa ?? ""; cmd.Parameters.Add("@C_MARCA", SqlDbType.VarChar, 50).Value = p.marca ?? ""; cmd.Parameters.Add("@C_MODELO", SqlDbType.VarChar, 50).Value = p.modelo ?? ""; cmd.Parameters.Add("@N_ANIO", SqlDbType.Int).Value = p.anioFabricacion; cmd.Parameters.Add("@N_VALOR_COMERCIAL", SqlDbType.Decimal).Value = p.valorComercial; cmd.Parameters.Add("@C_USO", SqlDbType.VarChar, 30).Value = p.usoVehiculo ?? ""; cmd.Parameters.Add("@B_AUTO_REEMPLAZO", SqlDbType.Bit).Value = p.tipoProducto == "VEHICULAR" && p.autoReemplazo; cmd.Parameters.Add("@C_USUARIO", SqlDbType.VarChar, 50).Value = p.usuarioRegistro ?? "";
        }

        private OfertaCotizacionBE MapearOferta(SqlDataReader dr)
        {
            OfertaCotizacionBE o = new OfertaCotizacionBE();
            o.idOferta = Convert.ToInt64(dr["N_ID_OFERTA"]); o.idCotizacion = Convert.ToInt64(dr["N_ID_COTIZACION"]); o.compania.idCompania = Convert.ToInt32(dr["N_ID_COMPANIA"]); o.compania.codigo = Convert.ToString(dr["C_CODIGO"]); o.compania.nombre = Convert.ToString(dr["C_NOMBRE_COMPANIA"]); o.compania.color = Convert.ToString(dr["C_COLOR"]); o.compania.descripcion = Convert.ToString(dr["C_DESCRIPCION"]); o.primaNeta = Convert.ToDouble(dr["N_PRIMA_NETA"]); o.igv = Convert.ToDouble(dr["N_IGV"]); o.primaTotal = Convert.ToDouble(dr["N_PRIMA_TOTAL"]); o.deducible = Convert.ToDouble(dr["N_DEDUCIBLE"]); o.descripcionDeducible = Convert.ToString(dr["C_DES_DEDUCIBLE"]); o.autoReemplazo = Convert.ToInt32(dr["B_AUTO_REEMPLAZO"]) == 1; o.costoAutoReemplazo = dr["N_COSTO_AUTO_REEMPLAZO"] == DBNull.Value ? 0 : Convert.ToDouble(dr["N_COSTO_AUTO_REEMPLAZO"]); o.beneficios = Convert.ToString(dr["C_BENEFICIOS"]); o.tiempoRespuesta = Convert.ToString(dr["C_TIEMPO_RESPUESTA"]); o.recomendada = Convert.ToInt32(dr["B_RECOMENDADA"]) == 1;
            return o;
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing) { if (!disposed) disposed = true; }
    }
}
