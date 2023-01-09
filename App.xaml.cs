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
using System.Windows.Interop;

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

            var mainWindow = (MainWindow)MainWindow;
            if (mainWindow == null)
            {
                DPDebug.Assert($"MainWindowの取得に失敗しました。");
                return;
            }
            
            var host = new WindowInteropHelper(mainWindow);
            if (host == null)
            {
                DPDebug.Assert($"MainWindowの取得に失敗しました。");
                return;
            }

            var globalHotKey = new GlobalHotKey(host.Handle);
            
            var dpDomain = new DPDomain(globalHotKey);

            dpDomain.SubscribeSwitchPage(__onSwitchPage);
            void __onSwitchPage(PageType pageType)
            {
                if (_pageMap.TryGetValue((int)pageType, out var page))
                {
                    mainWindow.SwitchPage(page);
                }
            }

            var mainWindowDataContext = new MainWindowDataContext(dpDomain);

            mainWindow.SubscribeWindowDataContext(mainWindowDataContext);
            mainWindow.Subscribe(dpDomain);

            var taskManagementPage = _instanceTaskManagementPage(dpDomain);
            var todayReflectionPage = _instanceTodayReflectionPage(dpDomain);
            var weekReflectionPage = _instanceWeekReflectionPage(dpDomain);

            mainWindow.SwitchPage(todayReflectionPage);
        }

        TaskManagementPage _instanceTaskManagementPage(DPDomain dpContext)
        {
            // TaskManagementPageにサブスクライブするのが自然そうだが、初期化をどこで行うかを検討する必要がある。
            var taskManagementDomain = new TaskManagementDomain(dpContext);
            MainWindow.Subscribe(taskManagementDomain);

            var taskManagementPageDataContext = new TaskManagementPageDataContext(taskManagementDomain);
            var taskManagementPage = new TaskManagementPage();
            _subscribePage(taskManagementPage, taskManagementPageDataContext);

            var taskEditorDataContext = new TaskEditorPageDataContext(taskManagementDomain);
            var taskEditorPage = new TaskEditorPage();
            _subscribePage(taskEditorPage, taskEditorDataContext);

            var taskListPageDataContext = new TaskListPageDataContext(taskManagementDomain);
            var taskListPage = new TaskListPage();
            _subscribePage(taskListPage, taskListPageDataContext);

            var scheduleListPageDataContext = new ScheduleListPageDataContext(taskManagementDomain);
            var scheduleListPage = new ScheduleListPage();
            _subscribePage(scheduleListPage, scheduleListPageDataContext);

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

            _pageMap[(int)PageType.TaskManagement] = taskManagementPage;

            return taskManagementPage;
        }

        TodayReflectionPage _instanceTodayReflectionPage(DPDomain dpContext)
        {
            var todayReflectionPage = new TodayReflectionPage();
            var todayReflectionDataContext = new TodayReflectionPageDataContext(dpContext);
            _subscribePage(todayReflectionPage, todayReflectionDataContext);

            _pageMap[(int)PageType.TodayReflection] = todayReflectionPage;

            return todayReflectionPage;
        }

        WeekReflectionPage _instanceWeekReflectionPage(DPDomain dpContext)
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
