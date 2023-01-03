using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace DailyProject_221204
{
    public class DPMainWindowDataContext : AbstractWindowDataContext
    {
        readonly EventPublisher<PageType> _switchPageEventPublisher = new EventPublisher<PageType>();

        public IDisposable SubscribeSwitchPage(Action<PageType> action) => _switchPageEventPublisher.Subscribe(action);

        public void SwitchPage(PageType pageType)
        {
            _switchPageEventPublisher.Publish(pageType);
        }
        public PageType GetInitialPageType()
        {
            return PageType.WeekReflection;
        }
    }

    public class AbstractWindowDataContext : AbstractFrameworkElementDataContext
    {
        /// <summary>
        /// WindowのClosedイベントに紐づけられる関数
        /// </summary>
        public void OnClosed()
        {
            _onClosed();
        }

        protected virtual void _onClosed()
        {
        }
    }

    public class AbstractFrameworkElementDataContext : INotifyPropertyChanged
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

        protected AbstractFrameworkElementDataContext()
        {
            _unloadDisposables.Add(UpdateViewCommand.Subscribe(_notifyUpdateView));
        }

        /// <summary>
        /// FrameworkElementのLoadedイベントに紐づけられる関数
        /// </summary>
        public void OnLoaded()
        {
            ReadSaveData();

            _onLoaded();
        }
        /// <summary>
        /// FrameworkElementのUnloadedイベントに紐づけられる関数
        /// ウィンドウが閉じられた時には呼ばれない
        /// </summary>
        public void OnUnloaded()
        {
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
}
