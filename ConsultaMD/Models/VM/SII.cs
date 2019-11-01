using System.Collections.Generic;

namespace ConsultaMD.Models.VM
{
    public class SII: Indexed
    {
        public string Rut { get; set; }
        public string Razon_Social { get; set; }
        public IEnumerable<Actividad> Actividades { get; } = new List<Actividad>();
    }
    public class Actividad
    {
        public string Giro { get; set; }
        public int Codigo { get; set; }
        public string Categoria { get; set; }
        public bool Afecta { get; set; }
    }
}
