using System.IO;

namespace DailyProject_221204
{
    public class JsonSaveData<T> : ISaveData, ISaveDataHandler<T>
        where T : class, new()
    {
        T _saveData = null!;
        string _saveDataFilePath = string.Empty;

        public JsonSaveData(string saveDataFilePath)
        {
            _saveDataFilePath = saveDataFilePath;

            Read();
        }

        public bool ExistSaveFile()
        {
            var exist = File.Exists(_saveDataFilePath);
            return exist;
        }
        public bool ExistSaveDirectory()
        {
            var directoryPath = Path.GetDirectoryName(_saveDataFilePath);
            var exist = Directory.Exists(directoryPath);
            return exist;
        }

        public void Write()
        {
            JsonUtility.SaveJson(_saveData, _saveDataFilePath);
        }
        public void Read()
        {
            _saveData = JsonUtility.LoadJson<T>(_saveDataFilePath, shouldCreateNewFileIfNoExistJson: true);
        }

        public void SetPath(string path)
        {
            _saveDataFilePath = path;
        }
        public void SetValue(T data)
        {
            _saveData = data;
        }
        public T GetValue()
        {
            return _saveData;
        }
    }
}
