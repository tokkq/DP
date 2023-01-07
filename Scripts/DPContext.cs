using System;

namespace DailyProject_221204
{
    public class DPContext
    {
        readonly EventPublisher<PageType> _switchPageEventPublisher = new EventPublisher<PageType>();

        public IDisposable SubscribeSwitchPage(Action<PageType> action) => _switchPageEventPublisher.Subscribe(action);

        public void SwitchPage(PageType pageType)
        {
            _switchPageEventPublisher.Publish(pageType);
        }
    }

}
