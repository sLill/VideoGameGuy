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
        public async Task<Game> GetGameFromRawgIdAsync(int rawgId)
        {
            Game game = default;

            try
            {
                game = await _applicationDbContext.Games.Include("PlayerbaseProgress").Include("Screenshots")
                    .FirstOrDefaultAsync(x => x.RawgId.HasValue && x.RawgId == rawgId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return game;
        }

        private async Task<bool> AddAsync(RawgGame rawgGame)
        {
            bool success = true;

            try
            {
                var game = new Game(rawgGame);
                await _applicationDbContext.AddAsync(game).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _applicationDbContext.SaveChangesAsync().ConfigureAwait(false);

            return success;
        }

        public async Task<bool> UpdateAsync(Game gameData, RawgGame rawgGame)
        {
            bool success = true;

            try
            {
                gameData.UpdateFromRawgGame(rawgGame);
                _applicationDbContext.Update(gameData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _applicationDbContext.SaveChangesAsync().ConfigureAwait(false);

            return success;
        }
        
        public async Task<bool> AddOrUpdateAsync(RawgGame rawgGame)
        {
            bool success = true;

            try
            {
                Game gameData = await GetGameFromRawgIdAsync(rawgGame.id).ConfigureAwait(false);

                // Add
                if (gameData == default)
                    success = await AddAsync(rawgGame);

                // Update
                else
                    success = await UpdateAsync(gameData, rawgGame);

            }
            catch (Exception ex) 
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            return success;
        }

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<RawgGame> rawgGames)
        {
            bool success = true;

            try 
            {
                foreach (var rawgGame in rawgGames)
                {
                    Game gameData = await GetGameFromRawgIdAsync(rawgGame.id).ConfigureAwait(false);

                    // Add
                    if (gameData == default)
                        _applicationDbContext.Games.Add(new Game(rawgGame));

                    // Update
                    else
                    {
                        gameData.UpdateFromRawgGame(rawgGame);
                        _applicationDbContext.Games.Update(gameData);
                    }
                }

                await _applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
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
