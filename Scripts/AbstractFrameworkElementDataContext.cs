using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace DailyProject_221204
{
    /// <summary>
    /// UIElement.DataContextに指定するクラスの抽象クラス。
    /// ViewModel（XAML）からViewModelクラスやModelにアクセスさせるための橋渡しも担うため、役割はViewModel。
    /// </summary>
    public class AbstractFrameworkElementDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// XAMLからViewを更新させる用のコマンド
        /// </summary>
        public EventCommand UpdateViewCommand = new();
        
        readonly DisposeComposer _unloadDisposables = new();
        readonly HashSet<string> _viewProperties = new();
        readonly HashSet<ISaveData> _saveDataSet = new();

        protected AbstractFrameworkElementDataContext()
        {
        }

        /// <summary>
        /// FrameworkElementのLoadedイベントに紐づけられる関数
        /// </summary>
        public void OnLoaded()
        {
            DPDebug.WriteLine($"[type: {GetType()}]OnLoaded");

            _addUnloadDispose(UpdateViewCommand.Subscribe(_notifyUpdateView));

            ReadSaveData();

            _onLoaded();

            _notifyUpdateView();
        }
        /// <summary>
        /// FrameworkElementのUnloadedイベントに紐づけられる関数
        /// </summary>
        public void OnUnloaded()
        {
            DPDebug.WriteLine($"[type: {GetType()}]OnUnloaded");

            _onUnloaded();

            WriteSaveData();

            _unloadDisposables.Dispose();
        }

        /// <summary>
        /// 登録されたセーブデータを恒久的なファイルに書き込む
        /// </summary>
        public void WriteSaveData()
        {
            foreach (var data in _saveDataSet)
            {
                data.Write();
            }
        }
        /// <summary>
        /// 登録されたセーブデータを恒久的なファイルから読み込む
        /// </summary>
        public void ReadSaveData()
        {
            foreach (var data in _saveDataSet)
            {
                data.Read();
            }
        }

        protected virtual void _onLoaded()
        {
        }
        protected virtual void _onUnloaded()
        {
        }

        /// <summary>
        /// セーブデータを登録する
        /// </summary>
        protected ISaveDataHandler<T> _registerSaveData<T>(string directoryPath, string fileName)
            where T : class, new()
        {
            var filePathWithoutExtension = Path.Combine(directoryPath, fileName);
            var filePath = Path.ChangeExtension(filePathWithoutExtension, ".json");

            var saveData = new JsonSaveData<T>(filePath);

            _saveDataSet.Add(saveData);

            return saveData;
        }

        /// <summary>
        /// Unload時に破棄されるDisposablesを追加。
        /// </summary>
        protected void _addUnloadDispose(IDisposable disposable)
        {
            _unloadDisposables.Add(disposable);
        }
        /// <summary>
        /// _notifyUpdateView()で更新通知が発行されるプロパティを追加。
        /// </summary>
        protected void _addViewProperty(string propertyName)
        {
            _viewProperties.Add(propertyName);
        }
        /// <summary>
        /// XAMLに対して更新通知を行う。
        /// </summary>
        protected void _notifyUpdateView()
        {
            foreach (var property in _viewProperties)
            {
                _onPropertyChanged(property);
            }
        }

        void _onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            DPDebug.WriteLine($"[CallerClassType: {GetType()}][PropertyName: {propertyName}]OnPropertyChanged.");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
