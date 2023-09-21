namespace VideoGameGuy.Data
{
    public interface ISystemStatusRepository
    {
        #region Methods..
        Task<SystemStatus> GetCurrentStatusAsync();
        Task<bool> AddAsync(SystemStatus systemStatus);
        Task<bool> UpdateAsync(SystemStatus systemStatus);
        #endregion Methods..
    }
}
