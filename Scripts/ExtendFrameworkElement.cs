using System;
using System.Windows;

namespace DailyProject_221204
{
    public static class ExtendFrameworkElement
    {
        /// <summary>
        /// Disposeを終了タイミングのイベントで呼ぶようにバインドする。
        /// </summary>
        public static void BindDispose(this FrameworkElement frameworkElement, IDisposable disposable)
        {
            frameworkElement.Unloaded += (a, b) =>
            {
                DPDebug.WriteLine($"[Unload][Dispose]frameworkElement.Name: {frameworkElement.Name}");
                disposable.Dispose();
            };
        }

        /// <summary>
        /// DataContextをセットし、UnloadedタイミングでDisposeするようにバインドする。
        /// </summary>
        public static void SetDataContext<TDataContextType>(this FrameworkElement frameworkElement, TDataContextType dataContext)
            where TDataContextType : AbstractPageDataContext
        {
            frameworkElement.DataContext = dataContext;
            frameworkElement.BindDispose(dataContext);
        }
    }
}
