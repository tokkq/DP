using System;

namespace DailyProject_221204
{
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
}
