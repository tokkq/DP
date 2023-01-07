using System;

namespace DailyProject_221204
{
    public class TaskManagementPageDataContext : AbstractTaskManagementPageDataContext
    {
        public IDisposable SubscribeOpenAddTaskWindow(Action action) => _taskManagementContext.OpenAddTaskWindowEventPublisher.Subscribe(action);

        public TaskManagementPageDataContext(TaskManagementContext taskManagementContext) : base(taskManagementContext)
        {
        }
    }
}
