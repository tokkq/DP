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
    /// TodayReflectionPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TodayReflectionPage : Page
    {
        static readonly string _dayReflectionJsonFileNameFormat = "yyyyMMdd";

        readonly DPMainWindowDataContext _dPMainWindowDataContext = null!;

        public DayReflectionViewModel YesterdayReflection { get; private set; } = null!;
        public DayReflectionViewModel TodayReflection { get; private set; } = null!;

        public StandardCommand SwitchPageToNextCommand { get; } = null!;

        public TodayReflectionPage(DPMainWindowDataContext dpMainWindowDataContext)
        {
            InitializeComponent();
            DataContext = this;

            _dPMainWindowDataContext = dpMainWindowDataContext;

            var reflectionModel = new DayReflectionModel();
            var reflectionViewModel = new DayReflectionViewModel(reflectionModel);

            SwitchPageToNextCommand = new StandardCommand(_onSwitchPageCommand);

            _loadDayReflection();
        }

        void _onSwitchPageCommand(object? parameter)
        {
            _saveDayReflection();
            _dPMainWindowDataContext.SwitchPage(PageType.TaskManagement);
        }

        void _saveDayReflection()
        {
            var todayFileName = $"{DateTime.Today.ToString(_dayReflectionJsonFileNameFormat)}.json";
            var savePath = Path.Combine(PathDefinition.DayReflectionsJsonDirectoryPath, todayFileName);
            JsonUtility.SaveJson(TodayReflection.Model, savePath);
        }

        void _loadDayReflection()
        {
            var yesterdayFileName = $"{DateTime.Today.AddDays(-1).ToString(_dayReflectionJsonFileNameFormat)}.json";
            var yesterdayReflectionFilePath = Path.Combine(PathDefinition.DayReflectionsJsonDirectoryPath, yesterdayFileName);
            var yesterdayModel = new DayReflectionModel();
            if (File.Exists(yesterdayReflectionFilePath) == true)
            {
                yesterdayModel = JsonUtility.LoadJson<DayReflectionModel>(yesterdayReflectionFilePath);
            }
            YesterdayReflection = _instanceDayReflectionViewModel(yesterdayModel);

            var todayFileName = $"{DateTime.Today.ToString(_dayReflectionJsonFileNameFormat)}.json";
            var todayReflectionFilePath = Path.Combine(PathDefinition.DayReflectionsJsonDirectoryPath, todayFileName);
            var todayModel = new DayReflectionModel()
            {
                Date = DateTime.Today,
            };
            if (File.Exists(todayReflectionFilePath) == true)
            {
                todayModel = JsonUtility.LoadJson<DayReflectionModel>(todayReflectionFilePath);
            }
            TodayReflection = _instanceDayReflectionViewModel(todayModel);
        }

        DayReflectionViewModel _instanceDayReflectionViewModel(DayReflectionModel model)
        {
            var viewModel = new DayReflectionViewModel(model);
            return viewModel;
        }
    }

    public class DayReflectionModel : AbstractModel
    {
        /// <summary>
        /// 昨日の目標を達成できたか？
        /// </summary>
        public float YesterdayTargetCompleteRate { get; set; } = 0f;
        /// <summary>
        /// 良かった点
        /// </summary>
        public string YesterdayTargetReflectionText { get; set; } = string.Empty;

        /// <summary>
        /// 昨日の目標を達成できたか？
        /// </summary>
        public float WeekTargetCompleteRate { get; set; } = 0f;
        /// <summary>
        /// 良かった点
        /// </summary>
        public string WeekTargetReflectionText { get; set; } = string.Empty;

        /// <summary>
        /// 良かった点
        /// </summary>
        public string GoodPointText { get; set; } = string.Empty;
        /// <summary>
        /// 変更点
        /// </summary>
        public string ChangePointText { get; set; } = string.Empty;
        /// <summary>
        /// 今日の目標
        /// </summary>
        public string TargetText { get; set; } = string.Empty;

        /// <summary>
        /// 振り返り日
        /// </summary>
        public DateTime Date { get; set; } = DateTime.MinValue;
    }
    public class DayReflectionViewModel : AbstractViewModel<DayReflectionModel>
    {
        public DayReflectionViewModel(DayReflectionModel model) : base(model)
        {
        }
    }
}
