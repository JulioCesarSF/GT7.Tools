using GT7.ScreenParser.Constantes;

namespace GT7.ScreenParser.Extensions
{
    /// <summary>
    /// Dictionary object extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Validates the initial configuration from command liene
        /// </summary>
        /// <param name="keyValues"></param>
        /// <exception cref="Exception"></exception>
        public static void ValidateConfiguration(this Dictionary<string, string> keyValues)
        {
            if (keyValues.Count == 0)
                throw new Exception($"Invalid command line arguments, valid args: " +
                    $"{ConfigurationKeys.ImagePath}{ConfigurationKeys.ConfigurationSplitter}\"image path\", " +
                    $"{ConfigurationKeys.SaveResult}{ConfigurationKeys.ConfigurationSplitter}true|false, " +
                    $"Splitter {ConfigurationKeys.ConfigurationSplitter}");

            if (!keyValues.ContainsKey(ConfigurationKeys.ImagePath)
                || string.IsNullOrEmpty(keyValues[ConfigurationKeys.ImagePath])
                || !File.Exists(keyValues[ConfigurationKeys.ImagePath]))
                throw new Exception($"Inform a valid image path: {ConfigurationKeys.ImagePath}#\"path to image\"");

            if (!keyValues.ContainsKey(ConfigurationKeys.SaveResult))
                keyValues.Add(ConfigurationKeys.SaveResult, "false");
            else if (string.IsNullOrEmpty(keyValues[ConfigurationKeys.SaveResult])
                || string.IsNullOrWhiteSpace(keyValues[ConfigurationKeys.SaveResult]))
                keyValues[ConfigurationKeys.SaveResult] = "false";

            if (!keyValues.ContainsKey(ConfigurationKeys.ImageType))
                keyValues.Add(ConfigurationKeys.ImageType, "drsr");
            else if (string.IsNullOrEmpty(keyValues[ConfigurationKeys.ImageType])
                || string.IsNullOrWhiteSpace(keyValues[ConfigurationKeys.ImageType]))
                keyValues[ConfigurationKeys.ImageType] = "drsr";
        }
    }
}
