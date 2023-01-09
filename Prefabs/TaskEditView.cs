using System.Windows;
using System.Windows.Controls;

namespace DailyProject_221204
{
    public class TaskEditView : AbstractCustomControl
    {
        static TaskEditView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskEditView), new FrameworkPropertyMetadata(typeof(TaskEditView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var nameTextBox = _getElement<TextBox>("_taskNameText");
            nameTextBox.Focus();
        }

        public TaskEditViewModel Task
        {
            get { return (TaskEditViewModel)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Task.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(TaskEditViewModel), typeof(TaskEditView));
    }
}
