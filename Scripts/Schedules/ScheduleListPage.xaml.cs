using DailyProject_221204.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace DailyProject_221204
{
    /// <summary>
    /// ScheduleListPage.xaml の相互作用ロジック
    /// </summary>
    public partial class ScheduleListPage : Page
    {
        public ScheduleListPage()
        {
            InitializeComponent();
        }
    }

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

    public class ScheduleModel : AbstractModel
    {
        /// <summary>
        /// スケジュールの名前
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// タスク開始予定日時
        /// </summary>
        public DateTime StartAt { get; set; } = DateTime.MinValue;
        /// <summary>
        /// タスク終了予定日時
        /// </summary>
        public DateTime EndAt { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 紐づけられたタスク
        /// </summary>
        public TaskModel BindTask { get; set; } = new();
    }

    public class ScheduleListItemViewModel : AbstractViewModel<ScheduleModel>
    {
        const int SCHEDULE_PERIOD_UNDER_LIMIT_SECONDS = 1800;

        public string ScheduleListItemName => _getName();

        public string LeftSpaceRateText => $"{_getLeftSpaceRate():F2}*";
        public string ScheduleSpaceRateText => $"{_getScheduleSpaceRate():F2}*";
        public string RightSpaceRateText => $"{_getRightSpaceRate():F2}*";

        public EventCommand RemoveEventCommand { get; } = new();
        public EventCommand SelectEventCommand { get; } = new();
        public ICommand MoveEventCommand { get; } = null!;
        public ICommand ChangeItemPeriodCommand { get; } = null!;

        DateTime _startAtSchedule = DateTime.MinValue;
        DateTime _endAtSchedule = DateTime.MinValue;

        TimeSpan _totalScheduleSpan = TimeSpan.Zero;
        TimeSpan _leftSpaceSpan = TimeSpan.Zero;
        TimeSpan _scheduleSpaceSpan = TimeSpan.Zero;
        TimeSpan _rightSpaceSpan = TimeSpan.Zero;

        public ScheduleListItemViewModel(ScheduleModel model) : base(model)
        {
            MoveEventCommand = new StandardCommand<int>(__move);
            void __move(int param)
            {
                if(int.TryParse(param.ToString(), out var offsetSeconds))
                {
                    var startAt = model.StartAt.AddSeconds(offsetSeconds);
                    var endAt = model.EndAt.AddSeconds(offsetSeconds);

                    UpdateScheduleItemPeriod(startAt, endAt);
                }
            }

            ChangeItemPeriodCommand = new StandardCommand<int>(__changeItemPeriod);
            void __changeItemPeriod(int param)
            {
                if (int.TryParse(param.ToString(), out var addSeconds))
                {
                    var startAt = model.StartAt;
                    var endAt = model.EndAt.AddSeconds(addSeconds);

                    UpdateScheduleItemPeriod(startAt, endAt);
                }
            }
        }

        /// <summary>
        /// スケジュール全体の期間をセットし、スケジュールアイテムのView設定を行う。
        /// </summary>
        public void UpdateSchedulePeriod(DateTime startAt, DateTime endAt)
        {
            _startAtSchedule = startAt;
            _endAtSchedule = endAt;

            _updateSpaceSpan();
        }

        /// <summary>
        /// スケジュールアイテムの期間をセットし、View設定を行う。
        /// </summary>
        public void UpdateScheduleItemPeriod(DateTime startAt, DateTime endAt)
        {
            Model.StartAt = startAt;
            Model.EndAt = endAt;

            _updateSpaceSpan();
        }

        void _updateSpaceSpan()
        {
            _validateScheduleItemPeriod();

            _totalScheduleSpan = _endAtSchedule - _startAtSchedule;
            _leftSpaceSpan = Model.StartAt - _startAtSchedule;
            _scheduleSpaceSpan = Model.EndAt - Model.StartAt;
            _rightSpaceSpan = _endAtSchedule - Model.EndAt;

            _onPropertyChanged(nameof(LeftSpaceRateText));
            _onPropertyChanged(nameof(ScheduleSpaceRateText));
            _onPropertyChanged(nameof(RightSpaceRateText));
        }

        void _validateScheduleItemPeriod()
        {
            var period = Model.EndAt - Model.StartAt;
            if (period.TotalSeconds < SCHEDULE_PERIOD_UNDER_LIMIT_SECONDS)
            {
                Model.EndAt = Model.EndAt.AddSeconds(SCHEDULE_PERIOD_UNDER_LIMIT_SECONDS);
            }

            var underLimitDateTime = _startAtSchedule;

            if (Model.StartAt < underLimitDateTime)
            {
                Model.StartAt = underLimitDateTime;
            }
            if (Model.EndAt < underLimitDateTime)
            {
                Model.EndAt = underLimitDateTime;
            }

            // endAtScheduleだと、Viewとの整合性が取れなくなるため-1
            var overLimitDateTime = _endAtSchedule.AddHours(-1);

            if (overLimitDateTime < Model.StartAt)
            {
                Model.StartAt = overLimitDateTime;
            }
            if (overLimitDateTime < Model.EndAt)
            {
                Model.EndAt = overLimitDateTime;
            }
        }

        float _getLeftSpaceRate()
        {
            var rate = (float)(_leftSpaceSpan / _totalScheduleSpan);
            return rate;
        }
        float _getScheduleSpaceRate()
        {
            var rate = (float)(_scheduleSpaceSpan / _totalScheduleSpan);
            return rate;
        }
        float _getRightSpaceRate()
        {
            var rate = (float)(_rightSpaceSpan / _totalScheduleSpan);
            return rate;
        }

        string _getName() 
        {
            if (Model.BindTask != null)
            {
                return Model.BindTask.Name;
            }
            else
            {
                return Model.Name;
            }
        }
    }

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
