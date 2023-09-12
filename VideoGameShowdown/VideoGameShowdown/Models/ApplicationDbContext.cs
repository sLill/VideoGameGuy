using Microsoft.EntityFrameworkCore;

namespace VideoGameShowdown.Models
{
    public class ApplicationDbContext : DbContext
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<Game> Games { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<EsrbRating> EsrbRatings { get; set; }
        public DbSet<ShortScreenshot> ShortScreenshots { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Store> Stores { get; set; }

        // Relationship tables
        public DbSet<Platform_Game> Platforms_Games { get; set; }
        public DbSet<Genre_Game> Genres_Games { get; set; }
        public DbSet<Store_Game> Stores_Games { get; set; }
        public DbSet<Tag_Game> Tags_Games { get; set; }
        #endregion Properties..

        #region Constructors..
        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
