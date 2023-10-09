namespace VideoGameGuy.Data
{
    public interface IIgdbGamesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateAsync(IgdbApiGame apiGame, bool suspendSaveChanges = false);
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames, bool suspendSaveChanges = false);

        Task<List<IgdbGame>> GetGamesWithStorylines(int minimumNumberOfRatings);
        Task<List<IgdbGame>> GetGamesWithArtwork(int artworkCount);
        Task<List<IgdbGame>> GetGamesWithScreenshots(int screenshotCount);
        Task<List<IgdbArtwork>> GetArtworkFromGameAsync(IgdbGame game);
        Task<List<IgdbScreenshot>> GetScreenshotsFromGameAsync(IgdbGame game);
        #endregion Methods..
    }
}
