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
        public DbSet<IgdbPlatformFamily> IgdbPlatformFamilies { get; set; }
        public DbSet<IgdbPlatformLogo> IgdbPlatformLogos { get; set; }
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
            DefinePlatformFamilySchema(modelBuilder);
            DefinePlatformLogoSchema(modelBuilder);
            DefineGameModeSchema(modelBuilder);
            DefineMultiplayerModeSchema(modelBuilder);
            DefineArtworkSchema(modelBuilder);
            DefineScreenshotSchema(modelBuilder);
            DefineThemeSchema(modelBuilder);
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

            modelBuilder.Entity<IgdbGame>()
                .HasMany(g => g.Platforms)
                .WithMany(p => p.Games)
                .UsingEntity(j => j.ToTable("Games_Platforms"));

            modelBuilder.Entity<IgdbGame>()
                .HasMany(g => g.GameModes)
                .WithMany(gm => gm.Games)
                .UsingEntity(j => j.ToTable("Games_GameModes"));

            modelBuilder.Entity<IgdbGame>()
                .HasMany(g => g.MultiplayerModes)
                .WithMany(mm => mm.Games)
                .UsingEntity(j => j.ToTable("Games_MultiplayerModes"));

            modelBuilder.Entity<IgdbGame>()
                .HasMany(g => g.Artworks)
                .WithOne(a => a.Game)
                .HasForeignKey(a => a.GameId);

            modelBuilder.Entity<IgdbGame>()
                .HasMany(g => g.Screenshots)
                .WithOne(s => s.Game)
                .HasForeignKey(s => s.GameId);

            modelBuilder.Entity<IgdbGame>()
                .HasMany(g => g.Themes)
                .WithOne(t => t.Game)
                .HasForeignKey(t => t.GameId);
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

            modelBuilder.Entity<IgdbPlatform>()
                .HasOne(p => p.IgdbPlatformFamily)
                .WithMany(pf => pf.Platforms)
                .HasForeignKey(p => p.IgdbPlatformFamilyId);

            modelBuilder.Entity<IgdbPlatform>()
                .HasOne(p => p.IgdbPlatformLogo)
                .WithMany(pl => pl.Platforms)
                .HasForeignKey(p => p.IgdbPlatformLogoId);
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
        #endregion Methods..
    }
}
