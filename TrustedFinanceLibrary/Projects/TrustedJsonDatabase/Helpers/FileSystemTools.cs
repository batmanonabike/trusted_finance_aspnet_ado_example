namespace TrustedJsonDatabase.Helpers
{
    internal class FileSystemTools
    {
        const string EmptyLibrary = """{ "Books": [] }""";

        // %LOCALAPPDATA%\TrustedLibrary\Json
        public static string GetAppDataDirectory()
        {
            const string DataDirectory = "Json";
            const string ApplicationDirectory = "TrustedLibrary";

            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ApplicationDirectory, DataDirectory);
        }

        // %LOCALAPPDATA%\TrustedLibrary\Json\Library.json
        public static string GetOrCreateJsonLibrary(string directory)
        {
            Directory.CreateDirectory(directory);

            var jsonLibrary = Path.Combine(directory, $"Library.json");
            if (!File.Exists(jsonLibrary))
                File.WriteAllText(jsonLibrary, EmptyLibrary);

             return jsonLibrary;
        }

        // %LOCALAPPDATA%\TrustedLibrary\Json\Library_000.json
        // %LOCALAPPDATA%\TrustedLibrary\Json\Library_001.json
        public static string CreateNumberedJsonLibrary(string directory)
        {
            Directory.CreateDirectory(directory);

            const int MaxSuffix = 999;
            for (var suffix = 0; suffix <= MaxSuffix; suffix++)
            {
                var jsonLibrary = Path.Combine(directory, $"Library_{suffix:D3}.json");
                if (!File.Exists(jsonLibrary))
                {
                    File.WriteAllText(jsonLibrary, EmptyLibrary);
                    return jsonLibrary;
                }
            }

            throw new IOException($"Failed to create file in '{directory}'.");
        }
    }
}
