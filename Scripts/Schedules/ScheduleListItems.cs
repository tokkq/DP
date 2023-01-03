using System;

namespace DailyProject_221204
{
    public class ScheduleListItems : AbstractBindableCollection<ScheduleListItemViewModel>
    {
        readonly EventPublisher<TaskModel> _selectEventPublisher = new();

        public IDisposable SubscribeSelect(Action<TaskModel> action) => _selectEventPublisher.Subscribe(action);

        protected override void __bind(ScheduleListItemViewModel item)
        {
            base.__bind(item);

            _subscribe(item, new[]
            {
                item.RemoveEventCommand.Subscribe(() => Remove(item)),
                item.SelectEventCommand.Subscribe(() => _selectEventPublisher.Publish(item.Model.BindTask)),
            });
        }
    }
}
