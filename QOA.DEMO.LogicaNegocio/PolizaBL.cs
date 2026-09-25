using QOA.DEMO.AccesoDatos;
using QOA.DEMO.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace QOA.DEMO.LogicaNegocio
{
    public class PolizaBL : IDisposable
    {
        private Component component = new Component();
        private bool disposed = false;
        PolizaDL dataDL = new PolizaDL();

        public MensajeResultado registrarPoliza(PolizaBE objParametro)
        {
            try
            {
                // Si existe una oferta se respeta su precio; el flujo antiguo sigue recalculando todo.
                if (objParametro.idOfertaCotizacion <= 0 && objParametro.tipoProducto == "TARJETA")
                {
                    if (objParametro.planTarjeta == "PLAN 1") objParametro.primaNeta = 29.90;
                    else if (objParametro.planTarjeta == "PLAN 2") objParametro.primaNeta = 49.90;
                    else objParametro.primaNeta = 69.90;
                    objParametro.descripcionProducto = "Proteccion de Tarjeta";
                    objParametro.coberturas.Add("Compras no reconocidas"); objParametro.coberturas.Add("Robo en cajero");
                }
                else if (objParametro.idOfertaCotizacion <= 0)
                {
                    objParametro.primaNeta = objParametro.valorComercial * 0.028;
                    if (objParametro.usoVehiculo == "TAXI") objParametro.primaNeta = objParametro.primaNeta * 1.45;
                    objParametro.descripcionProducto = "Seguro Vehicular";
                    objParametro.coberturas.Add("Danos propios"); objParametro.coberturas.Add("Responsabilidad civil");
                    if (objParametro.autoReemplazo) objParametro.coberturas.Add("Auto de Reemplazo");
                }
                else
                {
                    objParametro.descripcionProducto = objParametro.tipoProducto == "TARJETA" ? "Proteccion de Tarjeta" : "Seguro Vehicular";
                    objParametro.coberturas.Add(objParametro.tipoProducto == "TARJETA" ? "Compras no reconocidas" : "Danos propios");
                    objParametro.coberturas.Add(objParametro.tipoProducto == "TARJETA" ? "Robo en cajero" : "Responsabilidad civil");
                    if (objParametro.tipoProducto == "VEHICULAR" && objParametro.autoReemplazo) objParametro.coberturas.Add("Auto de Reemplazo");
                }
                if (objParametro.idOfertaCotizacion <= 0)
                {
                    objParametro.igv = objParametro.primaNeta * 0.18;
                    objParametro.primaTotal = objParametro.primaNeta + objParametro.igv;
                }
                return new PolizaDL().registrarPoliza(objParametro);
            }
            catch (Exception ex) { throw; }
        }

        public List<PolizaBE> consultarPolizas(PolizaBE objParametro)
        {
            try { return new PolizaDL().consultarPolizas(objParametro); }
            catch (Exception ex) { throw; }
        }

        public PolizaBE consultarDetalle(long idPoliza)
        {
            try { return new PolizaDL().consultarDetalle(idPoliza); }
            catch (Exception ex) { throw; }
        }

        public MensajeResultado anularPoliza(long idPoliza, string motivo, string usuario)
        {
            try { return new PolizaDL().anularPoliza(idPoliza, motivo, usuario); }
            catch (Exception ex) { throw; }
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing)
        {
            if (!disposed) { if (disposing) component.Dispose(); disposed = true; }
        }
    }
}
