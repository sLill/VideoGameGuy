namespace VideoGameGuy.Data
{
    public interface IIgdbPlatformFamiliesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies);
        #endregion Methods..
    }
}
