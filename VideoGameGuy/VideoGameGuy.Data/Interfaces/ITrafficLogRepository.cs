namespace VideoGameGuy.Data
{
    public interface ITrafficLogRepository
    {
        #region Methods..
        Task<bool> AddAsync(TrafficLog trafficLog);
        #endregion Methods..
    }
}
