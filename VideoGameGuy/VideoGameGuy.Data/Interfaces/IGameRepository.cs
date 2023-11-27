namespace VideoGameGuy.Data
{
    public interface IGameRepository
    {
        #region Methods..
        Task<Game?> GetGameAsync(Guid sessionId, GameType gameType);
        Task<bool> AddAsync(Game game);
        Task<bool> UpdateAsync(Game game);
        Task AddOrUpdateAsync(Game game);
        #endregion Methods..
    }
}
