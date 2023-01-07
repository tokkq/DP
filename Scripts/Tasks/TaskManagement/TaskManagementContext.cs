using DailyProject_221204.Scripts;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace DailyProject_221204
{

    public class TaskManagementContext : IDisposable
    {
        const int AUTO_SAVE_INTERVAL_SECOND = 3;

        readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer();

        public DPContext DPContext { get; } = null!;

        public EventPublisher<TaskModel> SelectEventPublisher = new();
        public EventPublisher<TaskModel> StatusUpdateEventPublisher = new();
        public EventPublisher<TaskModel> AddScheduleEventPublisher = new();
        public EventPublisher<TaskModel> AddTaskEventPublisher = new();
        public EventPublisher OpenAddTaskWindowEventPublisher = new();

        public TaskManagementContext(DPContext dpContext)
        {
            _autoSaveTimer.Interval = TimeSpan.FromSeconds(AUTO_SAVE_INTERVAL_SECOND);
            _autoSaveTimer.Start();

            DPContext = dpContext;
        }

        public void Dispose()
        {
            _autoSaveTimer.Stop();
        }

        public IDisposable SubscribeAutoSave(Action saveProcess)
        {
            _autoSaveTimer.Tick += (a, b) => saveProcess();
            return new ActionDisposer(() => _autoSaveTimer.Tick -= (a, b) => saveProcess());
        }
    }
}
