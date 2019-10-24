using System.Collections.Generic;

namespace ConsultaMD.Models.VM
{
    public class Bundle
    {
        public string OutputFileName { get; set; }
        public List<string> InputFiles { get; } = new List<string>();
    }
}
