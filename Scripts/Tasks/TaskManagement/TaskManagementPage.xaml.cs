using DailyProject_221204.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DailyProject_221204
{
    /// <summary>
    /// TaskManagementPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskManagementPage : Page
    {
        TaskEditorPage _taskEditorPage = null!;
        TaskListPage _taskListPage = null!;
        ScheduleListPage _scheduleListPage = null!;

        public TaskManagementPage(DPMainWindowDataContext dpMainWindowDataContext)
        {
            InitializeComponent();

            var context = new TaskManagementPageDataContext();
            this.Subscribe(context);

            _taskEditorPage = new TaskEditorPage(context);
            _taskListPage = new TaskListPage(context);
            _scheduleListPage = new ScheduleListPage(context);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _editFrame.Navigate(_taskEditorPage);
            _taskListFrame.Navigate(_taskListPage);
            _scheduleFrame.Navigate(_scheduleListPage);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
