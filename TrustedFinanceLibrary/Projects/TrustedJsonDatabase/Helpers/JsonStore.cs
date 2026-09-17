using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, Lock> Locks =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly string _fullPath = Path.GetFullPath(path);

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        public T Query<T>(Func<JsonContent, T> query)
        {
            lock (GetLockForPath())
            {
                var content = Load();
                return query(content);
            }
        }

        public T Modify<T>(Func<JsonContent, T> modify)
        {
            lock (GetLockForPath())
            {
                var content = Load();
                var result = modify(content);
                Save(content);
                return result;
            }
        }

        private Lock GetLockForPath()
        {
            return Locks.GetOrAdd(_fullPath, _ => new Lock());
        }

        private JsonContent Load()
        {
            if (!File.Exists(_fullPath)) return new JsonContent();

            var json = File.ReadAllText(_fullPath);
            if (string.IsNullOrWhiteSpace(json)) return new JsonContent();

            return JsonSerializer.Deserialize<JsonContent>(json, Options) ?? new JsonContent();
        }

        private void Save(JsonContent document) =>
            File.WriteAllText(_fullPath, JsonSerializer.Serialize(document, Options));
    }
}