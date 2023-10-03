namespace VideoGameGuy.Data
{
    public interface IIgdbGamesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateAsync(IgdbApiGame apiGame, bool suspendSaveChanges = false);
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames, bool suspendSaveChanges = false);

        Task<List<IgdbGame>> GetGamesWithStorylines(int minimumNumberOfRatings);
        Task<IgdbArtwork> GetArtworkFromGameAsync(IgdbGame game);
        Task<IgdbScreenshot> GetScreenshotFromGameAsync(IgdbGame game);
        #endregion Methods..
    }
}
