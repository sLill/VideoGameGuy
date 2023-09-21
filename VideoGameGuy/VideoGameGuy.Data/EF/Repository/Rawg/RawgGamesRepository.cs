using Microsoft.EntityFrameworkCore;
using VideoGameGuy.Common;

namespace VideoGameGuy.Data
{
    public class RawgGamesRepository : RepositoryBase, IRawgGamesRepository
    {
        #region Fields..
        protected readonly RawgDbContext _rawgDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public RawgGamesRepository(ILogger<RawgGamesRepository> logger, 
                                   RawgDbContext rawgDbContext)
            : base(logger) 
        { 
            _rawgDbContext = rawgDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<Game> GetGameFromGameIdAsync(Guid gameId)
        {
            Game game = default;

            try
            {
                game = await _rawgDbContext.Games.Include("PlayerbaseProgress").Include("Screenshots").Include("Ratings")
                    .FirstOrDefaultAsync(x => x.GameId == gameId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return game;
        }

        public async Task<Game> GetGameFromRawgIdAsync(int rawgId)
        {
            Game game = default;

            try
            {
                game = await _rawgDbContext.Games.Include("PlayerbaseProgress").Include("Screenshots").Include("Ratings")
                    .FirstOrDefaultAsync(x => x.RawgId.HasValue && x.RawgId == rawgId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return game;
        }

        public async Task<List<Game>> GetRandomGamesAsync(int numberOfGames)
        {
            List<Game> games = default;
            int minimumNumberOfRatings = 750;

            try
            {
                if (_rawgDbContext.Games.Count() >= numberOfGames)
                {
                    var eligibleGames = await (from x in _rawgDbContext.Games.Include("Screenshots")
                                               where x.MetacriticScore != null
                                                  && x.RatingsCount != null
                                                  && x.RatingsCount >= minimumNumberOfRatings
                                               select x).ToListAsync();

                    games = eligibleGames.TakeRandom(numberOfGames);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return games;
        }

        private async Task<bool> AddAsync(RawgGame rawgGame)
        {
            bool success = true;

            try
            {
                var game = new Game(rawgGame);
                await _rawgDbContext.AddAsync(game).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _rawgDbContext.SaveChangesAsync().ConfigureAwait(false);

            return success;
        }

        public async Task<bool> UpdateAsync(Game gameData, RawgGame rawgGame)
        {
            bool success = true;

            try
            {
                gameData.UpdateFromRawgGame(rawgGame);
                _rawgDbContext.Update(gameData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _rawgDbContext.SaveChangesAsync().ConfigureAwait(false);

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
                        _rawgDbContext.Games.Add(new Game(rawgGame));

                    // Update
                    else
                    {
                        gameData.UpdateFromRawgGame(rawgGame);
                        _rawgDbContext.Games.Update(gameData);
                    }
                }

                await _rawgDbContext.SaveChangesAsync().ConfigureAwait(false);
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
