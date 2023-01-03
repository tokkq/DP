using DailyProject_221204.Scripts;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace DailyProject_221204
{
    public class TaskManagementPageDataContext : AbstractPageDataContext
    {
        const int AUTO_SAVE_INTERVAL_SECOND = 3;

        readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer();

        public EventPublisher<TaskModel> SelectEventPublisher = new();
        public EventPublisher<TaskModel> StatusUpdateEventPublisher = new();
        public EventPublisher<TaskModel> AddScheduleEventPublisher = new();

        public TaskManagementPageDataContext()
        {
            _autoSaveTimer.Interval = TimeSpan.FromSeconds(AUTO_SAVE_INTERVAL_SECOND);
            _autoSaveTimer.Start();
            _unloadDisposables.Add(new ActionDisposer(_autoSaveTimer.Stop));    
        }

        public void AddAutoSave(Action saveProcess)
        {
            _autoSaveTimer.Tick += (a, b) => saveProcess();
        }
    }
}
