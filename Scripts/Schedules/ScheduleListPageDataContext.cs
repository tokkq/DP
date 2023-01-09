using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DailyProject_221204
{
    public class ScheduleListPageDataContext : AbstractTaskManagementPageDataContext
    {
        const int SCHEDULE_START_AT = 9;
        const int SCHEDULE_END_AT = 18;

        public DateTime DisplayDay 
        {
            get
            {
                return _displayDay;
            }
            set
            {
                _displayDay = value;
                _updateScheduleDay(_displayDay);
            }
        }
        public IEnumerable<ScheduleListItemViewModel> DisplaySchedules => _getDisplaySchedules();

        public ScheduleTimeHeaderItems TimeHeaders { get; } = new();

        ScheduleListItems _schedules { get; } = new();
        DateTime _displayDay = DateTime.Today;
        TaskManagementDomain _taskManagementContext = null!;
        ISaveDataHandler<List<ScheduleModel>> _saveDataHandler = null!;

        public ScheduleListPageDataContext(TaskManagementDomain taskManagementContext) : base(taskManagementContext)
        {
            _addViewProperty(nameof(DisplayDay));
            _addViewProperty(nameof(DisplaySchedules));

            _saveDataHandler = _registerSaveData<List<ScheduleModel>>(PathDefinition.SchedulesDirectoryPath, "Schedule");

            _taskManagementContext = taskManagementContext;
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            _loadSchedule();

            _addUnloadDispose(_schedules);
            _addUnloadDispose(_schedules.SubscribeCollectionChange(_notifyUpdateView));
            _addUnloadDispose(_schedules.SubscribeSelect(_taskManagementContext.SelectEventPublisher.Publish));
            _addUnloadDispose(_taskManagementContext.AddScheduleEventPublisher.Subscribe(_addSchedule));

            _updateScheduleDay(_displayDay);
        }

        protected override void _onUnloaded()
        {
            base._onUnloaded();

            _saveSchedule();
        }

        void _saveSchedule()
        {
            var taskModels = _schedules.Select(vm => vm.Model).ToList();
            _saveDataHandler.SetValue(taskModels);
        }
        void _loadSchedule()
        {
            _schedules.Clear();
            foreach (var model in _saveDataHandler.GetValue())
            {
                var viewModel = new ScheduleListItemViewModel(model);
                _schedules.Add(viewModel);
            }
        }

        void _updateScheduleDay(DateTime day)
        {
            var startAt = day.AddHours(SCHEDULE_START_AT);
            var endAt = day.AddHours(SCHEDULE_END_AT).AddHours(1);

            foreach (var schedule in DisplaySchedules)
            {
                schedule.UpdateSchedulePeriod(startAt, endAt);
            }

            _updateTimeHeaderItems(startAt, endAt);

            _notifyUpdateView();
        }

        IEnumerable<ScheduleListItemViewModel> _getDisplaySchedules()
        {
            var schedules = _schedules.Where(s =>
            {
                DPDebug.WriteLine($"[s.Model.StartAt.Date: {s.Model.StartAt.Date}][DisplayDay: {DisplayDay}]");
                return s.Model.StartAt.Date == DisplayDay;
            });
            return schedules;
        }

        void _updateTimeHeaderItems(DateTime startAt, DateTime endAt)
        {
            TimeHeaders.Clear();

            var duration = endAt - startAt;
            var durationHour = duration.Hours;
            var startOclock = startAt.Hour;
            for (int i = 0; i < durationHour; i++)
            {
                var model = new ScheduleTimeHeaderModel()
                {
                    Name = $"{i + startOclock}"
                };
                TimeHeaders.Add(new ScheduleTimeHeaderViewModel(model));
            }
        }

        void _addSchedule(TaskModel taskModel)
        {
            var scheduleModel = new ScheduleModel()
            {
                Name = taskModel.Name,
                StartAt = _displayDay.AddHours(SCHEDULE_START_AT),
                EndAt = _displayDay.AddHours(SCHEDULE_START_AT + 1),
                BindTask = taskModel,
            };

            _addSchedule(scheduleModel);
        }
        void _addSchedule(ScheduleModel scheduleModel)
        {
            var scheduleViewModel = new ScheduleListItemViewModel(scheduleModel);

            _schedules.Add(scheduleViewModel);

            _updateScheduleDay(_displayDay);
        }
    }
}
