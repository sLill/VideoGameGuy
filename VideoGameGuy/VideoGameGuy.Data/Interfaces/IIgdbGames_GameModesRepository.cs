namespace VideoGameGuy.Data
{
    public interface IIgdbGames_GameModesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_GameModes> igdbGames_GameModes, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbGames_GameModes> igdbGames_GameModes);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
