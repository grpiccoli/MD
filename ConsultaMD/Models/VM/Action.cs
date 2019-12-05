using System.Collections.Generic;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class ActionConfig
    {
        public string Name { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool Menu { get; set; }
        public bool Material { get; set; }
        public string Limit { get; set; }
        public IEnumerable<string> Path { get; } = new List<string>();
    }
}
