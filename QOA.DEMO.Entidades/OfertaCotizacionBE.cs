namespace QOA.DEMO.Entidades
{
    public class OfertaCotizacionBE
    {
        public long idOferta { get; set; }
        public long idCotizacion { get; set; }
        public CompaniaBE compania { get; set; }
        public double primaNeta { get; set; }
        public double igv { get; set; }
        public double primaTotal { get; set; }
        public double deducible { get; set; }
        public string descripcionDeducible { get; set; }
        public string beneficios { get; set; }
        public string tiempoRespuesta { get; set; }
        public bool seleccionada { get; set; }
        public bool recomendada { get; set; }
        public int orden { get; set; }

        public OfertaCotizacionBE()
        {
            compania = new CompaniaBE();
        }
    }
}
