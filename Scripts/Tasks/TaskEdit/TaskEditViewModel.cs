using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DailyProject_221204
{
    public class TaskEditViewModel : AbstractViewModel<TaskModel>
    {
        public IEnumerable<TaskPriority> TaskPriorityOption { get; } = new[]
        {
            TaskPriority.MostImportant,
            TaskPriority.Important,
            TaskPriority.Unimportant,
        };
        public IEnumerable<TaskStatusType> TaskStatusItems { get; } = new[]
        {
            TaskStatusType.Active,
            TaskStatusType.Wait,
            TaskStatusType.Idea,
            TaskStatusType.Complete,
        };

        public string CreateAtText => Model.CreateAt.ToString("G");
        public string UpdateAtText => Model.UpdateAt.ToString("G");
        public string CompleteAtText => Model.CompleteAt.ToString("G");

        public CommentListItems Comments { get; } = new();

        public StandardCommand AddCommentCommand { get; } = null!;
        public EventCommand ChangeStatusCommand { get; } = new();

        public TaskEditViewModel(TaskModel model) : base(model)
        {
            AddCommentCommand = new StandardCommand(_addNewComment);
        }

        protected override void _onSetModel(TaskModel model)
        {
            base._onSetModel(model);

            Comments.Clear();
            foreach (var commentModel in Model.Comments)
            {
                var commentViewModel = new CommentViewModel(commentModel);
                Comments.Add(commentViewModel);
            }

            _onPropertyChanged(nameof(CreateAtText));
            _onPropertyChanged(nameof(UpdateAtText));
            _onPropertyChanged(nameof(CompleteAtText));
        }

        void _addNewComment(object? param)
        {
            var commentModel = new CommentModel()
            {
                CreateAt = DateTime.Now,
            };
            Model.Comments.Add(commentModel);

            var commentViewModel = new CommentViewModel(commentModel);
            Comments.Add(commentViewModel);
        }
    }
}
