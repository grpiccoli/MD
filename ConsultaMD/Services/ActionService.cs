using ConsultaMD.Models.VM.PatientsVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsultaMD.Services
{
    public static class Actions
    {
        private static IEnumerable<ActionConfig> Action { get; set; } = new List<ActionConfig>();
        private static IEnumerable<ActionConfig> MenuActions { get; set; } = new List<ActionConfig>();
        public static IEnumerable<ActionConfig> LoadJson()
        {
            using (StreamReader r = new StreamReader("actions.json"))
            {
                string json = r.ReadToEnd();
                Action = JsonConvert.DeserializeObject<List<ActionConfig>>(json);
                MenuActions = Action.Where(m => m.Menu);
                return Action;
            }
        }
        public static IEnumerable<ActionConfig> GetActionsByController(string name)
        {
            return Action.Where(m => m.Controller == name);
        }
        public static IEnumerable<ActionConfig> GetMenuActions()
        {
            return MenuActions;
        }
        public static ActionConfig GetActionByName(string name) {
            return Action.SingleOrDefault(m => m.Name == name);
        }
        public static ActionConfig GetActionByUrl(Uri returl)
        {
            var fullUrl = returl?.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?', StringComparison.InvariantCulture);
            string queryString = null;
            string url = fullUrl;
            if (questionMarkIndex != -1) // There is a QueryString
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }
            // Arranges
            var splitted = url.Split("/");
            var parsed = int.TryParse(splitted.Last(), out int result);
            var name = parsed ? splitted.SkipLast(1).Last() : splitted.Last();
            return Action.SingleOrDefault(a => a.Name == name);
        }
    }
}
