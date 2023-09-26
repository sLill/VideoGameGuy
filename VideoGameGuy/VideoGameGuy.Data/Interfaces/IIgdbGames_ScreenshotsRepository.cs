namespace VideoGameGuy.Data
{
    public interface IIgdbGames_ScreenshotsRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Screenshots> igdbGames_Screenshots, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
