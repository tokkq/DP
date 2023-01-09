using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DailyProject_221204
{
    public class TaskManagementPageDataContext : AbstractTaskManagementPageDataContext
    {
        public StandardCommand AddTaskCommand { get; } = null!;

        public IDisposable SubscribeOpenAddTaskWindow(Action action) => _taskManagementDomain.OpenAddTaskWindowEventPublisher.Subscribe(action);

        ISaveDataHandler<List<TaskModel>>? _taskSaveDataHandler = null;

        public TaskManagementPageDataContext(TaskManagementDomain taskManagementDomain) : base(taskManagementDomain)
        {
            AddTaskCommand = new StandardCommand(o => _publishOpenAddTaskWindowEvent());
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _addUnloadDispose(_taskManagementDomain.DPContext.GlobalHotKey.SubscribeGlobalHotKey(_publishOpenAddTaskWindowEvent, Key.Q, ModifierKeys.Alt));

            _taskSaveDataHandler = _registerSaveData<List<TaskModel>>(PathDefinition.TaskJsonDirectoryPath, "Tasks");
            _loadTask();
        }

        protected override void _onUnloaded()
        {
            _saveTask();
        }

        void _publishOpenAddTaskWindowEvent()
        {
            _taskManagementDomain.OpenAddTaskWindowEventPublisher.Publish();
        }

        void _loadTask()
        {
            if(_taskSaveDataHandler != null )
            {
                _taskManagementDomain.ClearTask();
                foreach (var model in _taskSaveDataHandler.GetValue())
                {
                    _taskManagementDomain.AddTask(model);
                }
            }
            else
            {
                DPDebug.Assert("_taskSaveDataHandler is null.");
            }
        }
        void _saveTask()
        {
            if (_taskSaveDataHandler != null)
            {
                var taskModels = _taskManagementDomain.Tasks.ToList();
                _taskSaveDataHandler.SetValue(taskModels);
            }
        }
    }
}
