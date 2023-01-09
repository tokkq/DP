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
        public IEnumerable<TaskListItemViewModel> ActiveTasks => _taskListItems.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Active);
        public IEnumerable<TaskListItemViewModel> WaitTasks => _taskListItems.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Wait);
        public IEnumerable<TaskListItemViewModel> IdeaTasks => _taskListItems.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Idea);
        public IEnumerable<TaskListItemViewModel> CompleteTasks => _taskListItems.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Complete);

        public StandardCommand AddTaskCommand { get; set; } = null!;

        TaskListItems _taskListItems = new();

        public TaskListPageDataContext(TaskManagementDomain taskManagementDomain) : base(taskManagementDomain)
        {
            AddTaskCommand = new StandardCommand(__openAddTaskWindow);
            void __openAddTaskWindow(object? param)
            {
                _taskManagementDomain.OpenAddTaskWindowEventPublisher.Publish();
            }

            _addViewProperty(nameof(ActiveTasks));
            _addViewProperty(nameof(WaitTasks));
            _addViewProperty(nameof(IdeaTasks));
            _addViewProperty(nameof(CompleteTasks));

        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _addUnloadDispose(_taskListItems);
            _addUnloadDispose(_taskListItems.SubscribeCollectionChange(() => _notifyUpdateView()));
            _addUnloadDispose(_taskListItems.SubscribeSelect(vm => _taskManagementDomain.SelectEventPublisher.Publish(vm.Model)));
            _addUnloadDispose(_taskListItems.SubscribeAddSchedule(vm => _taskManagementDomain.AddScheduleEventPublisher.Publish(vm.Model)));
            _addUnloadDispose(_taskListItems.SubscribeRemove(vm => _taskManagementDomain.RemoveTask(vm.Model)));

            _addUnloadDispose(_taskManagementDomain.StatusUpdateEventPublisher.Subscribe(vm => _notifyUpdateView()));
            _addUnloadDispose(_taskManagementDomain.AddTaskEventPublisher.Subscribe(_addTask));

            foreach (var model in _taskManagementDomain.Tasks)
            {
                _addTask(model);
            }
        }

        void _addTask(TaskModel taskModel)
        {
            var viewModel = new TaskListItemViewModel(taskModel);
            _taskListItems.Add(viewModel);
            _notifyUpdateView();
        }
    }
}
