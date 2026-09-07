using QOA.DEMO.Entidades;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace QOA.DEMO.AccesoDatos
{
    public class UsuarioDL : ConexionDL
    {
        public UsuarioBE loginUsuario(UsuarioBE objParametro)
        {
            try
            {
                if (UsarDatosSimulados())
                {
                    UsuarioBE resultado = null;
                    // Busqueda lineal y espera artificial para hacer visible el problema de rendimiento.
                    foreach (UsuarioBE item in DatosDemo.usuarios)
                    {
                        Thread.Sleep(150);
                        if (item.usuario.ToLower().Trim() == (objParametro.usuario ?? "").ToLower().Trim() && item.clave == objParametro.clave && item.activo == 1)
                            resultado = item;
                    }
                    return resultado;
                }

                using (SqlConnection conex = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SPS_SEG_LOGIN", conex);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@C_USUARIO", SqlDbType.VarChar, 50).Value = objParametro.usuario ?? "";
                    cmd.Parameters.Add("@C_CLAVE", SqlDbType.VarChar, 100).Value = objParametro.clave ?? "";
                    conex.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        UsuarioBE u = new UsuarioBE();
                        u.idUsuario = Convert.ToInt32(dr["N_ID_USUARIO"]); u.usuario = Convert.ToString(dr["C_USUARIO"]); u.nombreCompleto = Convert.ToString(dr["C_NOMBRE_COMPLETO"]); u.perfil = Convert.ToString(dr["C_PERFIL"]); u.activo = 1;
                        return u;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
