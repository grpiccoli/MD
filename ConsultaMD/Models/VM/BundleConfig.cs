using System.Collections.Generic;

namespace ConsultaMD.Models.VM
{
    public class BundleConfig
    {
        public string OutputFileName { get; set; }
        public List<string> InputFiles { get; } = new List<string>();
    }
}
