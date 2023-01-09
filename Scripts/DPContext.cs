using System;
using System.Windows;

namespace DailyProject_221204
{
    public class DPContext : IDisposable
    {
        public GlobalHotKey GlobalHotKey { get; } = null!;

        readonly EventPublisher<PageType> _switchPageEventPublisher = new EventPublisher<PageType>();

        public DPContext(GlobalHotKey globalHotKey)
        {
            GlobalHotKey = globalHotKey;
        }

        public IDisposable SubscribeSwitchPage(Action<PageType> action) => _switchPageEventPublisher.Subscribe(action);

        public void SwitchPage(PageType pageType)
        {
            _switchPageEventPublisher.Publish(pageType);
        }

        public void Dispose()
        {
            GlobalHotKey.Dispose();
        }
    }

}
