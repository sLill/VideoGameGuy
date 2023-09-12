using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Inputs;
using Syncfusion.Licensing;

namespace VideoGameShowdown.Models
{
    public class ApplicationDbContext : DbContext
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<RAWG_Game> RAWG_Games { get; set; }
        public DbSet<RAWG_Rating> RAWG_Ratings { get; set; }
        public DbSet<RAWG_EsrbRating> RAWG_EsrbRatings { get; set; }
        public DbSet<RAWG_ShortScreenshot> RAWG_ShortScreenshots { get; set; }
        public DbSet<RAWG_Platform> RAWG_Platforms { get; set; }
        public DbSet<RAWG_Genre> RAWG_Genres { get; set; }
        public DbSet<RAWG_Store> RAWG_Stores { get; set; }

        public DbSet<PlatformGame> RAWG_PlatformGames { get; set; }
        public DbSet<GenreGame> RAWG_GenreGames { get; set; }
        public DbSet<StoreGame> RAWG_StoreGames { get; set; }
        public DbSet<TagGame> RAWG_TagGames { get; set; }
        #endregion Properties..

        #region Constructors..
        public ApplicationDbContext(DbContextOptions options) 
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RAWG_Game>()
            //    .HasKey(g => g.Id);

            //modelBuilder.Entity<RAWG_Game>()
            //    .HasOne(g => g.EsrbRating)
            //    .WithMany()
            //    .HasForeignKey(g => g.EsrbRatingId);

            //modelBuilder.Entity<Rating>()
            //    .HasKey(r => r.Id);

            //modelBuilder.Entity<PlatformGame>()
            //    .HasKey(pg => pg.Id);

            //modelBuilder.Entity<GenreGame>()
            //    .HasKey(gg => gg.Id);

            //modelBuilder.Entity<StoreGame>()
            //    .HasKey(sg => sg.Id);

            //modelBuilder.Entity<TagGame>()
            //    .HasKey(tg => tg.Id);

            //modelBuilder.Entity<RAWG_EsrbRating>()
            //    .HasKey(e => e.Id);

            //modelBuilder.Entity<RAWG_ShortScreenshot>()
            //    .HasKey(ss => ss.Id);

            //modelBuilder.Entity<RAWG_Platform>()
            //    .HasKey(p => p.Id);

            //modelBuilder.Entity<RAWG_Genre>()
            //    .HasKey(g => g.Id);

            //modelBuilder.Entity<RAWG_Store>()
            //    .HasKey(s => s.Id);

            // Define relationships here if necessary, such as HasMany, HasOne, etc.
        }
        #endregion Methods..
    }
}
