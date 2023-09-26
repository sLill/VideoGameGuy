namespace VideoGameGuy.Data
{
    public interface IIgdbGames_PlatformsRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Platforms> igdbGames_Platforms, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
