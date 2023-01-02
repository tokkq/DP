using System;
using System.Windows.Input;

namespace DailyProject_221204
{
    public class StandardCommand : ICommand
    {
        readonly Action<object?> _onExecute = null!;
        readonly Func<bool> _getCanExecute = null!;

        public event EventHandler? CanExecuteChanged;

        public StandardCommand(Action<object?> onExecute)
        {
            _onExecute = onExecute;
            _getCanExecute = () => true;
        }

        public StandardCommand(Action<object?> onExecute, Func<bool> getCanExecute)
        {
            _onExecute = onExecute;
            _getCanExecute = getCanExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _getCanExecute.Invoke();
        }

        public void Execute(object? parameter)
        {
            _onExecute?.Invoke(parameter);
        }
    }
    public class StandardCommand<T> : ICommand
    {
        readonly Action<T> _onExecute = null!;
        readonly Func<bool> _getCanExecute = null!;

        public event EventHandler? CanExecuteChanged;

        public StandardCommand(Action<T> onExecute)
        {
            _onExecute = onExecute;
            _getCanExecute = () => true;
        }

        public StandardCommand(Action<T> onExecute, Func<bool> getCanExecute)
        {
            _onExecute = onExecute;
            _getCanExecute = getCanExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _getCanExecute.Invoke();
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException($"[parameter: {parameter}]引数付きのコマンドですが、引数がNullです。");
            }

            var value = (T)parameter;
            if (value == null)
            {
                throw new InvalidCastException($"[parameter: {parameter}][castType: {typeof(T)}]parameterのキャストに失敗しました。");
            }

            _onExecute.Invoke(value);
        }
    }
}
