using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace DailyProject_221204
{
    static class JsonUtility
    {
        public static int SaveJson<T>(T value, string saveFilePath)
            where T : class, new()
        {
            var extension = Path.GetExtension(saveFilePath);
            if (extension != ".json")
            {
                Debug.WriteLine($"[SaveJson][Failed]saveFilePath doesnt has json extension. [extension]{extension}[saveFilePath]{saveFilePath}");
                return -1;
            }

            var saveDirectory = Directory.GetParent(saveFilePath);
            // The returned value is null if the specified path is null, empty, or a root (such as "\", "C:", or "\\server\share").
            if (saveDirectory == null) 
            { 
                throw new DirectoryNotFoundException($"[value: {value}][saveFilePath: {saveFilePath}] Jsonファイルの保存ディレクトリの取得に失敗しました。");
            }

            var existDirectory = Directory.Exists(saveDirectory.FullName);
            if (existDirectory == false)
            {
                Directory.CreateDirectory(saveDirectory.FullName);
            }

            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            var serial = JsonSerializer.Serialize(value, options);
            using (var fileStream = File.Create(saveFilePath))
            using (var writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
            {
                writer.WriteLine($"{serial}");
            }

            Debug.WriteLine($"[SaveJson][value]{value}[Path]{saveFilePath}");

            return 0;
        }

        public static T LoadJson<T>(string path, bool shouldCreateNewFileIfNoExistJson = false)
            where T : class, new()
        {
            if (File.Exists(path) == false)
            {
                if (shouldCreateNewFileIfNoExistJson == true)
                {
                    SaveJson(new T(), path);
                }
                else
                {
                    throw new FileNotFoundException($"[path: {path}][shouldCreateNewFileIfNoExistJson: {shouldCreateNewFileIfNoExistJson}]Jsonファイルが存在しません。");
                }
            }

            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                MaxDepth = 16,
            };
            using (var file = File.OpenText(path))
            {
                var text = file.ReadToEnd();
                var json = JsonSerializer.Deserialize<T>(text, options);

                if (json == null)
                {
                    throw new FileLoadException($"[path: {path}][shouldCreateNewFileIfNoExistJson: {shouldCreateNewFileIfNoExistJson}]Jsonファイルの読み込みに失敗しました。");
                }

                Debug.WriteLine($"[LoadJson][Path]{path}");

                return json;
            }
        }
    }
}
