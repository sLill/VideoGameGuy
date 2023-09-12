using Microsoft.EntityFrameworkCore;

namespace VideoGameShowdown.Models
{
    public class ApplicationDbContext : DbContext
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<RAWG_Game> Games { get; set; }
        #endregion Properties..

        #region Constructors..
        public ApplicationDbContext(DbContextOptions options) 
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
