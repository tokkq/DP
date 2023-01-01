using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DailyProject_221204
{
    /// <summary>
    /// ViewModelの抽象クラス。Modelに対して必要なViewの状態を保持する。状態に基づいたViewの処理はXAMLで行う。
    /// すべてのViewModelはこのクラスを継承する。
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public class AbstractViewModel<TModel> : INotifyPropertyChanged where TModel : class
    {
        public TModel Model { get; private set; } = null!;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AbstractViewModel(TModel model)
        {
            SetModel(model);
        }

        public void SetModel(TModel model)
        {
            Model = model;
            _onPropertyChanged(nameof(Model));

            _onSetModel(model);
        }

        protected virtual void _onSetModel(TModel model)
        {
        }

        protected void _onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}