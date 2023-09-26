namespace VideoGameGuy.Data
{
    public interface IIgdbGames_GameModesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_GameModes> igdbGames_GameModes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
