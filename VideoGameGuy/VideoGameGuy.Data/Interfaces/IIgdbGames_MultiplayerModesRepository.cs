namespace VideoGameGuy.Data
{
    public interface IIgdbGames_MultiplayerModesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_MultiplayerModes> igdbGames_MultiplayerModes, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbGames_MultiplayerModes> igdbGames_MultiplayerModes);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
