using System.Text.Json;
using TrustedAbstractions;

namespace TrustedJsonDatabase.Helpers
{
    internal class JsonContent
    {
        public List<Book> Books { get; set; } = [];
    }

    internal class JsonStore(string path)
    {
        private readonly Lock _lock = new();

        private static readonly JsonSerializerOptions Options = new() 
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        public T Query<T>(Func<JsonContent, T> query)
        {
            lock (_lock)
            {
                var content = Load();
                return query(content);
            }
        }

        public T Modify<T>(Func<JsonContent, T> modify)
        {
            lock (_lock)
            {
                var content = Load();
                var result = modify(content);
                Save(content);
                return result;
            }
        }

        private JsonContent Load()
        {
            if (!File.Exists(path)) return new JsonContent();

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return new JsonContent();

            return JsonSerializer.Deserialize<JsonContent>(json, Options) ?? new JsonContent();
        }

        private void Save(JsonContent document) =>
            File.WriteAllText(path, JsonSerializer.Serialize(document, Options));
    }
}
