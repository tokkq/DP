using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DailyProject_221204
{
    /// <summary>
    /// WeekReflectionPage.xaml の相互作用ロジック
    /// </summary>
    public partial class WeekReflectionPage : Page
    {
        static readonly string _dayReflectionJsonFileNameFormat = "yyyyMMdd";

        readonly DPMainWindowDataContext _dPMainWindowDataContext = null!;

        public WeekReflectionViewModel LastWeekReflection { get; private set; } = null!;
        public WeekReflectionViewModel ThisWeekReflection { get; private set; } = null!;

        public StandardCommand SwitchPageToNextCommand { get; } = null!;

        public WeekReflectionPage(DPMainWindowDataContext dpMainWindowDataContext)
        {
            InitializeComponent();
            DataContext = this;

            _dPMainWindowDataContext = dpMainWindowDataContext;

            var reflectionModel = new WeekReflectionModel();
            var reflectionViewModel = new WeekReflectionViewModel(reflectionModel);

            SwitchPageToNextCommand = new StandardCommand(_onSwitchPageCommand);

            _loadWeekReflection();
        }

        void _onSwitchPageCommand(object? parameter)
        {
            _saveWeekReflection();
            _dPMainWindowDataContext.SwitchPage(PageType.TaskManagement);
        }

        void _saveWeekReflection()
        {
            var thisWeekFileName = _getWeekJsonFileName(DateTime.Today);
            var savePath = Path.Combine(PathDefinition.WeekReflectionsJsonDirectoryPath, thisWeekFileName);
            JsonUtility.SaveJson(ThisWeekReflection.Model, savePath);
        }

        void _loadWeekReflection()
        {
            var lastWeekFileName = _getWeekJsonFileName(DateTime.Today.AddDays(-7));
            var lastWeekReflectionFilePath = Path.Combine(PathDefinition.WeekReflectionsJsonDirectoryPath, lastWeekFileName);
            var lastWeekModel = new WeekReflectionModel();
            if (File.Exists(lastWeekReflectionFilePath) == true)
            {
                lastWeekModel = JsonUtility.LoadJson<WeekReflectionModel>(lastWeekReflectionFilePath);
            }
            LastWeekReflection = _instanceWeekReflectionViewModel(lastWeekModel);

            var thisWeekFileName = _getWeekJsonFileName(DateTime.Today);
            var thisWeekReflectionFilePath = Path.Combine(PathDefinition.WeekReflectionsJsonDirectoryPath, thisWeekFileName);
            var thisWeekModel = new WeekReflectionModel()
            {
                WeekStartDate = DateTime.Today.GetFirstDayOfWeek(),
                WeekEndDate = DateTime.Today.GetLastDayOfWeek(),
            };
            if (File.Exists(thisWeekFileName) == true)
            {
                thisWeekModel = JsonUtility.LoadJson<WeekReflectionModel>(thisWeekReflectionFilePath);
            }
            ThisWeekReflection = _instanceWeekReflectionViewModel(thisWeekModel);
        }

        string _getWeekJsonFileName(DateTime day)
        {
            var weekStartDayText = day.GetFirstDayOfWeek().ToString(_dayReflectionJsonFileNameFormat);
            var weekEndDayText = day.GetLastDayOfWeek().ToString(_dayReflectionJsonFileNameFormat);
            var thisWeekFileName = $"{weekStartDayText}_{weekEndDayText}.json";
            return thisWeekFileName;
        }

        WeekReflectionViewModel _instanceWeekReflectionViewModel(WeekReflectionModel model)
        {
            var viewModel = new WeekReflectionViewModel(model);
            return viewModel;
        }
    }

    public class WeekReflectionModel : AbstractModel
    {
        /// <summary>
        /// 先週の目標を達成できたか？
        /// </summary>
        public float LastWeekTargetCompleteRate { get; set; } = 0f;
        /// <summary>
        /// 良かった点
        /// </summary>
        public string LastWeekTargetReflectionText { get; set; } = string.Empty;

        /// <summary>
        /// 良かった点
        /// </summary>
        public string GoodPointText { get; set; } = string.Empty;
        /// <summary>
        /// 変更点
        /// </summary>
        public string ChangePointText { get; set; } = string.Empty;
        /// <summary>
        /// 今週の目標
        /// </summary>
        public string TargetText { get; set; } = string.Empty;

        /// <summary>
        /// 週の開始日
        /// </summary>
        public DateTime WeekStartDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 週の終了日
        /// </summary>
        public DateTime WeekEndDate { get; set; } = DateTime.MinValue;
    }
    public class WeekReflectionViewModel : AbstractViewModel<WeekReflectionModel>
    {
        public WeekReflectionViewModel(WeekReflectionModel model) : base(model)
        {
        }
    }
}
