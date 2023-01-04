namespace DailyProject_221204
{
    public class TaskEditorDataContext : AbstractPageDataContext
    {
        public TaskEditorDataContext(TaskManagementPageDataContext context)
        {
            _addUnloadDispose(context.SelectEventPublisher.Subscribe(_onTaskSelect));
            _addUnloadDispose(Task.ChangeStatusCommand.Subscribe(() => context.StatusUpdateEventPublisher.Publish(Task.Model)));
        }

        public TaskEditViewModel Task { get; } = new TaskEditViewModel(new TaskModel());

        void _onTaskSelect(TaskModel model)
        {
            Task.SetModel(model);
        }
    }
}
