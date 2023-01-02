using System;
using System.Collections.Generic;

namespace DailyProject_221204
{
    public class TaskModel : AbstractModel
    {
        /// <summary>
        /// タスクの名前
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// タスクの説明
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// タスクの状態
        /// </summary>
        public TaskStatusModel Status { get; set; } = new();
        /// <summary>
        /// タスクに関するコメント
        /// </summary>
        public List<CommentModel> Comments { get; set; } = new();

        /// <summary>
        /// 私にとっての重要度
        /// </summary>
        public TaskPriority MyPriority { get; set; } = TaskPriority.Unimportant;
        /// <summary>
        /// 私以外の人にとっての重要度
        /// </summary>
        public TaskPriority OtherPriority { get; set; } = TaskPriority.Unimportant;

        /// <summary>
        /// タスク作成日時
        /// </summary>
        public DateTime CreateAt { get; set; } = DateTime.MinValue;
        /// <summary>
        /// タスク更新日時
        /// </summary>
        public DateTime UpdateAt { get; set; } = DateTime.MinValue;
        /// <summary>
        /// タスク開始予定日時
        /// </summary>
        public DateTime StartAt { get; set; } = DateTime.MinValue;
        /// <summary>
        /// タスク完了日時
        /// </summary>
        public DateTime CompleteAt { get; set; } = DateTime.MinValue;
    }

    public class TaskStatusModel : AbstractModel
    {
        /// <summary>
        /// タスクの状態
        /// </summary>
        public TaskStatusType StatusType { get; set; } = TaskStatusType.Active;
    }

    public enum TaskStatusType
    {
        /// <summary>
        /// 対応中
        /// </summary>
        Active,
        /// <summary>
        /// 待ち
        /// </summary>
        Wait,
        /// <summary>
        /// アイデアレベル
        /// </summary>
        Idea,
        /// <summary>
        /// 完了済み
        /// </summary>
        Complete,
    }
}
