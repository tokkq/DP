namespace DailyProject_221204
{
    public class AbstractWindowDataContext : AbstractFrameworkElementDataContext
    {
        /// <summary>
        /// WindowのClosedイベントに紐づけられる関数
        /// </summary>
        public void OnClosed()
        {
            _onClosed();
        }

        protected virtual void _onClosed()
        {
        }
    }
}
