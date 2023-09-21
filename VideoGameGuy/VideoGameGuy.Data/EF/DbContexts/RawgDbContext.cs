using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class RawgDbContext : DbContextBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerbaseProgress> PlayerbaseProgress { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Screenshot> Screenshots { get; set; }
        #endregion Properties..

        #region Constructors..
        public RawgDbContext(DbContextOptions<RawgDbContext> options)
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Games
            modelBuilder.Entity<Game>()
                .HasKey(x => x.GameId);

            modelBuilder.Entity<Game>()
                .Property(x => x.Name)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

            modelBuilder.Entity<Game>()
                .Property(x => x.ImageUri)
                .HasColumnType("NVARCHAR(2048)");

            // PlayerbaseProgress
            modelBuilder.Entity<PlayerbaseProgress>()
                .HasKey(x => x.PlayerbaseProgressId);

            modelBuilder.Entity<PlayerbaseProgress>()
                .Property(x => x.BeatTheGame_Percent)
                .HasColumnType("DECIMAL(9,2)")
                .HasComputedColumnSql("CASE " +
                                      "     WHEN [OwnTheGame] > 0 THEN CAST((([BeatTheGame] / [OwnTheGame]) * 100.0) AS DECIMAL(9,2)) " +
                                      "     ELSE 0 " +
                                      "END", true);

            // Ratings
            modelBuilder.Entity<Rating>()
                       .HasKey(x => x.RatingId);


            modelBuilder.Entity<Rating>()
                .Property(x => x.Description)
                .HasColumnType("NVARCHAR(32)");

            // Screenshots
            modelBuilder.Entity<Screenshot>()
                .HasKey(x => x.ScreenshotId);

            modelBuilder.Entity<Screenshot>()
                .Property(x => x.Source)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

            modelBuilder.Entity<Screenshot>()
                .Property(x => x.Uri)
                .HasColumnType("NVARCHAR(2048)")
                .IsRequired();
        }
        #endregion Methods..
    }
}
