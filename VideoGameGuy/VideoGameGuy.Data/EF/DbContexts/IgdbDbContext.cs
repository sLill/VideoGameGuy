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
        public DbSet<IgdbPlatformFamily> PlatformFamilies { get; set; }
        public DbSet<IgdbPlatformLogo> PlatformLogos { get; set; }
        public DbSet<IgdbGameMode> GameModes { get; set; }
        public DbSet<IgdbMultiplayerMode> MultiplayerModes { get; set; }
        public DbSet<IgdbArtwork> Artworks { get; set; }
        public DbSet<IgdbScreenshot> Screenshots { get; set; }
        public DbSet<IgdbTheme> Themes { get; set; }

        // Join tables
        public DbSet<IgdbGames_GameModes> Games_GameModes { get; set; }
        public DbSet<IgdbGames_MultiplayerModes> Games_MultiplayerModes { get; set; }
        public DbSet<IgdbGames_Platforms> Games_Platforms { get; set; }
        public DbSet<IgdbGames_Themes> Games_Themes { get; set; }
        public DbSet<IgdbGames_Artworks> Games_Artworks { get; set; }
        public DbSet<IgdbGames_Screenshots> Games_Screenshots { get; set; }
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
            DefinePlatformFamilySchema(modelBuilder);
            DefinePlatformLogoSchema(modelBuilder);
            DefineGameModeSchema(modelBuilder);
            DefineMultiplayerModeSchema(modelBuilder);
            DefineArtworkSchema(modelBuilder);
            DefineScreenshotSchema(modelBuilder);
            DefineThemeSchema(modelBuilder);

            // Join tables
            DefineGames_GameModesSchema(modelBuilder);
            DefineGames_MultiplayerModesSchema(modelBuilder);
            DefineGames_PlatformsSchema(modelBuilder);
            DefineGames_ThemesSchema(modelBuilder);
            DefineGames_ArtworksSchema(modelBuilder);
            DefineGames_ScreenshotsSchema(modelBuilder);
        }

        private void DefineGameSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGame>()
                .HasKey(g => g.IgdbGameId);

            modelBuilder.Entity<IgdbGame>()
                .Property(g => g.Name)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbGame>()
                .Property(g => g.Category)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbGame>()
                .Property(g => g.Status)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbGame>()
                .Property(g => g.Storyline)
                .HasColumnType("NVARCHAR(MAX)");

            modelBuilder.Entity<IgdbGame>()
                .Property(g => g.Summary)
                .HasColumnType("NVARCHAR(MAX)");
        }

        private void DefinePlatformSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbPlatform>()
                .HasKey(p => p.IgdbPlatformId);

            modelBuilder.Entity<IgdbPlatform>()
                .Property(p => p.Name)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbPlatform>()
                .Property(p => p.Category)
                .HasColumnType("NVARCHAR(255)");
        }

        private void DefinePlatformFamilySchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbPlatformFamily>()
                .HasKey(pf => pf.IgdbPlatformFamilyId);

            modelBuilder.Entity<IgdbPlatformFamily>()
                .Property(pf => pf.Name)
                .HasColumnType("NVARCHAR(255)");
        }

        private void DefinePlatformLogoSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbPlatformLogo>()
                .HasKey(pl => pl.IgdbPlatformLogoId);

            modelBuilder.Entity<IgdbPlatformLogo>()
                .Property(pf => pf.ImageId)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbPlatformLogo>()
                .Property(pf => pf.Url)
                .HasColumnType("NVARCHAR(2048)");
        }

        private void DefineGameModeSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGameMode>()
                .HasKey(gm => gm.IgdbGameModeId);

            modelBuilder.Entity<IgdbGameMode>()
                .Property(gm => gm.Name)
                .HasColumnType("NVARCHAR(255)");
        }

        private void DefineMultiplayerModeSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbMultiplayerMode>()
                .HasKey(mm => mm.IgdbMultiplayerModeId);
        }

        private void DefineArtworkSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbArtwork>()
                .HasKey(a => a.IgdbArtworkId);

            modelBuilder.Entity<IgdbArtwork>()
                .Property(a => a.ImageId)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbArtwork>()
                .Property(a => a.Url)
                .HasColumnType("NVARCHAR(2048)");
        }

        private void DefineScreenshotSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbScreenshot>()
                .HasKey(s => s.IgdbScreenshotId);

            modelBuilder.Entity<IgdbScreenshot>()
                .Property(a => a.ImageId)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbScreenshot>()
                .Property(a => a.Url)
                .HasColumnType("NVARCHAR(2048)");
        }

        private void DefineThemeSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbTheme>()
                .HasKey(t => t.IgdbThemeId);

            modelBuilder.Entity<IgdbTheme>()
                .Property(a => a.Name)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<IgdbTheme>()
                .Property(a => a.Url)
                .HasColumnType("NVARCHAR(2048)");
        }

        private void DefineGames_GameModesSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGames_GameModes>()
                .HasKey(t => t.IgdbGames_GameModesId);
        }

        private void DefineGames_MultiplayerModesSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGames_MultiplayerModes>()
                .HasKey(t => t.IgdbGames_MultiplayerModesId);
        }

        private void DefineGames_PlatformsSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGames_Platforms>()
                .HasKey(t => t.IgdbGames_PlatformsId);
        }

        private void DefineGames_ThemesSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGames_Themes>()
                .HasKey(t => t.IgdbGames_ThemesId);
        }

        private void DefineGames_ArtworksSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGames_Artworks>()
                .HasKey(t => t.IgdbGames_ArtworksId);
        }

        private void DefineGames_ScreenshotsSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IgdbGames_Screenshots>()
                .HasKey(t => t.IgdbGames_ScreenshotsId);
        }
        #endregion Methods..
    }
}
