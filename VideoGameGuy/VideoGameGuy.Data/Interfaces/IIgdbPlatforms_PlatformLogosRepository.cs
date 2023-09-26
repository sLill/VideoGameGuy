namespace VideoGameGuy.Data
{
    public interface IIgdbPlatforms_PlatformLogosRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbPlatforms_PlatformLogos> igdbPlatforms_PlatformLogos, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
