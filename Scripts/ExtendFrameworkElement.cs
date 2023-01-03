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
        /// PageDataContextをFrameworkElementに購読させる。
        /// </summary>
        public static void SubscribeFrameworkElementDataContext<TFrameworkElementDataContext>(this FrameworkElement element, TFrameworkElementDataContext pageDataContext)
            where TFrameworkElementDataContext : AbstractFrameworkElementDataContext
        {
            element.DataContext = pageDataContext;

            element.SubscribeOnLoaded(pageDataContext.OnLoaded);
            element.SubscribeOnUnloaded(pageDataContext.OnUnloaded);
        }

        /// <summary>
        /// PageDataContextをPageに購読させる。
        /// </summary>
        public static void SubscribePageDataContext<TPageDataContext>(this Page page, TPageDataContext pageDataContext)
            where TPageDataContext : AbstractPageDataContext
        {
            page.SubscribeFrameworkElementDataContext(pageDataContext);
        }


        /// <summary>
        /// Closed時イベントに処理を購読させる。購読されたイベントは発火後に非購読となる。
        /// </summary>
        public static void SubscribeOnClosed(this Window window, Action action)
        {
            window.Closed += __onClosed;
            void __onClosed(object? sender, EventArgs e)
            {
                DPDebug.WriteLine($"[Closed]window.Name: {window.Name}");
                action();
                window.Closed -= __onClosed;
            }
        }
        /// <summary>
        /// WindowDataContextをWindowに購読させる。
        /// </summary>
        public static void SubscribeWindowDataContext<TWindowDataContext>(this Window window, TWindowDataContext windowDataContext)
            where TWindowDataContext : AbstractWindowDataContext
        {
            window.SubscribeFrameworkElementDataContext(windowDataContext);
            window.SubscribeOnClosed(windowDataContext.OnClosed);
        }
        /// <summary>
        /// PageDataContextをWindowに購読させる。
        /// </summary>
        public static void SubscribePageDataContext<TPageDataContext>(this Window window, TPageDataContext pageDataContext)
            where TPageDataContext : AbstractPageDataContext
        {
            // Closedイベントがページ側に存在しないため、OnUnloadedイベントをWindowのClosedに購読させる。
            window.SubscribeOnClosed(pageDataContext.OnUnloaded);
        }
    }
}
