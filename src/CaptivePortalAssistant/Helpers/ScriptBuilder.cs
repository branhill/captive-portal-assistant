using System.IO;
using System.Linq;
using System.Reflection;
using CaptivePortalAssistant.Models;
using Newtonsoft.Json;

namespace CaptivePortalAssistant.Helpers
{
    public static class ScriptBuilder
    {
        public static string GetFillScript(Profile profile, bool isLoginEnabled)
        {
            const string fillScriptName = "Fill.js";

            var fieldsJson = JsonConvert.SerializeObject(profile.Fields, JsonStorage.GetJsonSerializerSettings());
            var script = GetEmbeddedScript(fillScriptName)
                .Replace("{ fieldsJson }", fieldsJson)
                .Replace("{ isLoginEnabled }", isLoginEnabled.ToString().ToLower())
                .Replace("{ submitButton }", profile.SubmitButton);

            return script;
        }

        public static string GetSaveScript()
        {
            const string saveScriptName = "Save.js";
            return GetEmbeddedScript(saveScriptName);
        }

        private static string GetEmbeddedScript(string name)
        {
            var assembly = typeof(ScriptBuilder).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().Single(x => x.EndsWith(name));
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
