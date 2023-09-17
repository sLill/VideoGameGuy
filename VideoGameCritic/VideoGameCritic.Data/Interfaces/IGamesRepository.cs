namespace VideoGameCritic.Data
{
    public interface IGamesRepository
    {
        #region Methods..
        Task<Game> GetGameFromGameIdAsync(Guid gameId);

        Task<Game> GetGameFromRawgIdAsync(int rawgId);
        
        Task<List<Game>> GetRandomGamesAsync(int numberOfGames);

        Task<bool> AddOrUpdateAsync(RawgGame rawgGame);

        Task<bool> AddOrUpdateRangeAsync(IEnumerable<RawgGame> rawgGames);
        #endregion Methods..
    }
}
