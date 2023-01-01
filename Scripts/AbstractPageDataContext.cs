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
        readonly DisposeComposer _disposeComposer = new();
        readonly HashSet<string> _viewProperties = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public EventCommand UpdateViewCommand = new();

        protected AbstractPageDataContext()
        {
            _subscribe(UpdateViewCommand.Subscribe(_notifyUpdateView));
        }

        public void Dispose()
        {
            _disposeComposer.Dispose();
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
