namespace DailyProject_221204
{
    public interface ISaveDataHandler<T>
        where T : class
    {
        void SetValue(T data);
        T GetValue();
    }
}
