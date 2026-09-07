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
                // Reglas de productos mezcladas, constantes magicas y calculos duplicados.
                if (objParametro.tipoProducto == "TARJETA")
                {
                    if (objParametro.planTarjeta == "PLAN 1") objParametro.primaNeta = 29.90;
                    else if (objParametro.planTarjeta == "PLAN 2") objParametro.primaNeta = 49.90;
                    else objParametro.primaNeta = 69.90;
                    objParametro.descripcionProducto = "Proteccion de Tarjeta";
                    objParametro.coberturas.Add("Compras no reconocidas"); objParametro.coberturas.Add("Robo en cajero");
                }
                else
                {
                    objParametro.primaNeta = objParametro.valorComercial * 0.028;
                    if (objParametro.usoVehiculo == "TAXI") objParametro.primaNeta = objParametro.primaNeta * 1.45;
                    objParametro.descripcionProducto = "Seguro Vehicular";
                    objParametro.coberturas.Add("Danos propios"); objParametro.coberturas.Add("Responsabilidad civil");
                }
                objParametro.igv = objParametro.primaNeta * 0.18;
                objParametro.primaTotal = objParametro.primaNeta + objParametro.igv;
                return new PolizaDL().registrarPoliza(objParametro);
            }
            catch (Exception ex) { throw ex; }
        }

        public List<PolizaBE> consultarPolizas(PolizaBE objParametro)
        {
            try { return new PolizaDL().consultarPolizas(objParametro); }
            catch (Exception ex) { throw ex; }
        }

        public PolizaBE consultarDetalle(long idPoliza)
        {
            try { return new PolizaDL().consultarDetalle(idPoliza); }
            catch (Exception ex) { throw ex; }
        }

        public MensajeResultado anularPoliza(long idPoliza, string motivo, string usuario)
        {
            try { return new PolizaDL().anularPoliza(idPoliza, motivo, usuario); }
            catch (Exception ex) { throw ex; }
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing)
        {
            if (!disposed) { if (disposing) component.Dispose(); disposed = true; }
        }
    }
}
