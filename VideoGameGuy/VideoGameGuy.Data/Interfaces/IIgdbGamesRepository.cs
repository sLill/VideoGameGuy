namespace VideoGameGuy.Data
{
    public interface IIgdbGamesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames);
        #endregion Methods..
    }
}
