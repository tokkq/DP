using System;

namespace DailyProject_221204.Scripts
{
    public class ActionDisposer : IDisposable
    {
        Action _dispose;

        public ActionDisposer(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}
