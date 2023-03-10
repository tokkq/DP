using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyProject_221204
{
    public class TaskListItems : AbstractBindableCollection<TaskListItemViewModel>
    {
        readonly EventPublisher<TaskListItemViewModel> _selectEventPublisher = new();
        readonly EventPublisher<TaskListItemViewModel> _changeCompleteEventPublisher = new();
        readonly EventPublisher<TaskListItemViewModel> _addScheduleEventPublisher = new();

        TaskListItemViewModel? _selectedItem = null;

        public IDisposable SubscribeSelect(Action<TaskListItemViewModel> action) => _selectEventPublisher.Subscribe(action);
        public IDisposable SubscribeChangeStatus(Action<TaskListItemViewModel> action) => _changeCompleteEventPublisher.Subscribe(action);
        public IDisposable SubscribeAddSchedule(Action<TaskListItemViewModel> action) => _addScheduleEventPublisher.Subscribe(action);

        protected override void __bind(TaskListItemViewModel item)
        {
            _subscribe(item, new[]
            {
                item.RemoveEventCommand.Subscribe(() => Remove(item)),
                item.SelectEventCommand.Subscribe(() => _select(item)),
                item.ChangeStatusEventCommand.Subscribe(__onStatusChange),
                item.AddScheduleEventCommand.Subscribe(() => _addScheduleEventPublisher.Publish(item)),
            });

            void __onStatusChange()
            {
                _sort();
                _changeCompleteEventPublisher.Publish(item);
            }
        }


        void _select(TaskListItemViewModel item) 
        {
            DPDebug.WriteLine($"[Name: {item.Model.Name}] Select");

            var prevSelectedItem = _selectedItem;
            if (prevSelectedItem != null)
            {
                _unselect(prevSelectedItem);
            }

            item.TurnOnHighlight();

            _selectedItem = item;

            _selectEventPublisher.Publish(_selectedItem);
        }
        void _unselect(TaskListItemViewModel item)
        {
            DPDebug.WriteLine($"[Name: {item.Model.Name}] Unselect");

            item.TurnOffHighlight();
        }

        protected override IEnumerable<TaskListItemViewModel> _sort(IEnumerable<TaskListItemViewModel> items)
        {
            items = items
                .OrderBy(vm => vm.Model.MyPriority)
                .ThenBy(vm => vm.Model.OtherPriority)
                .ThenBy(vm => vm.Model.StartAt);

            return items;
        }
    }
}
