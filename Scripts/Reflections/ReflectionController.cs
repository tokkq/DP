using System.Windows;

namespace DailyProject_221204
{
    public class ReflectionController
    {
        public TodayReflectionPage TodayReflectionPage { get; } = null!;
        public WeekReflectionPage WeekReflectionPage { get; } = null!;

        public ReflectionController(DPDomain dpContext, Window mainWindow)
        {
            TodayReflectionPage = _instanceTodayReflectionPage(dpContext, mainWindow);
            WeekReflectionPage = _instanceWeekReflectionPage(dpContext, mainWindow);
        }

        TodayReflectionPage _instanceTodayReflectionPage(DPDomain dpContext, Window mainWindow)
        {
            var todayReflectionPage = new TodayReflectionPage();
            var todayReflectionDataContext = new TodayReflectionPageDataContext(dpContext);
            mainWindow.SubscribePageDataContext(todayReflectionPage, todayReflectionDataContext);

            return todayReflectionPage;
        }

        WeekReflectionPage _instanceWeekReflectionPage(DPDomain dpContext, Window mainWindow)
        {
            var weekReflectionPage = new WeekReflectionPage();
            var weekReflectionDataContext = new WeekReflectionPageDataContext(dpContext);
            mainWindow.SubscribePageDataContext(weekReflectionPage, weekReflectionDataContext);

            return weekReflectionPage;
        }
    }
}
