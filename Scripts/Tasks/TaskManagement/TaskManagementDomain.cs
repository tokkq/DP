using DailyProject_221204.Scripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace DailyProject_221204
{

    public class TaskManagementDomain : IDisposable
    {
        const int AUTO_SAVE_INTERVAL_SECOND = 3;

        public DPDomain DPDomain { get; } = null!;

        public EventPublisher<TaskModel> SelectEventPublisher { get; } = new();
        public EventPublisher<TaskModel> StatusUpdateEventPublisher { get; } = new();
        public EventPublisher<TaskModel> AddScheduleEventPublisher { get; } = new();
        public EventPublisher<TaskModel> AddTaskEventPublisher { get; } = new();
        public EventPublisher OpenAddTaskWindowEventPublisher { get; } = new();

        public IEnumerable<TaskModel> Tasks => _tasks;

        readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer();

        readonly List<TaskModel> _tasks = new List<TaskModel>();

        public TaskManagementDomain(DPDomain dpDomain)
        {
            _autoSaveTimer.Interval = TimeSpan.FromSeconds(AUTO_SAVE_INTERVAL_SECOND);
            _autoSaveTimer.Start();

            DPDomain = dpDomain;
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

        public void AddTask(TaskModel task)
        {
            _tasks.Add(task);
            AddTaskEventPublisher.Publish(task);
        }
        public void RemoveTask(TaskModel task) 
        {
            _tasks.Remove(task);
        }
        public void AddNewTask()
        {
            var model = new TaskModel()
            {
                StartAt = DateTime.Now,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };
            AddTask(model);
        }
        public void ClearTask()
        {
            _tasks.Clear();
        }
    }
}
