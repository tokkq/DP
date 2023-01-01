using System;
using System.Diagnostics;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace DailyProject_221204
{
    public class EventCommand : ICommand
    {
        readonly EventPublisher _eventPublisher = new();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Debug.WriteLine($"[parameter: {parameter}]EventCommand Excute");

            _eventPublisher.Publish();
        }

        public IDisposable Subscribe(Action action) => _eventPublisher.Subscribe(action);
    }
    public class EventCommand<T> : ICommand
    {
        readonly EventPublisher<T> _eventPublisher = new();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException($"[parameter: {parameter}]引数付きのコマンドですが、引数がNullです。");
            }

            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException($"[parameter: {parameter}]引数付きのコマンドですが、引数がNullです。");
            }

            Debug.WriteLine($"[parameter: {parameter}]EventCommand Excute");

            var value = (T)parameter;
            if (value == null )
            {
                throw new InvalidCastException($"[parameter: {parameter}][castType: {typeof(T)}]parameterのキャストに失敗しました。");
            }

            _eventPublisher.Publish(value);
        }

        public IDisposable Subscribe(Action<T> action) => _eventPublisher.Subscribe(action);
    }
}
