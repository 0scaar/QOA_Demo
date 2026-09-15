using QOA.DEMO.AccesoDatos;
using QOA.DEMO.Entidades;
using System;
using System.ComponentModel;

namespace QOA.DEMO.LogicaNegocio
{
    public class CotizacionBL : IDisposable
    {
        private Component component = new Component();
        private bool disposed = false;
        CotizacionDL dataDL = new CotizacionDL();

        public CotizacionBE cotizarPoliza(PolizaBE objParametro)
        {
            try
            {
                if (String.IsNullOrEmpty(objParametro.contratante.numeroDocumento)) throw new Exception("Debe ingresar el documento del contratante");
                if (String.IsNullOrEmpty(objParametro.contratante.nombreCompleto)) throw new Exception("Debe ingresar el nombre del contratante");
                if (objParametro.tipoProducto == "VEHICULAR" && objParametro.valorComercial <= 0) throw new Exception("El valor comercial debe ser mayor a cero");
                return new CotizacionDL().cotizar(objParametro);
            }
            catch (Exception ex) { throw; }
        }

        public MensajeResultado emitirCotizacion(CotizacionBE cotizacion, long idOferta, string usuario)
        {
            try
            {
                MensajeResultado seleccion = new CotizacionDL().seleccionarOferta(cotizacion.idCotizacion, idOferta, usuario);
                if (!seleccion.EXITO) return seleccion;

                OfertaCotizacionBE elegida = null;
                foreach (OfertaCotizacionBE item in cotizacion.ofertas)
                {
                    if (item.idOferta == idOferta) elegida = item;
                }
                if (elegida == null) return new MensajeResultado { CODIGO = 3, MENSAJE = "No fue posible recuperar la oferta", EXITO = false };

                cotizacion.poliza.idCotizacion = cotizacion.idCotizacion;
                cotizacion.poliza.idOfertaCotizacion = elegida.idOferta;
                cotizacion.poliza.idCompania = elegida.compania.idCompania;
                cotizacion.poliza.numCotizacion = cotizacion.numCotizacion;
                cotizacion.poliza.nombreCompania = elegida.compania.nombre;
                cotizacion.poliza.primaNeta = elegida.primaNeta;
                cotizacion.poliza.igv = elegida.igv;
                cotizacion.poliza.primaTotal = elegida.primaTotal;
                cotizacion.poliza.deducible = elegida.deducible;
                cotizacion.poliza.usuarioRegistro = usuario;
                return new PolizaBL().registrarPoliza(cotizacion.poliza);
            }
            catch (Exception ex) { throw; }
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing) { if (!disposed) { if (disposing) component.Dispose(); disposed = true; } }
    }
}
