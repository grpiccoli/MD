using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsultaMD.Services
{
    public static class WebpackChunkNamer
    {
        private static Dictionary<string, Dictionary<string, string>> Tags { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public static void Init()
        {
            using (var fs = File.OpenRead("src/stats.json"))
            using (var sr = new StreamReader(fs))
            using (var reader = new JsonTextReader(sr))
            {
                JObject obj = JObject.Load(reader);

                var chunks = obj["assetsByChunkName"];
                foreach (var chunk in chunks)
                {
                    JProperty prop = (JProperty)chunk;
                    if (prop.Value.Type == JTokenType.String)
                    {
                        Tags = AddTag(Tags, prop.Name, prop.Value.ToString());
                    }
                    else if (prop.Value.Type == JTokenType.Array)
                    {
                        foreach (var file in prop.Value)
                        {
                            if(file.Type == JTokenType.String)
                            {
                                Tags = AddTag(Tags, prop.Name, file.ToString());
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<string, Dictionary<string, string>> AddTag(Dictionary<string, Dictionary<string, string>> Tags, string name, string file)
        {
            var ext = file.Split('.').Last();
            if (!Tags.ContainsKey(name))
                Tags[name] = new Dictionary<string, string>();
            Tags[name].Add(ext, file);
            return Tags;
        }

        public static string GetJsFile(string name, string ext)
        {
            return (Tags.ContainsKey(name) && Tags[name].ContainsKey(ext)) ? Tags[name][ext] : string.Empty;
        }
    }
}
