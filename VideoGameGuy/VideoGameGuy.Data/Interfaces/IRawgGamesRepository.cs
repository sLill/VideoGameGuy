namespace VideoGameGuy.Data
{
    public interface IRawgGamesRepository
    {
        #region Methods..
        Task<RawgGame> GetGameFromGameIdAsync(Guid gameId);

        Task<RawgGame> GetGameFromRawgIdAsync(int rawgId);
        
        Task<List<RawgGame>> GetRandomGamesAsync(int numberOfGames);

        Task<bool> AddOrUpdateAsync(RawgApiGame rawgGame);

        Task<bool> AddOrUpdateRangeAsync(IEnumerable<RawgApiGame> rawgGames);
        #endregion Methods..
    }
}
