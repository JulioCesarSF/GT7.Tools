using GT7.ScreenParser.Constantes;

namespace GT7.ScreenParser.Extensions
{
    /// <summary>
    /// Array object extensions
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// For a given array transform into a dictionary
        /// </summary>
        /// <param name="args"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetConfigurationFromArgs(this string[] args, char splitter = ConfigurationKeys.ConfigurationSplitter)
        {
            var configuration = new Dictionary<string, string>();

            Array.ForEach(args, (currentValue) =>
            {
                var split = currentValue.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                //discard keys without values
                if (split.Length == 2)
                {
                    var key = split[0];
                    var value = split[1];
                    if (!configuration.ContainsKey(key))
                        configuration.Add(key, value);
                }
            });

            return configuration;
        }
    }
}
