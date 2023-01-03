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

            var dpMainWindowDataContext = _instanceDPMainWindowDataContext();

            var taskManagementPage = _instanceTaskManagementPage();
            var todayReflectionPage = new TodayReflectionPage(dpMainWindowDataContext);

            var weekReflectionPage = new WeekReflectionPage();
            var weekReflectionDataContext = new WeekReflectionPageDataContext(dpMainWindowDataContext);
            _subscribePage(weekReflectionPage, weekReflectionDataContext);

            var pageTypeToPage = new Dictionary<int, Page>()
            {
                { (int)PageType.TaskManagement, taskManagementPage },
                { (int)PageType.WeekReflection, weekReflectionPage },
                { (int)PageType.TodayReflection, todayReflectionPage },
            };
            var mainDisplayPageSwitcher = new PageSwitcher(_mainFrame, pageTypeToPage);

            _dpMainWindowDataContext = dpMainWindowDataContext;
            _mainDisplayPageSwitcher = mainDisplayPageSwitcher;
        }

        DPMainWindowDataContext _instanceDPMainWindowDataContext()
        {
            var dPMainWindowDataContext = new DPMainWindowDataContext();
            this.SubscribeWindowDataContext(dPMainWindowDataContext);
            this.Subscribe(dPMainWindowDataContext.SubscribeSwitchPage(_onSwitchPage));

            return dPMainWindowDataContext;
        }

        TaskManagementPage _instanceTaskManagementPage()
        {
            var taskManagementPage = new TaskManagementPage();
            var taskManagementPageDataContext = new TaskManagementPageDataContext();
            _subscribePage(taskManagementPage, taskManagementPageDataContext);

            var taskEditorPage = new TaskEditorPage();
            var taskEditorDataContext = new TaskEditorDataContext(taskManagementPageDataContext);
            _subscribePage(taskEditorPage, taskEditorDataContext);

            var taskListPage = new TaskListPage();
            var taskListPageDataContext = new TaskListPageDataContext(taskManagementPageDataContext);
            _subscribePage(taskListPage, taskListPageDataContext);

            var scheduleListPage = new ScheduleListPage();
            var scheduleListPageDataContext = new ScheduleListPageDataContext(taskManagementPageDataContext);
            _subscribePage(scheduleListPage, scheduleListPageDataContext);

            taskManagementPage.InitializePage(taskEditorPage, taskListPage, scheduleListPage);

            return taskManagementPage;
        }

        void _subscribePage(Page page, AbstractPageDataContext context)
        {
            page.SubscribePageDataContext(context);
            this.SubscribePageDataContext(context);
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
