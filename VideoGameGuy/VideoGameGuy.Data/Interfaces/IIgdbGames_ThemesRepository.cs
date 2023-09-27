namespace VideoGameGuy.Data
{
    public interface IIgdbGames_ThemesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Themes> igdbGames_Themes, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbGames_Themes> igdbGames_Themes);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
