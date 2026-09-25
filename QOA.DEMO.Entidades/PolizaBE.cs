using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace QOA.DEMO.Entidades
{
    // Esta clase concentra intencionalmente campos de ambos productos, consulta, auditoria y UI.
    public class PolizaBE
    {
        public long idPoliza { get; set; }
        public string numPoliza { get; set; }
        public string tipoProducto { get; set; }
        public string descripcionProducto { get; set; }
        public PersonaBE contratante { get; set; }
        public PersonaBE asegurado { get; set; }
        public String fecEmision { get; set; }
        public String fecIniVigencia { get; set; }
        public String fecFinVigencia { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public Double primaNeta { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public Double igv { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public Double primaTotal { get; set; }
        public string moneda { get; set; }
        public string estado { get; set; }
        public int activo { get; set; }
        public string observacion { get; set; }
        public string usuarioRegistro { get; set; }
        public String fechaRegistro { get; set; }
        public String fechaAnulacion { get; set; }
        public string motivoAnulacion { get; set; }

        // Datos de cotizacion mezclados intencionalmente con la entidad de poliza.
        public long idCotizacion { get; set; }
        public long idOfertaCotizacion { get; set; }
        public int idCompania { get; set; }
        public string numCotizacion { get; set; }
        public string nombreCompania { get; set; }
        public Double deducible { get; set; }

        // Proteccion de tarjeta.
        public string banco { get; set; }
        public string tipoTarjeta { get; set; }
        public string ultimosDigitosTarjeta { get; set; }
        public Double lineaCredito { get; set; }
        public string planTarjeta { get; set; }

        // Vehicular.
        public string placa { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public int anioFabricacion { get; set; }
        public string numeroMotor { get; set; }
        public string numeroSerie { get; set; }
        public Double valorComercial { get; set; }
        public string usoVehiculo { get; set; }
        public bool autoReemplazo { get; set; }
        public Double costoAutoReemplazo { get; set; }

        // Campos de pantalla y de acceso a datos dentro de la misma entidad.
        public string filtroGeneral { get; set; }
        public string pagina { get; set; }
        public string mensaje { get; set; }
        public int codError { get; set; }
        public DataTable TablaCoberturas { get; set; }
        public List<string> coberturas { get; set; }

        public PolizaBE()
        {
            contratante = new PersonaBE();
            asegurado = new PersonaBE();
            coberturas = new List<string>();
            TablaCoberturas = new DataTable();
            moneda = "PEN";
            estado = "EMITIDA";
            activo = 1;
        }
    }
}
