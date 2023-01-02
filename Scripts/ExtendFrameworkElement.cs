using System;
using System.Windows;
using System.Windows.Controls;

namespace DailyProject_221204
{
    public static class ExtendFrameworkElement
    {
        /// <summary>
        /// Unloaded時にDisposeを購読させる。
        /// </summary>
        public static void Subscribe(this FrameworkElement frameworkElement, IDisposable disposable)
        {
            frameworkElement.SubscribeOnUnloaded(disposable.Dispose);
        }

        /// <summary>
        /// Loaded時イベントに処理を購読させる。購読されたイベントはUnload時に非購読となる。
        /// </summary>
        public static void SubscribeOnLoaded(this FrameworkElement frameworkElement, Action action)
        {
            frameworkElement.Loaded += __onLoaded;
            void __onLoaded(object sender, RoutedEventArgs e)
            {
                DPDebug.WriteLine($"[Load][Dispose]frameworkElement.Name: {frameworkElement.Name}");
                action();
            }

            frameworkElement.SubscribeOnUnloaded(() => frameworkElement.Loaded -= __onLoaded);
        }
        /// <summary>
        /// Unloaded時イベントに処理を購読させる。購読されたイベントは発火後に非購読となる。
        /// </summary>
        public static void SubscribeOnUnloaded(this FrameworkElement frameworkElement, Action action)
        {
            frameworkElement.Unloaded += __onUnloaded;
            void __onUnloaded(object sender, RoutedEventArgs e)
            {
                DPDebug.WriteLine($"[Unload][Dispose]frameworkElement.Name: {frameworkElement.Name}");
                action();
                frameworkElement.Unloaded -= __onUnloaded;
            }
        }

        /// <summary>
        /// DataContextをセットし、UnloadedタイミングでDisposeするようにバインドする。
        /// </summary>
        public static void SetPageDataContext<TPageDataContextType>(this Page page, TPageDataContextType pageDataContext)
            where TPageDataContextType : AbstractPageDataContext
        {
            page.DataContext = pageDataContext;
            page.Subscribe(pageDataContext);

            page.SubscribeOnLoaded(pageDataContext.OnLoaded);
            page.SubscribeOnUnloaded(pageDataContext.OnUnloaded);
        }
    }
}
