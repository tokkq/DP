using System;

namespace DailyProject_221204
{
    public class TaskAddWindowDataContext : AbstractWindowDataContext
    {
        readonly TaskManagementDomain _taskManagementContext = null!;

        public TaskEditViewModel Task { get; set; } = null!;

        public EventCommand TaskAddCommand { get; } = new EventCommand();

        public TaskAddWindowDataContext(TaskManagementDomain taskManagementContext)
        {
            _addViewProperty(nameof(Task));

            _taskManagementContext = taskManagementContext;
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _addUnloadDispose(TaskAddCommand.Subscribe(__onTaskAddCommand));
            void __onTaskAddCommand()
            {
                _taskManagementContext.AddTask(Task.Model);
            }

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
}
