namespace DailyProject_221204
{
    public class AbstractTaskManagementPageDataContext : AbstractPageDataContext
    {
        readonly protected TaskManagementDomain _taskManagementDomain = null!;

        public AbstractTaskManagementPageDataContext(TaskManagementDomain taskManagementDomain)
        {
            _taskManagementDomain = taskManagementDomain;
        }
    }
}
