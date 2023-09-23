using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbDbContext : DbContextBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<IgdbGame> Games { get; set; }
        public DbSet<IgdbPlatform> Platforms { get; set; }
        public DbSet<IgdbGameMode> GameModes { get; set; }
        public DbSet<IgdbMultiplayerMode> MultiplayerModes { get; set; }
        public DbSet<IgdbArtwork> Artworks { get; set; }
        public DbSet<IgdbScreenshot> Screenshots { get; set; }
        public DbSet<IgdbTheme> Themes { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbDbContext(DbContextOptions<IgdbDbContext> options)
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DefineGameSchema(modelBuilder);
            DefinePlatformSchema(modelBuilder);
            DefineGameModeSchema(modelBuilder);
            DefineMultiplayerModeSchema(modelBuilder);
            DefineArtworkSchema(modelBuilder);
            DefineScreenshotSchema(modelBuilder);
            DefineThemeSchema(modelBuilder);
        }

        private void DefineGameSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGame>()
              .HasKey(x => x.IgdbGameId);

            modelBuilder.Entity<IgdbGame>()
                .HasKey(x => x.SourceId);

            modelBuilder.Entity<IgdbGame>()
                .Property(x => x.Name)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbGame>()
                .Property(x => x.Category)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbGame>()
                .Property(x => x.Status)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbGame>()
                .Property(x => x.Storyline)
                .HasColumnType("NVARCHAR(MAX)");

            modelBuilder.Entity<IgdbGame>()
                .Property(x => x.Summary)
                .HasColumnType("NVARCHAR(MAX)");

            modelBuilder.Entity<IgdbGame>()
                .HasMany(x => x.Platforms)
                .WithMany(x => x.Games)
                .UsingEntity(x => x.ToTable("Games_Platforms"));

            modelBuilder.Entity<IgdbGame>()
                .HasMany(x => x.GameModes)
                .WithMany(x => x.Games)
                .UsingEntity(x => x.ToTable("Games_GameModes"));

            modelBuilder.Entity<IgdbGame>()
                .HasMany(x => x.MultiplayerModes)
                .WithMany(x => x.Games)
                .UsingEntity(x => x.ToTable("Games_MultiplayerModes"));

            modelBuilder.Entity<IgdbGame>()
                .HasMany(x => x.Artworks)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);

            modelBuilder.Entity<IgdbGame>()
                .HasMany(x => x.Screenshots)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);

            modelBuilder.Entity<IgdbGame>()
                .HasMany(x => x.Themes)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);
        }

        private void DefinePlatformSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbPlatform>()
                .HasKey(x => x.IgdbPlatformId);
        }

        private void DefineGameModeSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGameMode>()
                .HasKey(x => x.IgdbGameModeId);
        }

        private void DefineMultiplayerModeSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbMultiplayerMode>()
                .HasKey(x => x.IgdbMultiplayerModeId);
        }

        private void DefineArtworkSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbArtwork>()
                .HasKey(x => x.IgdbArtworkId);
        }

        private void DefineScreenshotSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbScreenshot>()
                .HasKey(x => x.IgdbScreenshotId);
        }

        private void DefineThemeSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbTheme>()
                .HasKey(x => x.IgdbThemeId);
        }
        #endregion Methods..
    }
}
