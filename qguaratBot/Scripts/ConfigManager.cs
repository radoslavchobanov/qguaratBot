using Newtonsoft.Json;

namespace qguaratBot
{
    public static class ConfigManager
    {
        private static string ConfigFolder = "Config";
        private static string ConfigFile = "config.json";
        private static string ConfigPath = ConfigFolder + "/" + ConfigFile;

        public static BotConfig Config { get; private set; }

        static ConfigManager()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Console.Log(Console.LogLevel.WARNING, "Config folder does not exist ... CREATING DIRECTORY !!!");

                Directory.CreateDirectory(ConfigFolder);
            }

            if (!File.Exists(ConfigPath))
            {
                Console.Log(Console.LogLevel.WARNING, $"{ConfigPath}  does not exist, CREATING FILE !!!");

                Config = new BotConfig();
                var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigPath);

                if (json == null)
                {
                    Console.Log(Console.LogLevel.ERROR, $"Could not read data from {ConfigFile} !!!");
                    return;
                }

                Config = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("caseSensitive")]
        public bool CaseSensitive { get; private set; }

        // [JsonProperty("minimumLogLevel")]
        // public LogLevel MinimumLogLevel { get; private set; }

    }
}