namespace DailyProject_221204
{
    public interface ISaveDataHandler<T>
        where T : class
    {
        bool ExistSaveFile();
        bool ExistSaveDirectory();

        void SetValue(T data);
        T GetValue();
        SaveReadResult GetReadResult();
        SaveWriteResult GetWriteResult();
    }
}
