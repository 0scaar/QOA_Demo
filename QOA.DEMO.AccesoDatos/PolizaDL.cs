using QOA.DEMO.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace QOA.DEMO.AccesoDatos
{
    public class PolizaDL : ConexionDL, IDisposable
    {
        private bool disposed = false;

        public MensajeResultado registrarPoliza(PolizaBE objParametro)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    long id = 0;
                    // MAX calculado recorriendo todo en vez de usar una secuencia/IDENTITY.
                    foreach (PolizaBE x in DatosDemo.polizas) { Thread.Sleep(60); if (x.idPoliza > id) id = x.idPoliza; }
                    objParametro.idPoliza = id + 1;
                    objParametro.numPoliza = (objParametro.tipoProducto == "TARJETA" ? "PCT" : "VEH") + "-" + DateTime.Now.Year + "-" + objParametro.idPoliza.ToString("000000");
                    objParametro.fecEmision = DateTime.Now.ToString("dd/MM/yyyy"); objParametro.fechaRegistro = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); objParametro.estado = "EMITIDA"; objParametro.activo = 1;
                    if (objParametro.idOfertaCotizacion <= 0) { objParametro.igv = objParametro.primaNeta * 0.18; objParametro.primaTotal = objParametro.primaNeta + objParametro.igv; }
                    DatosDemo.polizas.Add(objParametro);
                    return new MensajeResultado { CODIGO = 0, MENSAJE = "Poliza emitida correctamente", ID = objParametro.idPoliza, EXITO = true, DATA = objParametro };
                }

                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPI_POLIZA_EMITIR", conex); cmd.CommandType = CommandType.StoredProcedure;
                    AgregarParametrosPoliza(cmd, objParametro);
                    SqlParameter id = cmd.Parameters.Add("@PO_ID_POLIZA", SqlDbType.BigInt); id.Direction = ParameterDirection.Output;
                    conex.Open(); cmd.ExecuteNonQuery();
                    return new MensajeResultado { CODIGO = 0, MENSAJE = "Poliza emitida correctamente", ID = Convert.ToInt64(id.Value), EXITO = true };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PolizaBE> consultarPolizas(PolizaBE objParametro)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    List<PolizaBE> temporal = new List<PolizaBE>();
                    // Copias y filtros en memoria, sin paginacion y con espera por fila.
                    foreach (PolizaBE item in DatosDemo.polizas.ToList())
                    {
                        Thread.Sleep(80);
                        string texto = ((item.numPoliza ?? "") + " " + (item.contratante.nombreCompleto ?? "") + " " + (item.contratante.numeroDocumento ?? "")).ToLower();
                        if (String.IsNullOrEmpty(objParametro.filtroGeneral) || texto.Contains(objParametro.filtroGeneral.ToLower())) temporal.Add(item);
                    }
                    List<PolizaBE> resultado = new List<PolizaBE>();
                    foreach (PolizaBE p in temporal.OrderByDescending(x => x.fechaRegistro).ToList()) resultado.Add(p);
                    return resultado;
                }

                List<PolizaBE> lista = new List<PolizaBE>();
                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPS_POLIZA_CONSULTAR", conex); cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@C_FILTRO", SqlDbType.VarChar, 100).Value = objParametro.filtroGeneral ?? "";
                    conex.Open(); SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read()) lista.Add(Mapear(dr));
                }
                return lista;
            }
            catch (Exception ex) { throw; }
        }

        public PolizaBE consultarDetalle(long idPoliza)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    // N+1 simulado: vuelve a consultar toda la bandeja antes de buscar el detalle.
                    List<PolizaBE> todos = consultarPolizas(new PolizaBE());
                    foreach (PolizaBE item in todos) { Thread.Sleep(70); if (item.idPoliza == idPoliza) return item; }
                    return null;
                }
                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPS_POLIZA_DETALLE", conex); cmd.CommandType = CommandType.StoredProcedure; cmd.Parameters.Add("@N_ID_POLIZA", SqlDbType.BigInt).Value = idPoliza;
                    conex.Open(); SqlDataReader dr = cmd.ExecuteReader();
                    if (!dr.Read()) return null;
                    PolizaBE p = Mapear(dr);
                    p.descripcionProducto = Convert.ToString(dr["C_DES_PRODUCTO"]); p.fecIniVigencia = Convert.ToString(dr["D_INI_VIGENCIA"]); p.fecFinVigencia = Convert.ToString(dr["D_FIN_VIGENCIA"]); p.fechaAnulacion = Convert.ToString(dr["D_FEC_ANULACION"]); p.motivoAnulacion = Convert.ToString(dr["C_MOTIVO_ANULACION"]);
                    p.banco = Convert.ToString(dr["C_BANCO"]); p.tipoTarjeta = Convert.ToString(dr["C_TIPO_TARJETA"]); p.ultimosDigitosTarjeta = Convert.ToString(dr["C_ULTIMOS_DIGITOS"]); p.planTarjeta = Convert.ToString(dr["C_PLAN"]); if (dr["N_LINEA_CREDITO"] != DBNull.Value) p.lineaCredito = Convert.ToDouble(dr["N_LINEA_CREDITO"]);
                    p.placa = Convert.ToString(dr["C_PLACA"]); p.marca = Convert.ToString(dr["C_MARCA"]); p.modelo = Convert.ToString(dr["C_MODELO"]); p.numeroMotor = Convert.ToString(dr["C_NUM_MOTOR"]); p.numeroSerie = Convert.ToString(dr["C_NUM_SERIE"]); p.usoVehiculo = Convert.ToString(dr["C_USO"]); if (dr["N_ANIO"] != DBNull.Value) p.anioFabricacion = Convert.ToInt32(dr["N_ANIO"]); if (dr["N_VALOR_COMERCIAL"] != DBNull.Value) p.valorComercial = Convert.ToDouble(dr["N_VALOR_COMERCIAL"]);
                    if (dr.NextResult()) while (dr.Read()) p.coberturas.Add(Convert.ToString(dr["C_NOMBRE"]));
                    return p;
                }
            }
            catch (Exception ex) { throw; }
        }

        public MensajeResultado anularPoliza(long idPoliza, string motivo, string usuario)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    PolizaBE poliza = consultarDetalle(idPoliza);
                    if (poliza == null) return new MensajeResultado { CODIGO = 1, MENSAJE = "Poliza no encontrada", EXITO = false };
                    if (poliza.estado == "ANULADA") return new MensajeResultado { CODIGO = 2, MENSAJE = "La poliza ya esta anulada", EXITO = false };
                    Thread.Sleep(400); poliza.estado = "ANULADA"; poliza.fechaAnulacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); poliza.motivoAnulacion = motivo;
                    return new MensajeResultado { CODIGO = 0, MENSAJE = "Poliza anulada", ID = idPoliza, EXITO = true };
                }
                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPU_POLIZA_ANULAR", conex); cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@N_ID_POLIZA", SqlDbType.BigInt).Value = idPoliza; cmd.Parameters.Add("@C_MOTIVO", SqlDbType.VarChar, 500).Value = motivo ?? ""; cmd.Parameters.Add("@C_USUARIO", SqlDbType.VarChar, 50).Value = usuario ?? "";
                    conex.Open(); cmd.ExecuteNonQuery(); return new MensajeResultado { CODIGO = 0, MENSAJE = "Poliza anulada", ID = idPoliza, EXITO = true };
                }
            }
            catch (Exception ex) { throw; }
        }

        private void AgregarParametrosPoliza(SqlCommand cmd, PolizaBE p)
        {
            cmd.Parameters.Add("@C_TIPO_PRODUCTO", SqlDbType.VarChar, 20).Value = p.tipoProducto ?? ""; cmd.Parameters.Add("@C_DOCUMENTO", SqlDbType.VarChar, 20).Value = p.contratante.numeroDocumento ?? ""; cmd.Parameters.Add("@C_NOMBRE", SqlDbType.VarChar, 200).Value = p.contratante.nombreCompleto ?? p.contratante.nombres ?? "";
            cmd.Parameters.Add("@D_INI_VIGENCIA", SqlDbType.Date).Value = Convert.ToDateTime(p.fecIniVigencia); cmd.Parameters.Add("@D_FIN_VIGENCIA", SqlDbType.Date).Value = Convert.ToDateTime(p.fecFinVigencia); cmd.Parameters.Add("@N_PRIMA_NETA", SqlDbType.Decimal).Value = p.primaNeta;
            cmd.Parameters.Add("@C_BANCO", SqlDbType.VarChar, 100).Value = p.banco ?? ""; cmd.Parameters.Add("@C_TIPO_TARJETA", SqlDbType.VarChar, 50).Value = p.tipoTarjeta ?? ""; cmd.Parameters.Add("@C_ULTIMOS_DIGITOS", SqlDbType.Char, 4).Value = p.ultimosDigitosTarjeta ?? ""; cmd.Parameters.Add("@N_LINEA_CREDITO", SqlDbType.Decimal).Value = p.lineaCredito; cmd.Parameters.Add("@C_PLAN", SqlDbType.VarChar, 50).Value = p.planTarjeta ?? "";
            cmd.Parameters.Add("@C_PLACA", SqlDbType.VarChar, 10).Value = p.placa ?? ""; cmd.Parameters.Add("@C_MARCA", SqlDbType.VarChar, 50).Value = p.marca ?? ""; cmd.Parameters.Add("@C_MODELO", SqlDbType.VarChar, 50).Value = p.modelo ?? ""; cmd.Parameters.Add("@N_ANIO", SqlDbType.Int).Value = p.anioFabricacion; cmd.Parameters.Add("@C_NUM_MOTOR", SqlDbType.VarChar, 80).Value = p.numeroMotor ?? ""; cmd.Parameters.Add("@C_NUM_SERIE", SqlDbType.VarChar, 80).Value = p.numeroSerie ?? ""; cmd.Parameters.Add("@N_VALOR_COMERCIAL", SqlDbType.Decimal).Value = p.valorComercial; cmd.Parameters.Add("@C_USO", SqlDbType.VarChar, 30).Value = p.usoVehiculo ?? ""; cmd.Parameters.Add("@C_USUARIO", SqlDbType.VarChar, 50).Value = p.usuarioRegistro ?? "";
            cmd.Parameters.Add("@N_ID_COTIZACION", SqlDbType.BigInt).Value = p.idCotizacion; cmd.Parameters.Add("@N_ID_OFERTA", SqlDbType.BigInt).Value = p.idOfertaCotizacion; cmd.Parameters.Add("@N_ID_COMPANIA", SqlDbType.Int).Value = p.idCompania; cmd.Parameters.Add("@C_NOMBRE_COMPANIA", SqlDbType.VarChar, 150).Value = p.nombreCompania ?? ""; cmd.Parameters.Add("@C_NUM_COTIZACION", SqlDbType.VarChar, 30).Value = p.numCotizacion ?? ""; cmd.Parameters.Add("@N_DEDUCIBLE", SqlDbType.Decimal).Value = p.deducible;
        }

        private PolizaBE Mapear(SqlDataReader dr)
        {
            PolizaBE p = new PolizaBE();
            p.idPoliza = Convert.ToInt64(dr["N_ID_POLIZA"]); p.numPoliza = Convert.ToString(dr["C_NUM_POLIZA"]); p.tipoProducto = Convert.ToString(dr["C_TIPO_PRODUCTO"]); p.descripcionProducto = p.tipoProducto == "TARJETA" ? "Proteccion de Tarjeta" : "Seguro Vehicular"; p.estado = Convert.ToString(dr["C_ESTADO"]); p.fecEmision = Convert.ToString(dr["D_FEC_EMISION"]); p.primaNeta = Convert.ToDouble(dr["N_PRIMA_NETA"]); p.primaTotal = Convert.ToDouble(dr["N_PRIMA_TOTAL"]); p.igv = p.primaTotal - p.primaNeta; p.contratante.numeroDocumento = Convert.ToString(dr["C_DOCUMENTO"]); p.contratante.nombreCompleto = Convert.ToString(dr["C_NOMBRE_COMPLETO"]);
            if (ExisteColumna(dr, "N_ID_COTIZACION") && dr["N_ID_COTIZACION"] != DBNull.Value) p.idCotizacion = Convert.ToInt64(dr["N_ID_COTIZACION"]); if (ExisteColumna(dr, "N_ID_OFERTA") && dr["N_ID_OFERTA"] != DBNull.Value) p.idOfertaCotizacion = Convert.ToInt64(dr["N_ID_OFERTA"]); if (ExisteColumna(dr, "N_ID_COMPANIA") && dr["N_ID_COMPANIA"] != DBNull.Value) p.idCompania = Convert.ToInt32(dr["N_ID_COMPANIA"]); if (ExisteColumna(dr, "C_NOMBRE_COMPANIA")) p.nombreCompania = Convert.ToString(dr["C_NOMBRE_COMPANIA"]); if (ExisteColumna(dr, "C_NUM_COTIZACION")) p.numCotizacion = Convert.ToString(dr["C_NUM_COTIZACION"]); if (ExisteColumna(dr, "N_DEDUCIBLE") && dr["N_DEDUCIBLE"] != DBNull.Value) p.deducible = Convert.ToDouble(dr["N_DEDUCIBLE"]);
            return p;
        }

        private bool ExisteColumna(SqlDataReader dr, string columna)
        {
            for (int i = 0; i < dr.FieldCount; i++) if (dr.GetName(i).Equals(columna, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing) { if (!disposed) disposed = true; }
    }
}
