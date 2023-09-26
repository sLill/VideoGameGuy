namespace VideoGameGuy.Data
{
    public interface IIgdbGamesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateAsync(IgdbApiGame apiGame, bool suspendSaveChanges = false);

        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames);
        #endregion Methods..
    }
}
