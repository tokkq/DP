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
    public abstract class AbstractPageDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// XAMLからViewを更新させる用のコマンド
        /// </summary>
        public EventCommand UpdateViewCommand = new();

        /// <summary>
        /// Unload時に破棄されるDisposables
        /// </summary>
        readonly protected DisposeComposer _unloadDisposables = new();
        
        readonly HashSet<string> _viewProperties = new();
        readonly HashSet<ISaveData> _saveDataSet = new();

        protected AbstractPageDataContext()
        {
            _unloadDisposables.Add(UpdateViewCommand.Subscribe(_notifyUpdateView));
        }

        /// <summary>
        /// FrameworkElementのLoadedイベントに紐づけられる関数
        /// </summary>
        public void OnLoaded()
        {
            LoadData();

            _onLoaded();
        }
        /// <summary>
        /// FrameworkElementのUnloadedイベントに紐づけられる関数
        /// ウィンドウが閉じられた時には呼ばれない
        /// </summary>
        public void OnUnloaded()
        {
            _onUnloaded();

            SaveData();

            _unloadDisposables.Dispose();
        }

        /// <summary>
        /// 登録されたセーブデータをセーブする
        /// </summary>
        public void SaveData()
        {
            foreach (var data in _saveDataSet)
            {
                data.Save();
            }
        }
        /// <summary>
        /// 登録されたセーブデータをロードする
        /// </summary>
        public void LoadData()
        {
            foreach (var data in _saveDataSet)
            {
                data.Load();
            }
        }

        protected virtual void _onLoaded()
        {
        }
        protected virtual void _onUnloaded()
        {
        }

        protected void _addViewProperty(string propertyName)
        {
            _viewProperties.Add(propertyName);
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
