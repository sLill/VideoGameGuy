namespace VideoGameGuy.Data
{
    public interface IIgdbPlatformsRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatform> apiPlatforms, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
