namespace FinanceTracker
{
    public interface IRecordFileStorage
    {
        Task SaveToJsonFileAsync(string path);
        Task LoadFromJsonFileAsync(string path);
    }
}
