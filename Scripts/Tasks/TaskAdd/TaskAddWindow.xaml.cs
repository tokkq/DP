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
using System.Windows.Shapes;

namespace DailyProject_221204
{
    /// <summary>
    /// TaskAddWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskAddWindow : Window
    {
        public TaskAddWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }

    public class TaskAddWindowDataContext : AbstractWindowDataContext
    {
        readonly TaskManagementContext _taskManagementContext = null!;

        public TaskEditViewModel Task { get; set; } = null!;

        public EventCommand TaskAddCommand { get; } = new EventCommand();

        public TaskAddWindowDataContext(TaskManagementContext taskManagementContext)
        {
            _addUnloadDispose(TaskAddCommand.Subscribe(__onTaskAddCommand));
            void __onTaskAddCommand()
            {
                taskManagementContext.AddTaskEventPublisher.Publish(Task.Model);
            }

            _addViewProperty(nameof(Task));

            _taskManagementContext = taskManagementContext;
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _initializeTask();
        }

        void _initializeTask()
        {
            var model = new TaskModel()
            {
                StartAt = DateTime.Now,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };
            var viewModel = new TaskEditViewModel(model);
            
            Task = viewModel;
        }
    }

    public class TaskAddPageDataContext : AbstractPageDataContext
    {
        
    }
}
