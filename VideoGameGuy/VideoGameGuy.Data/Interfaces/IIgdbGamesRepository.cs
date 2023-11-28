namespace VideoGameGuy.Data
{
    public interface IIgdbGamesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateAsync(IgdbApiGame apiGame, bool suspendSaveChanges = false);
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames, bool suspendSaveChanges = false);

        Task<List<IgdbGame>> GetGamesWithStorylines(int minimumNumberOfRatings);
        List<IgdbGame> GetGamesWithArtwork(int artworkCount, int minTotalRatingCount = 0);
        List<IgdbGame> GetGamesWithScreenshots(int screenshotCount, int minTotalRatingCount = 0);
        List<IgdbArtwork> GetArtworkFromGame(IgdbGame game);
        List<IgdbScreenshot> GetScreenshotsFromGame(IgdbGame game);
        #endregion Methods..
    }
}
