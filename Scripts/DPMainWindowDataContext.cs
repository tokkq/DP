using System;

namespace DailyProject_221204
{
    public class MainWindowDataContext : AbstractWindowDataContext
    {
        readonly DPContext _dpContext = null!;

        public MainWindowDataContext(DPContext dpContext)
        {
            _dpContext = dpContext;
        }

        public IDisposable SubscribeSwitchPage(Action<PageType> action) => _dpContext.SubscribeSwitchPage(action);
    } 
}
