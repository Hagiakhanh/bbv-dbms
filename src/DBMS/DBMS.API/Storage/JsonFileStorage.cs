using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBMS.API.Storage
{
    public static class JsonFileStorage
    {
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static bool IsTestEnvironment =>
            AppDomain.CurrentDomain.FriendlyName.Contains("testhost", StringComparison.OrdinalIgnoreCase) ||
            AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName?.Contains("xunit", StringComparison.OrdinalIgnoreCase) == true);

        private static string GetFilePath(string fileName)
        {
            var current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, "docs", "api-roadmap");
                if (Directory.Exists(candidate))
                {
                    return Path.Combine(candidate, fileName);
                }
                current = current.Parent;
            }

            var fallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(fallback))
            {
                Directory.CreateDirectory(fallback);
            }
            return Path.Combine(fallback, fileName);
        }

        public static async Task<T> LoadAsync<T>(string fileName, T defaultData)
        {
            if (IsTestEnvironment)
            {
                return defaultData;
            }

            var filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
            {
                await SaveAsync(fileName, defaultData);
                return defaultData;
            }

            try
            {
                await _semaphore.WaitAsync();
                var json = await File.ReadAllTextAsync(filePath);
                var data = JsonSerializer.Deserialize<T>(json, _options);
                return data ?? defaultData;
            }
            catch
            {
                return defaultData;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task SaveAsync<T>(string fileName, T data)
        {
            if (IsTestEnvironment)
            {
                return;
            }

            var filePath = GetFilePath(fileName);
            await _semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(data, _options);
                await File.WriteAllTextAsync(filePath, json);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

