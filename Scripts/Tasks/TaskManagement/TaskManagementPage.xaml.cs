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
        TaskAddWindow _taskAddWindow = null!;

        public TaskManagementPage()
        {
            InitializeComponent();
        }

        public void Initialize(TaskEditorPage taskEditorPage, TaskListPage taskListPage, ScheduleListPage scheduleListPage)
        {
            _editFrame.Navigate(taskEditorPage);
            _taskListFrame.Navigate(taskListPage);
            _scheduleFrame.Navigate(scheduleListPage);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
