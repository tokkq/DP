namespace DailyProject_221204
{
    public class CommentListItems : AbstractBindableCollection<CommentViewModel>
    {
        protected override void __bind(CommentViewModel item)
        {
            _subscribe(item, new[]
            {
                item.RemoveEventCommand.Subscribe(() => Remove(item)),
            });
        }
    }
}
