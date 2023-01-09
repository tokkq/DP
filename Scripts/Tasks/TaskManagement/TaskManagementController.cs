using System.Windows;

namespace DailyProject_221204
{
    public class TaskManagementController
    {
        public TaskManagementPage TaskManagementPage { get; } = null!;

        public TaskManagementController(DPDomain dpDomain, Window mainWindow)
        {
            // TaskManagementPageにサブスクライブするのが自然そうだが、初期化をどこで行うかを検討する必要がある。
            var taskManagementDomain = new TaskManagementDomain(dpDomain);
            mainWindow.Subscribe(taskManagementDomain);

            var taskManagementPageDataContext = new TaskManagementPageDataContext(taskManagementDomain);
            var taskManagementPage = new TaskManagementPage();
            mainWindow.SubscribePageDataContext(taskManagementPage, taskManagementPageDataContext);

            var taskEditorDataContext = new TaskEditorPageDataContext(taskManagementDomain);
            var taskEditorPage = new TaskEditorPage();
            mainWindow.SubscribePageDataContext(taskEditorPage, taskEditorDataContext);

            var taskListPageDataContext = new TaskListPageDataContext(taskManagementDomain);
            var taskListPage = new TaskListPage();
            mainWindow.SubscribePageDataContext(taskListPage, taskListPageDataContext);

            var scheduleListPageDataContext = new ScheduleListPageDataContext(taskManagementDomain);
            var scheduleListPage = new ScheduleListPage();
            mainWindow.SubscribePageDataContext(scheduleListPage, scheduleListPageDataContext);

            var taskAddWindowDataContext = new TaskAddWindowDataContext(taskManagementDomain);

            taskManagementPage.Subscribe(taskManagementDomain.OpenAddTaskWindowEventPublisher.Subscribe(__instanceTaskAddWindow));
            void __instanceTaskAddWindow()
            {
                var taskAddWindow = new TaskAddWindow();

                taskAddWindow.Topmost = true;

                taskAddWindow.SubscribeWindowDataContext(taskAddWindowDataContext);
                taskAddWindow.Subscribe(taskAddWindowDataContext.TaskAddCommand.Subscribe(taskAddWindow.Close));
                taskAddWindow.Show();
            }

            taskManagementPage.Initialize(taskEditorPage, taskListPage, scheduleListPage);

            TaskManagementPage = taskManagementPage;
        }
    }
}
