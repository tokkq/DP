using DailyProject_221204.Scripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DailyProject_221204
{
    /// <summary>
    /// UIElement.DataContextに指定するクラスの抽象クラス。
    /// </summary>
    public abstract class AbstractPageDataContext : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public EventCommand UpdateViewCommand = new();

        readonly DisposeComposer _disposeComposer = new();
        readonly HashSet<string> _viewProperties = new();
        readonly HashSet<ISaveData> _saveDataSet = new();

        protected AbstractPageDataContext()
        {
            _subscribe(UpdateViewCommand.Subscribe(_notifyUpdateView));
        }

        public void OnLoaded()
        {
            LoadData();

            _onLoaded();
        }
        public void OnUnloaded()
        {
            _onUnloaded();

            SaveData();
        }

        public void SaveData()
        {
            foreach (var data in _saveDataSet)
            {
                data.Save();
            }
        }
        public void LoadData()
        {
            foreach (var data in _saveDataSet)
            {
                data.Load();
            }
        }

        public void Dispose()
        {
            _disposeComposer.Dispose();
        }

        protected virtual void _onLoaded()
        {
        }
        protected virtual void _onUnloaded()
        {
        }

        protected void _subscribe(IDisposable disposable)
        {
            _disposeComposer.Add(disposable);
        }
        protected void _subscribe(IEnumerable<IDisposable> disposables)
        {
            _disposeComposer.Add(disposables);
        }
        protected void _subscribe(Action onDisposeAction)
        {
            var actionDisposer = new ActionDisposer(onDisposeAction);
            _disposeComposer.Add(actionDisposer);
        }

        protected void _addViewProperty(string propertyName)
        {
            _viewProperties.Add(propertyName);
        }

        protected ISaveDataHandler<T> _registerSaveData<T>(string path)
            where T : class, new()
        {
            var saveData = new JsonSaveData<T>(path);

            _saveDataSet.Add(saveData);

            return saveData;
        }

        protected void _notifyUpdateView()
        {
            foreach (var property in _viewProperties)
            {
                _onPropertyChanged(property);
            }
        }

        void _onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Debug.WriteLine($"[PropertyName: {propertyName}]OnPropertyChanged");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class JsonSaveData<T> : ISaveData, ISaveDataHandler<T>
        where T : class, new()
    {
        T _data = null!;
        string _path = string.Empty;

        public JsonSaveData(string path)
        {
            _path = path;

            Load();
        }

        public void Save()
        {
            if (_data != null)
            {
                JsonUtility.SaveJson(_data, _path);
            }
        }
        public void Load()
        {
            _data = JsonUtility.LoadJson<T>(_path, shouldCreateNewFileIfNoExistJson:true);
        }

        public void SetPath(string path)
        {
            _path = path;
        }
        public void SetValue(T data)
        {
            _data = data;
        }
        public T GetValue()
        {
            return _data;
        }
    }

    public interface ISaveData
    {
        void Save();
        void Load();
    }

    public interface ISaveDataHandler<T>
        where T : class
    {
        void SetValue(T data);
        T GetValue();
    }
}
