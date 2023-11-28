using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class MainDbContext : DbContextBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<SystemStatus> SystemStatus { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
        public DbSet<TrafficLog> TrafficLogs { get; set; }
        public DbSet<Game> Games { get; set; }
        #endregion Properties..

        #region Constructors..
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options) { }
        #endregion Constructors..

        #region Methods..
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SystemStatus
            modelBuilder.Entity<SystemStatus>()
                 .HasKey(ss => ss.SystemStatusId);

            // ApplicationLogs
            modelBuilder.Entity<ApplicationLog>()
                .HasKey(al => al.LogId);

            modelBuilder.Entity<ApplicationLog>()
                .Property(al => al.Category)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<ApplicationLog>()
                .Property(al => al.Message)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();

            modelBuilder.Entity<ApplicationLog>()
                .Property(al => al.Exception)
                .HasColumnType("NVARCHAR(MAX)");

            // TrafficLogs
            modelBuilder.Entity<TrafficLog>()
             .HasKey(tl => tl.TrafficLogId);

            modelBuilder.Entity<TrafficLog>()
                .Property(tl => tl.Ip)
                .HasColumnType("NVARCHAR(64)");

            modelBuilder.Entity<TrafficLog>()
                .Property(tl => tl.Referer)
                .HasColumnType("NVARCHAR(512)");

            modelBuilder.Entity<TrafficLog>()
               .Property(tl => tl.UserAgent)
               .HasColumnType("NVARCHAR(255)");

            // Game
            modelBuilder.Entity<Game>()
                .HasKey(g => g.GameId);

            modelBuilder.Entity<Game>()
              .Property(g => g.ClientIp)
              .HasColumnType("NVARCHAR(64)");

            modelBuilder.Entity<Game>()
              .Property(g => g.GameScore)
              .HasColumnType("NVARCHAR(64)");
        }
        #endregion Methods..
    }
}
