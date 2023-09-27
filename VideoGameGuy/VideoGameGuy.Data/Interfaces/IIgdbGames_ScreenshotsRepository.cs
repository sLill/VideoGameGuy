namespace VideoGameGuy.Data
{
    public interface IIgdbGames_ScreenshotsRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Screenshots> igdbGames_Screenshots, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbGames_Screenshots> igdbGames_Screenshots);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
