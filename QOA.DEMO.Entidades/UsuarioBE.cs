using System;

namespace QOA.DEMO.Entidades
{
    public class UsuarioBE
    {
        public int idUsuario { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public string nombreCompleto { get; set; }
        public string perfil { get; set; }
        public string correo { get; set; }
        public int activo { get; set; }
        public string accion { get; set; }
        public String fechaUltimoAcceso { get; set; }
        public PersonaBE persona { get; set; }

        public UsuarioBE()
        {
            persona = new PersonaBE();
        }
    }
}
