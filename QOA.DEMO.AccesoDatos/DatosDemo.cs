using QOA.DEMO.Entidades;
using System;
using System.Collections.Generic;

namespace QOA.DEMO.AccesoDatos
{
    // Estado global, mutable y sin sincronizacion: replica una decision tipica del sistema legado.
    internal static class DatosDemo
    {
        public static List<UsuarioBE> usuarios = new List<UsuarioBE>();
        public static List<PolizaBE> polizas = new List<PolizaBE>();

        static DatosDemo()
        {
            usuarios.Add(new UsuarioBE { idUsuario = 1, usuario = "admin", clave = "admin", nombreCompleto = "Administrador Demo", perfil = "EMISOR", correo = "admin@demo.local", activo = 1 });

            polizas.Add(CrearPoliza(1, "PCT-2026-000001", "TARJETA", "Mariana Torres", "45871234", 39.90, "EMITIDA", "BCP", "Visa", "4587", "", "", ""));
            polizas.Add(CrearPoliza(2, "VEH-2026-000002", "VEHICULAR", "Carlos Paredes", "70112233", 1280.50, "EMITIDA", "", "", "", "ABC-123", "Toyota", "Corolla"));
            polizas.Add(CrearPoliza(3, "PCT-2026-000003", "TARJETA", "Rosa Huaman", "46995511", 54.90, "ANULADA", "Interbank", "Mastercard", "9901", "", "", ""));
        }

        private static PolizaBE CrearPoliza(long id, string numero, string producto, string nombre, string documento, double prima, string estado, string banco, string tipoTarjeta, string digitos, string placa, string marca, string modelo)
        {
            PolizaBE p = new PolizaBE();
            p.idPoliza = id; p.numPoliza = numero; p.tipoProducto = producto; p.descripcionProducto = producto == "TARJETA" ? "Proteccion de Tarjeta" : "Seguro Vehicular";
            p.contratante.numeroDocumento = documento; p.contratante.nombreCompleto = nombre; p.contratante.nombres = nombre;
            p.asegurado.numeroDocumento = documento; p.asegurado.nombreCompleto = nombre;
            p.fecEmision = DateTime.Today.AddDays(-id).ToString("dd/MM/yyyy"); p.fecIniVigencia = DateTime.Today.ToString("dd/MM/yyyy"); p.fecFinVigencia = DateTime.Today.AddYears(1).ToString("dd/MM/yyyy");
            p.primaNeta = prima; p.igv = prima * 0.18; p.primaTotal = p.primaNeta + p.igv; p.estado = estado; p.banco = banco; p.tipoTarjeta = tipoTarjeta; p.ultimosDigitosTarjeta = digitos;
            p.placa = placa; p.marca = marca; p.modelo = modelo; p.anioFabricacion = 2022; p.valorComercial = 55000; p.usuarioRegistro = "admin"; p.fechaRegistro = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            p.coberturas.Add(producto == "TARJETA" ? "Compras no reconocidas" : "Danos propios"); p.coberturas.Add(producto == "TARJETA" ? "Robo en cajero" : "Responsabilidad civil");
            return p;
        }
    }
}
