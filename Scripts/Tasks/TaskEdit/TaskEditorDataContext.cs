namespace DailyProject_221204
{
    public class TaskEditorDataContext : AbstractPageDataContext
    {
        public TaskEditorDataContext(TaskManagementPageDataContext context)
        {
            _unloadDisposables.Add(context.SelectEventPublisher.Subscribe(_onTaskSelect));
            _unloadDisposables.Add(Task.ChangeStatusCommand.Subscribe(() => context.StatusUpdateEventPublisher.Publish(Task.Model)));
        }

        public TaskEditViewModel Task { get; } = new TaskEditViewModel(new TaskModel());

        void _onTaskSelect(TaskModel model)
        {
            Task.SetModel(model);
        }
    }
}
