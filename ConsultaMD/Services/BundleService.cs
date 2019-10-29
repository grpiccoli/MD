using ConsultaMD.Models.VM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsultaMD.Services
{
    public static class Bundler
    {
        private static List<Bundle> Bundles { get; } = new List<Bundle>();
        public static List<Bundle> LoadJson()
        {
            using (StreamReader r = new StreamReader("bundleconfig.json"))
            {
                string json = r.ReadToEnd();
                Bundles.Clear();
                Bundles.AddRange(JsonConvert.DeserializeObject<List<Bundle>>(json));
                return Bundles;
            }
        }
        public static IEnumerable<Bundle> GetBundles(string lib) {
            return Bundles.Where(m => m.OutputFileName.Contains(lib, StringComparison.InvariantCulture));
        }
    }
}
