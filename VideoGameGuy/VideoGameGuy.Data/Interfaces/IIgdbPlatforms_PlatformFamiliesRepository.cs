namespace VideoGameGuy.Data
{
    public interface IIgdbPlatforms_PlatformFamiliesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbPlatforms_PlatformFamilies> igdbPlatforms_PlatformFamilies, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
