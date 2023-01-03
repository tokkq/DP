using System.IO;

namespace DailyProject_221204
{
    public class JsonSaveData<T> : ISaveData, ISaveDataHandler<T>
        where T : class, new()
    {
        T _saveData = null!;
        string _saveDataFilePath = string.Empty;

        SaveReadResult _lastReadResult = SaveReadResult.Success;
        SaveWriteResult _lastWriteResult = SaveWriteResult.Success;

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
            _lastWriteResult = SaveWriteResult.Success;
        }
        public void Read()
        {
            if (File.Exists(_saveDataFilePath) == true)
            {
                _saveData = JsonUtility.LoadJson<T>(_saveDataFilePath);
                _lastReadResult = SaveReadResult.Success;
            }
            else
            {
                _saveData = new T();
                _lastReadResult = SaveReadResult.NoExistFile;
            }
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
        public SaveReadResult GetReadResult()
        {
            return _lastReadResult;
        }
        public SaveWriteResult GetWriteResult()
        {
            return _lastWriteResult;
        }
    }

    public enum SaveReadResult
    {
        /// <summary>
        /// 処理がされていない。
        /// </summary>
        None,
        /// <summary>
        /// 成功。
        /// </summary>
        Success,
        /// <summary>
        /// ファイルが存在せず失敗。
        /// </summary>
        NoExistFile,
    }
    public enum SaveWriteResult
    {
        /// <summary>
        /// 処理がされていない。
        /// </summary>
        None,
        /// <summary>
        /// 成功。
        /// </summary>
        Success,
    }
}
