using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class RawgDbContext : DbContextBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<RawgGame> Games { get; set; }
        public DbSet<RawgPlayerbaseProgress> PlayerbaseProgress { get; set; }
        public DbSet<RawgRating> Ratings { get; set; }
        public DbSet<RawgScreenshot> Screenshots { get; set; }
        #endregion Properties..

        #region Constructors..
        public RawgDbContext(DbContextOptions<RawgDbContext> options)
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DefineGameSchema(modelBuilder);
            DefinePlayerProgressSchema(modelBuilder);
            DefineRatingSchema(modelBuilder);
            DefineScreenshotSchema(modelBuilder);
        }

        private void DefineGameSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawgGame>()
                .HasKey(g => g.RawgGameId);

            modelBuilder.Entity<RawgGame>()
                .Property(g => g.Name)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

            modelBuilder.Entity<RawgGame>()
                .Property(g => g.ImageUri)
                .HasColumnType("NVARCHAR(2048)");
        }

        private void DefinePlayerProgressSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawgPlayerbaseProgress>()
               .HasKey(pp => pp.RawgPlayerbaseProgressId);

            modelBuilder.Entity<RawgPlayerbaseProgress>()
                .Property(pp => pp.BeatTheGame_Percent)
                .HasColumnType("DECIMAL(9,2)")
                .HasComputedColumnSql("CASE " +
                                      "     WHEN [OwnTheGame] > 0 THEN CAST((([BeatTheGame] / [OwnTheGame]) * 100.0) AS DECIMAL(9,2)) " +
                                      "     ELSE 0 " +
                                      "END", true);
        }

        private void DefineRatingSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawgRating>()
                .HasKey(r => r.RawgRatingId);

            modelBuilder.Entity<RawgRating>()
                .Property(r => r.Description)
                .HasColumnType("NVARCHAR(32)");
        }

        private void DefineScreenshotSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawgScreenshot>()
                .HasKey(s => s.RawgScreenshotId);

            modelBuilder.Entity<RawgScreenshot>()
                .Property(s => s.Source)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

            modelBuilder.Entity<RawgScreenshot>()
                .Property(s => s.Uri)
                .HasColumnType("NVARCHAR(2048)")
                .IsRequired();
        }
        #endregion Methods..
    }
}
