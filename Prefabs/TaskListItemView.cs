using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DailyProject_221204
{
    public class TaskListItemView : AbstractCustomControl
    {
        readonly IEnumerable<TaskStatusType> _taskStatusItems = new TaskStatusType[]
        {
            TaskStatusType.Active,
            TaskStatusType.Wait,
            TaskStatusType.Idea,
            TaskStatusType.Complete,
        };

        static TaskListItemView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskListItemView), new FrameworkPropertyMetadata(typeof(TaskListItemView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _setBinding<Border>("_taskBorder", Border.BackgroundProperty, nameof(TaskBackgroundColor));
            _setBinding<Button>("_taskTitleButton", ButtonBase.CommandProperty, nameof(TaskTitleClickCommand));
            _setBinding<Button>("_taskTitleButton", ContentControl.ContentProperty, nameof(TaskTitleText));
            _setBinding<Button>("_taskStatusButton", ContentControl.ContentProperty, nameof(TaskStatusText));
        }

        public SolidColorBrush TaskBackgroundColor
        {
            get { return (SolidColorBrush)GetValue(TaskBackgroundColorProperty); }
            set { SetValue(TaskBackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskBackgroundColorProperty =
            DependencyProperty.Register("TaskBackgroundColor", typeof(SolidColorBrush), typeof(TaskListItemView));

        public ICommand TaskTitleClickCommand
        {
            get { return (ICommand)GetValue(TaskTitleClickCommandProperty); }
            set { SetValue(TaskTitleClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskTitleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskTitleClickCommandProperty =
            DependencyProperty.Register("TaskTitleClickCommand", typeof(ICommand), typeof(TaskListItemView));


        public string TaskTitleText
        {
            get { return (string)GetValue(TaskTitleTextProperty); }
            set { SetValue(TaskTitleTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskNameText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskTitleTextProperty =
            DependencyProperty.Register("TaskTitleText", typeof(string), typeof(TaskListItemView));


        public string TaskStatusText
        {
            get { return (string)GetValue(TaskStatusTextProperty); }
            set { SetValue(TaskStatusTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskStatusText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskStatusTextProperty =
            DependencyProperty.Register("TaskStatusText", typeof(string), typeof(TaskListItemView));
    }
}
