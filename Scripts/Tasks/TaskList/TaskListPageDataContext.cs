using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DailyProject_221204
{
    public class TaskListPageDataContext : AbstractPageDataContext
    {
        public IEnumerable<TaskListItemViewModel> ActiveTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Active);
        public IEnumerable<TaskListItemViewModel> WaitTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Wait);
        public IEnumerable<TaskListItemViewModel> IdeaTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Idea);
        public IEnumerable<TaskListItemViewModel> CompleteTasks => _tasks.Where(vm => vm.Model.Status.StatusType == TaskStatusType.Complete);

        public StandardCommand AddTaskCommand { get; set; } = null!;

        TaskListItems _tasks = new();
        ISaveDataHandler<List<TaskModel>> _taskSaveDataHandler= null!;

        public TaskListPageDataContext(TaskManagementPageDataContext context)
        {
            AddTaskCommand = new StandardCommand(__addNewTask);
            void __addNewTask(object? param)
            {
                var item = new TaskListItemViewModel(new TaskModel()
                {
                    Name = "NewTask",
                    Description = "",
                    StartAt = DateTime.Now,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                });

                DPDebug.WriteLine($"[Name: {item.Model.Name}]AddNewTask");

                _tasks.Add(item);
            }

            _unloadDisposables.Add(_tasks);
            _unloadDisposables.Add(_tasks.SubscribeCollectionChange(() => _notifyUpdateView()));
            _unloadDisposables.Add(_tasks.SubscribeSelect(vm => context.SelectEventPublisher.Publish(vm.Model)));
            _unloadDisposables.Add(_tasks.SubscribeAddSchedule(vm => context.AddScheduleEventPublisher.Publish(vm.Model)));

            context.AddAutoSave(WriteSaveData);
            _unloadDisposables.Add(context.StatusUpdateEventPublisher.Subscribe(vm => _notifyUpdateView()));

            _taskSaveDataHandler = _registerSaveData<List<TaskModel>>(PathDefinition.TaskJsonDirectoryPath, "Tasks");

            _addViewProperty(nameof(ActiveTasks));
            _addViewProperty(nameof(WaitTasks));
            _addViewProperty(nameof(IdeaTasks));
            _addViewProperty(nameof(CompleteTasks));
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _tasks.Clear();
            foreach (var model in _taskSaveDataHandler.GetValue())
            {
                var viewModel = new TaskListItemViewModel(model);
                _tasks.Add(viewModel);
            }

            _notifyUpdateView();
        }

        protected override void _onUnloaded()
        {
            base._onUnloaded();

            var taskModels = _tasks.Select(vm => vm.Model).ToList();
            _taskSaveDataHandler.SetValue(taskModels);
        }
    }
}
