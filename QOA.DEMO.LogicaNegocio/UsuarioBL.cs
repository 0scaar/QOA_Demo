using QOA.DEMO.AccesoDatos;
using QOA.DEMO.Entidades;
using System;
using System.ComponentModel;

namespace QOA.DEMO.LogicaNegocio
{
    public class UsuarioBL : IDisposable
    {
        private Component component = new Component();
        private bool disposed = false;
        UsuarioDL dataDL = new UsuarioDL();

        public UsuarioBE loginUsuario(UsuarioBE objParametro)
        {
            try
            {
                // Se crea otra instancia aunque ya existe dataDL.
                return new UsuarioDL().loginUsuario(objParametro);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing)
        {
            if (!disposed) { if (disposing) component.Dispose(); disposed = true; }
        }
    }
}
