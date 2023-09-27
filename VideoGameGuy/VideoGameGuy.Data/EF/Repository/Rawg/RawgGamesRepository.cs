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
            : base(logger, rawgDbContext) 
        { 
            _rawgDbContext = rawgDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<RawgGame> GetGameFromGameIdAsync(Guid gameId)
        {
            RawgGame game = default;

            try
            {
                game = await _rawgDbContext.Games.Include("PlayerbaseProgress").Include("Screenshots").Include("Ratings")
                    .FirstOrDefaultAsync(x => x.RawgGameId == gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return game;
        }

        public async Task<RawgGame> GetGameFromRawgIdAsync(int rawgId)
        {
            RawgGame game = default;

            try
            {
                game = await _rawgDbContext.Games.Include("PlayerbaseProgress").Include("Screenshots").Include("Ratings")
                    .FirstOrDefaultAsync(x => x.SourceId.HasValue && x.SourceId == rawgId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return game;
        }

        public async Task<List<RawgGame>> GetRandomGamesAsync(int numberOfGames)
        {
            List<RawgGame> games = default;
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

        private async Task<bool> AddAsync(RawgApiGame rawgGame)
        {
            bool success = true;

            try
            {
                var game = new RawgGame(rawgGame);
                await _rawgDbContext.AddAsync(game);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _rawgDbContext.SaveChangesAsync();

            return success;
        }

        public async Task<bool> UpdateAsync(RawgGame gameData, RawgApiGame rawgGame)
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

            await _rawgDbContext.SaveChangesAsync();

            return success;
        }
        
        public async Task<bool> AddOrUpdateAsync(RawgApiGame rawgGame)
        {
            bool success = true;

            try
            {
                RawgGame gameData = await GetGameFromRawgIdAsync(rawgGame.id);

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

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<RawgApiGame> rawgGames)
        {
            bool success = true;

            try 
            {
                foreach (var rawgGame in rawgGames)
                {
                    RawgGame gameData = await GetGameFromRawgIdAsync(rawgGame.id);

                    // Add
                    if (gameData == default)
                        _rawgDbContext.Games.Add(new RawgGame(rawgGame));

                    // Update
                    else
                    {
                        gameData.UpdateFromRawgGame(rawgGame);
                        _rawgDbContext.Games.Update(gameData);
                    }
                }

                await _rawgDbContext.SaveChangesAsync();
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
