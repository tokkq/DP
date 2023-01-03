using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DailyProject_221204
{
    public class ScheduleListPageDataContext : AbstractPageDataContext
    {
        const int SCHEDULE_START_AT = 9;
        const int SCHEDULE_END_AT = 18;

        readonly static string _scheduleJsonFilePath = Path.Combine(PathDefinition.SchedulesJsonDirectoryPath, "Schedule.json");

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

        public ScheduleListPageDataContext(TaskManagementPageDataContext taskManagementPageDataContext)
        {
            taskManagementPageDataContext.AddAutoSave(_saveSchedule);

            _loadSchedule();

            _unloadDisposables.Add(_schedules);
            _unloadDisposables.Add(_schedules.SubscribeCollectionChange(_notifyUpdateView));
            _unloadDisposables.Add(_schedules.SubscribeSelect(taskManagementPageDataContext.SelectEventPublisher.Publish));
            _unloadDisposables.Add(taskManagementPageDataContext.AddScheduleEventPublisher.Subscribe(_addSchedule));

            _addViewProperty(nameof(DisplayDay));
            _addViewProperty(nameof(DisplaySchedules));

            _updateScheduleDay(_displayDay);
            _notifyUpdateView();
        }

        void _saveSchedule()
        {
            var taskModels = _schedules.Select(vm => vm.Model).ToList();
            JsonUtility.SaveJson(taskModels, _scheduleJsonFilePath);
        }
        void _loadSchedule()
        {
            var models = JsonUtility.LoadJson<List<ScheduleModel>>(_scheduleJsonFilePath, shouldCreateNewFileIfNoExistJson: true);

            _schedules.Clear();
            foreach (var model in models)
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
