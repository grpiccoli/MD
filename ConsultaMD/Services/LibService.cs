using ConsultaMD.Models.VM;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace ConsultaMD.Services
{
    public static class Libman
    {
        private static Libs Libs { get; set; } = new Libs();

        public static Libs LoadJson()
        {
            using (StreamReader r = new StreamReader("libman.json"))
            {
                string json = r.ReadToEnd();
                Libs = JsonConvert.DeserializeObject<Libs>(json);
                return Libs;
            }
        }

        public static Lib GetLibs(string lib)
        {
            var libs = Libs.libraries.SingleOrDefault(m => m.library.StartsWith($"{lib}@"));
            return libs;
        }
    }
}
