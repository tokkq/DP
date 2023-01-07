namespace DailyProject_221204
{
    public class TaskEditorPageDataContext : AbstractTaskManagementPageDataContext
    {
        public TaskEditorPageDataContext(TaskManagementContext taskManagementContext) : base(taskManagementContext)
        {
            _addUnloadDispose(taskManagementContext.SelectEventPublisher.Subscribe(_onTaskSelect));
            _addUnloadDispose(Task.ChangeStatusCommand.Subscribe(() => taskManagementContext.StatusUpdateEventPublisher.Publish(Task.Model)));
        }

        public TaskEditViewModel Task { get; } = new TaskEditViewModel(new TaskModel());

        void _onTaskSelect(TaskModel model)
        {
            Task.SetModel(model);
        }
    }
}
