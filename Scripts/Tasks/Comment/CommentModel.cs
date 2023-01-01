using System;

namespace DailyProject_221204
{
    public class CommentModel : AbstractModel
    {
        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get; set; } = string.Empty;
        /// <summary>
        /// コメント作成日時
        /// </summary>
        public DateTime CreateAt { get; set; } = DateTime.MinValue;
    }
}
