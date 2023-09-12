using Microsoft.EntityFrameworkCore;

namespace VideoGameShowdown.Models
{
    public class ApplicationDbContext : DbContext
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        //public DbSet<RAWG_Game> Games { get; set; }
        //public DbSet<RAWG_Rating> Ratings { get; set; }
        //public DbSet<RAWG_EsrbRating> EsrbRatings { get; set; }
        //public DbSet<RAWG_ShortScreenshot> ShortScreenshots { get; set; }
        //public DbSet<RAWG_Platform> Platforms { get; set; }
        //public DbSet<RAWG_Genre> Genres { get; set; }
        //public DbSet<RAWG_Store> Stores { get; set; }

        //public DbSet<PlatformGame> PlatformGames { get; set; }
        //public DbSet<GenreGame> GenreGames { get; set; }
        //public DbSet<StoreGame> StoreGames { get; set; }
        //public DbSet<TagGame> TagGames { get; set; }
        #endregion Properties..

        #region Constructors..
        public ApplicationDbContext(DbContextOptions options) 
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        #region Event Handlers..
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RAWG_Game>()
            //    .HasKey(g => g.Id);

            //modelBuilder.Entity<RAWG_Game>()
            //    .HasOne(g => g.EsrbRating)
            //    .WithMany()
            //    .HasForeignKey(g => g.EsrbRatingId);

            //modelBuilder.Entity<RAWG_Rating>()
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
        }
        #endregion Event Handlers..
        #endregion Methods..
    }
}
