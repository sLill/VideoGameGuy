namespace VideoGameGuy.Data
{
    public interface IIgdbPlatformFamiliesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies);
        #endregion Methods..
    }
}
