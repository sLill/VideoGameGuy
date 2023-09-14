namespace VideoGameCritic.Data
{
    public interface ISystemStatusRepository
    {
        #region Methods..
        SystemStatus GetCurrentStatus();

        Task UpdateAsync(SystemStatus status);
        #endregion Methods..
    }
}
