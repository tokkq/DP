namespace DailyProject_221204
{
    public class CommentViewModel : AbstractViewModel<CommentModel>
    {
        public string CreateAtText => Model.CreateAt.ToString("G");

        public EventCommand RemoveEventCommand { get; } = new();

        public CommentViewModel(CommentModel model) : base(model)
        {
        }

        protected override void _onSetModel(CommentModel model)
        {
            base._onSetModel(model);

            _onPropertyChanged(nameof(CreateAtText));
        }
    }
}
