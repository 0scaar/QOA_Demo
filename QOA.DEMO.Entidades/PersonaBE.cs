using System;

namespace QOA.DEMO.Entidades
{
    // Entidad deliberadamente anemica. Mezcla datos de persona, documento y contacto.
    public class PersonaBE
    {
        public long idPersona { get; set; }
        public string tipoDocumento { get; set; }
        public string numeroDocumento { get; set; }
        public string nombres { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombreCompleto { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string departamento { get; set; }
        public string provincia { get; set; }
        public string distrito { get; set; }
        public String fechaNacimiento { get; set; }
        public int activo { get; set; }
    }
}
