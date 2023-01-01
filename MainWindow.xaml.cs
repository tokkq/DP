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

            _dpMainWindowDataContext = new DPMainWindowDataContext();
            
            _mainDisplayPageSwitcher = _instanceMainDisplayPageSwitcher();

            this.BindDispose(_dpMainWindowDataContext.SubscribeSwitchPage(_onSwitchPage));
        }

        PageSwitcher _instanceMainDisplayPageSwitcher()
        {
            var taskManagementPage = new TaskManagementPage(_dpMainWindowDataContext);
            var todayReflectionPage = new TodayReflectionPage(_dpMainWindowDataContext);
            var weekReflectionPage = new WeekReflectionPage(_dpMainWindowDataContext);
            var pageTypeToPage = new Dictionary<int, Page>()
            {
                { (int)PageType.TaskManagement, taskManagementPage },
                { (int)PageType.WeekReflection, todayReflectionPage },
                { (int)PageType.TodayReflection, weekReflectionPage },
            };

            return new PageSwitcher(_mainFrame, pageTypeToPage);
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
