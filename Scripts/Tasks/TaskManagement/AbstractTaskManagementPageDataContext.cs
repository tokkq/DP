namespace DailyProject_221204
{
    public class AbstractTaskManagementPageDataContext : AbstractPageDataContext
    {
        readonly protected TaskManagementContext _taskManagementContext = null!;

        public AbstractTaskManagementPageDataContext(TaskManagementContext taskManagementContext)
        {
            _taskManagementContext = taskManagementContext;
        }
    }
}
