using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DailyProject_221204.Scripts;

namespace DailyProject_221204
{
    /// <summary>
    /// TaskListPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskListPage : Page
    {
        TaskListPageDataContext _taskListPageDataContext = null!;

        public TaskListPage(TaskManagementPageDataContext context)
        {
            InitializeComponent();

            _taskListPageDataContext = new TaskListPageDataContext(context);
            this.SetDataContext(_taskListPageDataContext);
        }
    }
}
