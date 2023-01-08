using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DailyProject_221204
{
    public class TaskListPageDataContext : AbstractTaskManagementPageDataContext
    {
        public IEnumerable<TaskListItemViewModel> ActiveTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Active);
        public IEnumerable<TaskListItemViewModel> WaitTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Wait);
        public IEnumerable<TaskListItemViewModel> IdeaTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Idea);
        public IEnumerable<TaskListItemViewModel> CompleteTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Complete);

        public StandardCommand AddTaskCommand { get; set; } = null!;

        TaskListItems _tasks = new();
        ISaveDataHandler<List<TaskModel>> _taskSaveDataHandler= null!;

        public TaskListPageDataContext(TaskManagementContext taskManagementContext) : base(taskManagementContext)
        {
            AddTaskCommand = new StandardCommand(__openAddTaskWindow);
            void __openAddTaskWindow(object? param)
            {
                _taskManagementContext.OpenAddTaskWindowEventPublisher.Publish();
            }

            _addViewProperty(nameof(ActiveTasks));
            _addViewProperty(nameof(WaitTasks));
            _addViewProperty(nameof(IdeaTasks));
            _addViewProperty(nameof(CompleteTasks));

            _taskSaveDataHandler = _registerSaveData<List<TaskModel>>(PathDefinition.TaskJsonDirectoryPath, "Tasks");
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _addUnloadDispose(_tasks);
            _addUnloadDispose(_tasks.SubscribeCollectionChange(() => _notifyUpdateView()));
            _addUnloadDispose(_tasks.SubscribeSelect(vm => _taskManagementContext.SelectEventPublisher.Publish(vm.Model)));
            _addUnloadDispose(_tasks.SubscribeAddSchedule(vm => _taskManagementContext.AddScheduleEventPublisher.Publish(vm.Model)));

            _addUnloadDispose(_taskManagementContext.StatusUpdateEventPublisher.Subscribe(vm => _notifyUpdateView()));
            _addUnloadDispose(_taskManagementContext.AddTaskEventPublisher.Subscribe(_addTask));

            _loadTask();
        }

        protected override void _onUnloaded()
        {
            base._onUnloaded();

            _saveTask();
        }

        void _loadTask()
        {
            _tasks.Clear();
            foreach (var model in _taskSaveDataHandler.GetValue())
            {
                _addTask(model);
            }
        }
        void _saveTask()
        {
            var taskModels = _tasks.Select(vm => vm.Model).ToList();
            _taskSaveDataHandler.SetValue(taskModels);
        }
        void _onAutoSave()
        {
            _saveTask();
            WriteSaveData();
        }
        void _addTask(TaskModel taskModel)
        {
            var viewModel = new TaskListItemViewModel(taskModel);
            _tasks.Add(viewModel);
            _notifyUpdateView();
        }
    }
}
