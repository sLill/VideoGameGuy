namespace VideoGameGuy.Data
{
    public interface IIgdbGames_MultiplayerModesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_MultiplayerModes> igdbGames_MultiplayerModes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
