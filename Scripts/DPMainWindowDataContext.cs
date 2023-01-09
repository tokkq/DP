using System;

namespace DailyProject_221204
{
    public class MainWindowDataContext : AbstractWindowDataContext
    {
        readonly DPDomain _dpContext = null!;

        public MainWindowDataContext(DPDomain dpContext)
        {
            _dpContext = dpContext;
        }

        public IDisposable SubscribeSwitchPage(Action<PageType> action) => _dpContext.SubscribeSwitchPage(action);
    } 
}
