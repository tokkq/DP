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

        readonly static string _taskJsonFilePath = Path.Combine(PathDefinition.TaskJsonDirectoryPath, "Tasks.json");

        TaskListItems _tasks = new();

        public TaskListPageDataContext(TaskManagementPageDataContext context)
        {
            AddTaskCommand = new StandardCommand(_addNewTask);

            _subscribe(_tasks);
            _subscribe(_tasks.SubscribeCollectionChange(() => _notifyUpdateView()));
            _subscribe(_tasks.SubscribeSelect(vm => context.SelectEventPublisher.Publish(vm.Model)));
            _subscribe(_tasks.SubscribeAddSchedule(vm => context.AddScheduleEventPublisher.Publish(vm.Model)));

            context.AddAutoSave(_saveTasks);
            _subscribe(context.StatusUpdateEventPublisher.Subscribe(vm => _notifyUpdateView()));

            _addViewProperty(nameof(ActiveTasks));
            _addViewProperty(nameof(WaitTasks));
            _addViewProperty(nameof(IdeaTasks));
            _addViewProperty(nameof(CompleteTasks));

            _loadTasks();
            _notifyUpdateView();
        }

        void _saveTasks()
        {
            var taskModels = _tasks.Select(vm => vm.Model).ToList();
            JsonUtility.SaveJson(taskModels, _taskJsonFilePath);
        }
        void _loadTasks()
        {
            var taskModels = JsonUtility.LoadJson<List<TaskModel>>(_taskJsonFilePath, shouldCreateNewFileIfNoExistJson:true);

            _tasks.Clear();
            foreach (var model in taskModels)
            {
                var viewModel = new TaskListItemViewModel(model);
                _tasks.Add(viewModel);
            }
        }

        void _addNewTask(object? param)
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
    }
}
