using System;

namespace DailyProject_221204
{
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
}
