namespace VideoGameGuy.Data
{
    public interface IIgdbGames_ScreenshotsRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Screenshots> igdbGames_Screenshots, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
