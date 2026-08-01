using System.Text.Json;

namespace OnlineStore.Repositories
{
    public class JsonFileContext
    {
        private readonly string _dataDirectory;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public JsonFileContext(IWebHostEnvironment env)
        {
            // Base directory for Data folder
            _dataDirectory = Path.Combine(env.ContentRootPath, "Data");
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public List<T> ReadList<T>(string fileName)
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            lock (filePath)
            {
                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<T>();
                }
                return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
            }
        }

        public T? ReadObject<T>(string fileName)
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return default;
            }

            lock (filePath)
            {
                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return default;
                }
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
        }

        public void WriteList<T>(string fileName, List<T> items)
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            lock (filePath)
            {
                var json = JsonSerializer.Serialize(items, JsonOptions);
                File.WriteAllText(filePath, json);
            }
        }

        public void WriteObject<T>(string fileName, T item)
        {
            var filePath = Path.Combine(_dataDirectory, fileName);
            lock (filePath)
            {
                var json = JsonSerializer.Serialize(item, JsonOptions);
                File.WriteAllText(filePath, json);
            }
        }
    }
}
