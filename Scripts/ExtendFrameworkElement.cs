using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

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
        /// unsubscribeElementを指定した場合、非購読がそのElementのUnload時に行われる。
        /// </summary>
        public static void SubscribeOnLoaded(this FrameworkElement frameworkElement, Action action, FrameworkElement? unsubscribeElement = null)
        {
            frameworkElement.Loaded += __onLoaded;
            void __onLoaded(object sender, RoutedEventArgs e)
            {
                DPDebug.WriteLine($"[frameworkElement.Type: {frameworkElement.GetType()}]Loaded event invoke.");
                
                action();
            }

            if (unsubscribeElement == null)
            {
                frameworkElement.SubscribeOnUnloaded(() => frameworkElement.Loaded -= __onLoaded);
            }
            else
            {
                unsubscribeElement.SubscribeOnUnloaded(() => frameworkElement.Loaded -= __onLoaded);
            }
        }
        /// <summary>
        /// Unloaded時イベントに処理を購読させる。購読されたイベントは発火後に非購読となる。
        /// </summary>
        public static void SubscribeOnUnloaded(this FrameworkElement frameworkElement, Action action, FrameworkElement? unsubscribeElement = null)
        {
            frameworkElement.Unloaded += __onUnloaded;
            void __onUnloaded(object sender, RoutedEventArgs e)
            {
                DPDebug.WriteLine($"[frameworkElement.Type: {frameworkElement.GetType()}]Unloaded event invoke.");

                action();

                if(unsubscribeElement == null)
                {
                    frameworkElement.Unloaded -= __onUnloaded;
                }
            }

            if (unsubscribeElement != null)
            {
                unsubscribeElement.SubscribeOnUnloaded(() => frameworkElement.Unloaded -= __onUnloaded);
            }
        }

        /// <summary>
        /// Visible変更時イベントに処理を購読させる。購読されたイベントはInvisiable時に非購読となる。
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="action"></param>
        public static void SubscribeOnVisiable(this FrameworkElement frameworkElement, Action action)
        {
            frameworkElement.IsVisibleChanged += __onVisiable;
            void __onVisiable(object sender, DependencyPropertyChangedEventArgs e)
            {
                var oldValue = (bool)e.OldValue;
                var newValue = (bool)e.NewValue;
                if (oldValue == false && newValue == true)
                {
                    action();
                }
            }

            frameworkElement.SubscribeOnInvisiable(() => frameworkElement.IsVisibleChanged -= __onVisiable);
        }
        /// <summary>
        /// Invisible変更時イベントに処理を購読させる。購読されたイベントは発火後に非購読となる。
        /// </summary>
        public static void SubscribeOnInvisiable(this FrameworkElement frameworkElement, Action action)
        {
            frameworkElement.IsVisibleChanged += __onInvisiable;
            void __onInvisiable(object sender, DependencyPropertyChangedEventArgs e)
            {
                var oldValue = (bool)e.OldValue;
                var newValue = (bool)e.NewValue;
                if (oldValue == true && newValue == false)
                {
                    action();
                    frameworkElement.IsVisibleChanged -= __onInvisiable;
                }
            }
        }

        /// <summary>
        /// FrameworkElementDataContextをFrameworkElementに購読させる。
        /// </summary>
        public static void SubscribeFrameworkElementDataContext<TFrameworkElementDataContext>(this FrameworkElement element, TFrameworkElementDataContext pageDataContext)
            where TFrameworkElementDataContext : AbstractFrameworkElementDataContext
        {
            element.DataContext = pageDataContext;

            element.SubscribeOnLoaded(pageDataContext.OnLoaded);
            element.SubscribeOnUnloaded(pageDataContext.OnUnloaded);
        }

        /// <summary>
        /// Closed時イベントに処理を購読させる。購読されたイベントは発火後に非購読となる。
        /// </summary>
        public static void SubscribeOnClosed(this Window window, Action action)
        {
            window.Closed += __onClosed;
            void __onClosed(object? sender, EventArgs e)
            {
                DPDebug.WriteLine($"[window.Type: {window.GetType()}]Closed event invoke.");
                action();
                window.Closed -= __onClosed;
            }
        }

        /// <summary>
        /// Closed時イベントに処理を購読させる。購読されたイベントは発火後に非購読となる。
        /// </summary>
        public static void SubscribeOnExit(this Application application, Action action)
        {
            application.Exit += __onExit;
            void __onExit(object? sender, EventArgs e)
            {
                DPDebug.WriteLine($"[window.Type: {application.GetType()}]Closed event invoke.");
                action();
                application.Exit -= __onExit;
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
        public static void SubscribePageDataContext<TPageDataContext>(this Window window, Page page, TPageDataContext pageDataContext)
            where TPageDataContext : AbstractPageDataContext
        {
            page.DataContext = pageDataContext;

            page.SubscribeOnLoaded(pageDataContext.OnLoaded, unsubscribeElement: window);
            page.SubscribeOnUnloaded(pageDataContext.OnUnloaded, unsubscribeElement: window);

            // Closedイベントがページ側に存在しないため、OnUnloadedイベントをWindowのClosedに購読させる。
            window.SubscribeOnClosed(pageDataContext.OnUnloaded);
        }
    }
}
