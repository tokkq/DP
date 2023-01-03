using System;

namespace DailyProject_221204
{
    public class WeekReflectionPageDataContext : AbstractPageDataContext
    {
        static readonly string _dayReflectionJsonFileNameFormat = "yyyyMMdd";

        ISaveDataHandler<WeekReflectionModel> _lastWeekSaveDataHandler = null!;
        ISaveDataHandler<WeekReflectionModel> _thisWeekSaveDataHandler = null!;

        readonly DPMainWindowDataContext _dPMainWindowDataContext = null!;

        public WeekReflectionViewModel LastWeekReflection { get; private set; } = null!;
        public WeekReflectionViewModel ThisWeekReflection { get; private set; } = null!;

        public StandardCommand SwitchPageToTaskManagement { get; } = null!;
        public StandardCommand SwitchPageToTodayReflectionPage { get; } = null!;

        public WeekReflectionPageDataContext(DPMainWindowDataContext dpMainWindowDataContext)
        {
            _dPMainWindowDataContext = dpMainWindowDataContext;

            var reflectionModel = new WeekReflectionModel();
            var reflectionViewModel = new WeekReflectionViewModel(reflectionModel);

            SwitchPageToTaskManagement = new StandardCommand(a => _dPMainWindowDataContext.SwitchPage(PageType.TaskManagement));
            SwitchPageToTodayReflectionPage = new StandardCommand(a => _dPMainWindowDataContext.SwitchPage(PageType.TodayReflection));
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            var lastWeekSaveFileName = _getWeekJsonFileName(DateTime.Today.AddDays(-7));
            _lastWeekSaveDataHandler = _registerSaveData<WeekReflectionModel>(PathDefinition.WeekReflectionsJsonDirectoryPath, lastWeekSaveFileName);

            LastWeekReflection = _instanceWeekReflectionViewModel(_lastWeekSaveDataHandler.GetValue());

            var thisWeekSaveFileName = _getWeekJsonFileName(DateTime.Today);
            _thisWeekSaveDataHandler = _registerSaveData<WeekReflectionModel>(PathDefinition.WeekReflectionsJsonDirectoryPath, thisWeekSaveFileName);

            var thisWeekModel = new WeekReflectionModel()
            {
                WeekStartDate = DateTime.Today.GetFirstDayOfWeek(),
                WeekEndDate = DateTime.Today.GetLastDayOfWeek(),
            };
            if (_thisWeekSaveDataHandler.GetReadResult() == SaveReadResult.Success)
            {
                thisWeekModel = _thisWeekSaveDataHandler.GetValue();
            }
            else
            {
                _thisWeekSaveDataHandler.SetValue(thisWeekModel);
            }

            ThisWeekReflection = _instanceWeekReflectionViewModel(thisWeekModel);
        }

        string _getWeekJsonFileName(DateTime day)
        {
            var weekStartDayText = day.GetFirstDayOfWeek().ToString(_dayReflectionJsonFileNameFormat);
            var weekEndDayText = day.GetLastDayOfWeek().ToString(_dayReflectionJsonFileNameFormat);
            var thisWeekFileName = $"{weekStartDayText}_{weekEndDayText}";
            return thisWeekFileName;
        }

        WeekReflectionViewModel _instanceWeekReflectionViewModel(WeekReflectionModel model)
        {
            var viewModel = new WeekReflectionViewModel(model);
            return viewModel;
        }
    }
}
