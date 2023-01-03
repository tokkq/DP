using DailyProject_221204.Scripts;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DailyProject_221204
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DPMainWindowDataContext _dpMainWindowDataContext = null!;

        readonly PageSwitcher _mainDisplayPageSwitcher = null!;

        public MainWindow()
        {
            InitializeComponent();

            var dPMainWindowDataContext = new DPMainWindowDataContext();
            this.SubscribeWindowDataContext(dPMainWindowDataContext);
            this.Subscribe(dPMainWindowDataContext.SubscribeSwitchPage(_onSwitchPage));

            var taskManagementPageDataContext = new TaskManagementPageDataContext();
            var taskManagementPage = new TaskManagementPage();
            taskManagementPage.SubscribePageDataContext(taskManagementPageDataContext);
            this.SubscribePageDataContext(taskManagementPageDataContext);

            var taskEditorDataContext = new TaskEditorDataContext(taskManagementPageDataContext);
            var taskEditorPage = new TaskEditorPage();
            taskEditorPage.SubscribePageDataContext(taskEditorDataContext);
            this.SubscribePageDataContext(taskEditorDataContext);

            var taskListPageDataContext = new TaskListPageDataContext(taskManagementPageDataContext);
            var taskListPage = new TaskListPage();
            taskListPage.SubscribePageDataContext(taskListPageDataContext);
            this.SubscribePageDataContext(taskListPageDataContext);

            var scheduleListPageDataContext = new ScheduleListPageDataContext(taskManagementPageDataContext);
            var scheduleListPage = new ScheduleListPage();
            scheduleListPage.SubscribePageDataContext(scheduleListPageDataContext);
            this.SubscribePageDataContext(scheduleListPageDataContext);

            taskManagementPage.InitializePage(taskEditorPage, taskListPage, scheduleListPage);

            var todayReflectionPage = new TodayReflectionPage(dPMainWindowDataContext);
            var weekReflectionPage = new WeekReflectionPage(dPMainWindowDataContext);

            var pageTypeToPage = new Dictionary<int, Page>()
            {
                { (int)PageType.TaskManagement, taskManagementPage },
                { (int)PageType.WeekReflection, todayReflectionPage },
                { (int)PageType.TodayReflection, weekReflectionPage },
            };
            var mainDisplayPageSwitcher = new PageSwitcher(_mainFrame, pageTypeToPage);

            _dpMainWindowDataContext = dPMainWindowDataContext;
            _mainDisplayPageSwitcher = mainDisplayPageSwitcher;

        }

        void _onSwitchPage(PageType pageType)
        {
            _mainDisplayPageSwitcher.Switch((int)pageType);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var initialPage = _dpMainWindowDataContext.GetInitialPageType();
            _mainDisplayPageSwitcher.Switch((int)initialPage);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
