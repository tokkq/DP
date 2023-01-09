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
        public TodayReflectionPage()
        {
            InitializeComponent();
        }
    }

    public class TodayReflectionPageDataContext : AbstractPageDataContext
    {
        ISaveDataHandler<DayReflectionModel> _yesterdaySaveDataHandler = null!;
        ISaveDataHandler<DayReflectionModel> _todaySaveDataHandler = null!;

        readonly DPDomain _dpContext = null!;

        public DayReflectionViewModel YesterdayReflection { get; private set; } = null!;
        public DayReflectionViewModel TodayReflection { get; private set; } = null!;

        public StandardCommand SwitchPageToTaskManagementPageCommand { get; } = null!;
        public StandardCommand SwitchPageToWeekReflectionPageCommand { get; } = null!;

        public TodayReflectionPageDataContext(DPDomain dpContext)
        {
            _dpContext = dpContext;

            var reflectionModel = new DayReflectionModel();
            var reflectionViewModel = new DayReflectionViewModel(reflectionModel);

            SwitchPageToTaskManagementPageCommand = new StandardCommand(o => _dpContext.SwitchPage(PageType.TaskManagement));
            SwitchPageToWeekReflectionPageCommand = new StandardCommand(o => _dpContext.SwitchPage(PageType.WeekReflection));

            var yesterdaySaveFileName = $"{DateTime.Today.AddDays(-1).ToString(PathDefinition.DateFormat)}";
            _yesterdaySaveDataHandler = _registerSaveData<DayReflectionModel>(PathDefinition.DayReflectionsDirectoryPath, yesterdaySaveFileName);

            var todaySaveFileName = $"{DateTime.Today.ToString(PathDefinition.DateFormat)}";
            _todaySaveDataHandler = _registerSaveData<DayReflectionModel>(PathDefinition.DayReflectionsDirectoryPath, todaySaveFileName);

            _addViewProperty(nameof(YesterdayReflection));
            _addViewProperty(nameof(TodayReflection));
        }

        protected override void _onLoaded()
        {
            base._onLoaded();

            YesterdayReflection = _instanceDayReflectionViewModel(_yesterdaySaveDataHandler.GetValue());

            var todayModel = new DayReflectionModel()
            {
                Date = DateTime.Today,
            };
            if (_todaySaveDataHandler.GetReadResult() == SaveReadResult.Success)
            {
                todayModel = _todaySaveDataHandler.GetValue();
            }
            else
            {
                _todaySaveDataHandler.SetValue(todayModel);
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
