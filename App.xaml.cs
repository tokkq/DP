using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DailyProject_221204
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly Dictionary<int, Page> _pageMap = new();

        bool _isFirstActivated = true;

        private void Application_Activated(object sender, EventArgs e)
        {
            if(_isFirstActivated == true)
            {
                _initialize();
                _isFirstActivated = false;
            }
        }

        void _initialize()
        {
            DPDebug.WriteLine("Application run.");

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var dpContext = new DPContext();

            var mainWindowDataContext = new MainWindowDataContext(dpContext);
            var mainWindow = (MainWindow)MainWindow;
            if (mainWindow == null)
            {
                throw new InvalidCastException($"MainWindowの取得に失敗しました。");
            }
            mainWindow.SubscribeWindowDataContext(mainWindowDataContext);

            dpContext.SubscribeSwitchPage(__onSwitchPage);
            void __onSwitchPage(PageType pageType)
            {
                if (_pageMap.TryGetValue((int)pageType, out var page))
                {
                    mainWindow.SwitchPage(page);
                }
            }

            var taskManagementPage = _instanceTaskManagementPage(dpContext);
            var todayReflectionPage = _instanceTodayReflectionPage(dpContext);
            var weekReflectionPage = _instanceWeekReflectionPage(dpContext);

            mainWindow.SwitchPage(todayReflectionPage);
        }

        TaskManagementPage _instanceTaskManagementPage(DPContext dpContext)
        {
            var taskManagementContext = new TaskManagementContext(dpContext);

            var taskManagementPageDataContext = new TaskManagementPageDataContext(taskManagementContext);
            var taskManagementPage = new TaskManagementPage();
            _subscribePage(taskManagementPage, taskManagementPageDataContext);

            var taskEditorDataContext = new TaskEditorPageDataContext(taskManagementContext);
            var taskEditorPage = new TaskEditorPage();
            _subscribePage(taskEditorPage, taskEditorDataContext);

            var taskListPageDataContext = new TaskListPageDataContext(taskManagementContext);
            var taskListPage = new TaskListPage();
            _subscribePage(taskListPage, taskListPageDataContext);

            var scheduleListPageDataContext = new ScheduleListPageDataContext(taskManagementContext);
            var scheduleListPage = new ScheduleListPage();
            _subscribePage(scheduleListPage, scheduleListPageDataContext);

            var taskAddWindowDataContext = new TaskAddWindowDataContext(taskManagementContext);

            taskManagementPage.Subscribe(taskManagementContext.OpenAddTaskWindowEventPublisher.Subscribe(__instanceTaskAddWindow));
            void __instanceTaskAddWindow()
            {
                var taskAddWindow = new TaskAddWindow();
                taskAddWindow.SubscribeWindowDataContext(taskAddWindowDataContext);
                taskAddWindow.Subscribe(taskAddWindowDataContext.TaskAddCommand.Subscribe(taskAddWindow.Close));
                taskAddWindow.Show();
            }

            taskManagementPage.Initialize(taskEditorPage, taskListPage, scheduleListPage);

            _pageMap[(int)PageType.TaskManagement] = taskManagementPage;

            return taskManagementPage;
        }

        TodayReflectionPage _instanceTodayReflectionPage(DPContext dpContext)
        {
            var todayReflectionPage = new TodayReflectionPage();
            var todayReflectionDataContext = new TodayReflectionPageDataContext(dpContext);
            _subscribePage(todayReflectionPage, todayReflectionDataContext);

            _pageMap[(int)PageType.TodayReflection] = todayReflectionPage;

            return todayReflectionPage;
        }

        WeekReflectionPage _instanceWeekReflectionPage(DPContext dpContext)
        {
            var weekReflectionPage = new WeekReflectionPage();
            var weekReflectionDataContext = new WeekReflectionPageDataContext(dpContext);
            _subscribePage(weekReflectionPage, weekReflectionDataContext);

            _pageMap[(int)PageType.WeekReflection] = weekReflectionPage;

            return weekReflectionPage;
        }

        void _subscribePage(Page page, AbstractPageDataContext context)
        {
            MainWindow.SubscribePageDataContext(page, context);
        }
    }
}
