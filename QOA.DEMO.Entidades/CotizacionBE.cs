using System;
using System.Collections.Generic;

namespace QOA.DEMO.Entidades
{
    public class CotizacionBE
    {
        public long idCotizacion { get; set; }
        public string numCotizacion { get; set; }
        public string tipoProducto { get; set; }
        public PolizaBE poliza { get; set; }
        public List<OfertaCotizacionBE> ofertas { get; set; }
        public string estado { get; set; }
        public string fechaCotizacion { get; set; }
        public string usuarioRegistro { get; set; }
        public string mensaje { get; set; }

        public CotizacionBE()
        {
            poliza = new PolizaBE();
            ofertas = new List<OfertaCotizacionBE>();
            estado = "COTIZADA";
        }
    }
}
