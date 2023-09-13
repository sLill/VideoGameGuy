using Microsoft.EntityFrameworkCore;

namespace VideoGameShowdown.Data
{
    public class GamesRepository : RepositoryBase, IGamesRepository
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public GamesRepository(ILogger<GamesRepository> logger, ApplicationDbContext applicationDbContext)
            : base(logger, applicationDbContext) { }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateGameAsync(RawgGame rawgGame)
        {
            bool success = true;

            try
            {
                Game gameData = await _applicationDbContext.Games.FirstOrDefaultAsync(x => x.RawgId.HasValue && x.RawgId == rawgGame.id).ConfigureAwait(false);
                
                // Add
                if (gameData == default)
                {
                    gameData = new Game()
                    {
                        RawgId = rawgGame.id,
                        Name = rawgGame.name,
                        ReleaseDate = rawgGame.released,
                        ImageUri = rawgGame.background_image,
                        ReviewScore = rawgGame.rating,
                        ReviewMaxScore = rawgGame.rating_top,
                        MetacriticScore = rawgGame.metacritic,
                        AveragePlaytime = rawgGame.playtime,
                        UpdatedOn = rawgGame.updated,
                        ReviewCount = rawgGame.reviews_count,
                        PlayerbaseProgress = rawgGame.added_by_status != null
                            ? new PlayerbaseProgress() { OwnTheGame = rawgGame.added_by_status.owned, BeatTheGame = rawgGame.added_by_status.beaten }: null,
                        Screenshots = rawgGame.short_screenshots.Select(x => new Screenshot() { Source = "RAWG", SourceId = x.id, Uri = x.image })?.ToList()
                    };

                    await _applicationDbContext.AddAsync(gameData).ConfigureAwait(false);
                }

                // Update
                else
                {
                    gameData.RawgId = rawgGame.id;
                    gameData.Name = rawgGame.name;
                    gameData.ReleaseDate = rawgGame.released;
                    gameData.ImageUri = rawgGame.background_image;
                    gameData.ReviewScore = rawgGame.rating;
                    gameData.ReviewMaxScore = rawgGame.rating_top;
                    gameData.MetacriticScore = rawgGame.metacritic;
                    gameData.AveragePlaytime = rawgGame.playtime;
                    gameData.UpdatedOn = rawgGame.updated;
                    gameData.ReviewCount = rawgGame.reviews_count;

                    if (rawgGame.added_by_status != null)
                    {
                        var playerbaseProgressData = await _applicationDbContext.PlayerbaseProgress.FirstOrDefaultAsync(x => x.GameId == gameData.GameId).ConfigureAwait(false);

                        gameData.PlayerbaseProgress = gameData.PlayerbaseProgress ?? new PlayerbaseProgress();
                        gameData.PlayerbaseProgress.OwnTheGame = rawgGame.added_by_status.owned;
                        gameData.PlayerbaseProgress.BeatTheGame = rawgGame.added_by_status.beaten;
                    }

                    if (rawgGame.short_screenshots != null)
                    {
                        gameData.Screenshots = gameData.Screenshots ?? new List<Screenshot>();
                        rawgGame.short_screenshots.ForEach(rawgScreenshot =>
                        {
                            var screenshot = gameData.Screenshots.FirstOrDefault(x => x.SourceId != null && x.Source == "RAWG" && x.SourceId == rawgScreenshot.id);
                            if (screenshot == default)
                                gameData.Screenshots.Add(new Screenshot() { Source = "RAWG", SourceId = rawgScreenshot.id, Uri = rawgScreenshot.image });
                            else
                                screenshot.Uri = rawgScreenshot.image; 
                        });
                    }

                    _applicationDbContext.Update(gameData);
                }

                await _applicationDbContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            return success;
        }
        #endregion Methods..
    }
}
