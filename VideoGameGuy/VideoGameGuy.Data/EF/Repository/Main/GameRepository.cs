using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class GameRepository : RepositoryBase, IGameRepository
    {
        #region Fields..
        private readonly MainDbContext _mainDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public GameRepository(ILogger<RawgGamesRepository> logger,
                                     MainDbContext mainDbContext)
           : base(logger, mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }
        #endregion Constructors..

        #region Methods..		
        public async Task<Game?> GetGameAsync(Guid sessionId, GameType gameType)
        {
            Game? game = null;

            try
            {
                game = await _mainDbContext.Games.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.GameType == gameType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return game;
        }

        public async Task<bool> AddAsync(Game game)
        {
            bool success = true;

            try
            {
                await _mainDbContext.Games.AddAsync(game);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _mainDbContext.SaveChangesAsync();
            return success;
        }

        public async Task<bool> UpdateAsync(Game game)
        {
            bool success = true;

            try
            {
                _mainDbContext.Update(game);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _mainDbContext.SaveChangesAsync();
            return success;
        }

        public async Task AddOrUpdateAsync(Game game)
        {
            var existingGame = await GetGameAsync(game.SessionId, game.GameType);
            if (existingGame == null)
                await AddAsync(game);
            else
                await UpdateAsync(existingGame);
        }
        #endregion Methods..
    }
}
