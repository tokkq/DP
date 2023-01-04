using System;
using System.Windows.Input;

namespace DailyProject_221204
{
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
            MoveEventCommand = new StandardCommand(__move);
            void __move(object? param)
            {
                if (param == null)
                {
                    return;
                }
                if (int.TryParse(param.ToString(), out var offsetSeconds))
                {
                    var startAt = model.StartAt.AddSeconds(offsetSeconds);
                    var endAt = model.EndAt.AddSeconds(offsetSeconds);

                    UpdateScheduleItemPeriod(startAt, endAt);
                }
            }

            ChangeItemPeriodCommand = new StandardCommand(__changeItemPeriod);
            void __changeItemPeriod(object? param)
            {
                if (param == null)
                {
                    return;
                }
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
}