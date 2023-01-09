using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
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
                DPDebug.Assert($"WindowInteropHelperの取得に失敗しました。");
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

            var taskManagementController = new TaskManagementController(dpDomain, mainWindow);
            var reflectionController = new ReflectionController(dpDomain, mainWindow);
            var dailyMemoController = new DailyMemoController(dpDomain, mainWindow);

            _pageMap[(int)PageType.TaskManagement] = taskManagementController.TaskManagementPage;
            _pageMap[(int)PageType.TodayReflection] = reflectionController.TodayReflectionPage;
            _pageMap[(int)PageType.WeekReflection] = reflectionController.WeekReflectionPage;

            mainWindow.SwitchPage(reflectionController.TodayReflectionPage);
        }
    }
}
