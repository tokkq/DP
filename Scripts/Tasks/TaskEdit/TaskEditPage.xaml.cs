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
using DailyProject_221204.Scripts;

namespace DailyProject_221204
{
    /// <summary>
    /// TaskEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskEditorPage : Page
    {
        public TaskEditViewModel Task { get; } = new TaskEditViewModel(new TaskModel());

        public TaskEditorPage(TaskManagementPageDataContext context)
        {
            InitializeComponent();

            DataContext = this;

            this.Subscribe(context.SelectEventPublisher.Subscribe(_onTaskSelect));
            this.Subscribe(Task.ChangeStatusCommand.Subscribe(() => context.StatusUpdateEventPublisher.Publish(Task.Model)));
        }

        void _onTaskSelect(TaskModel model)
        {
            Task.SetModel(model);
        }
    }
}
