using System;
using System.IO;
using Utf8Json;

namespace Itamaram.Excel.SpecFlowPlugin
{
    public class JsonConfigFactory
    {
        public JsonConfig GetConfig()
        {
            string jsonConfigFilePath = GetItamaramJsonConfigFilePath("ItamaramExcelConfig.json");
            if (string.IsNullOrEmpty(jsonConfigFilePath)) return null;
            string jsonContent = File.ReadAllText(jsonConfigFilePath);
            JsonConfig myJsonConfig = JsonSerializer.Deserialize<JsonConfig>(jsonContent);
            return myJsonConfig;
        }

        public string GetItamaramJsonConfigFilePath(string JsonConfigurationFileName)
        {
            var specflowJsonFileInAppDomainBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonConfigurationFileName);

            if (File.Exists(specflowJsonFileInAppDomainBaseDirectory))
            {
                return specflowJsonFileInAppDomainBaseDirectory;
            }

            var specflowJsonFileTwoDirectoriesUp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", JsonConfigurationFileName);

            if (File.Exists(specflowJsonFileTwoDirectoriesUp))
            {
                return specflowJsonFileTwoDirectoriesUp;
            }

            var specflowJsonFileInCurrentDirectory = Path.Combine(Environment.CurrentDirectory, JsonConfigurationFileName);

            if (File.Exists(specflowJsonFileInCurrentDirectory))
            {
                return specflowJsonFileInCurrentDirectory;
            }

            return null;
        }
    }
}