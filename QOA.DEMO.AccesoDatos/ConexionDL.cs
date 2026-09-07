using System.Configuration;

namespace QOA.DEMO.AccesoDatos
{
    public class ConexionDL
    {
        protected string conexion = ConfigurationManager.ConnectionStrings["QoaDemo"] == null
            ? ""
            : ConfigurationManager.ConnectionStrings["QoaDemo"].ConnectionString;

        protected bool UsarDatosSimulados()
        {
            // El valor por defecto permite mostrar la aplicacion sin SQL Server.
            return ConfigurationManager.AppSettings["UsarDatosSimulados"] != "false";
        }
    }
}
