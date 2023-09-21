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
                 .HasKey(x => x.SystemStatusId);

            // ApplicationLogs
            modelBuilder.Entity<ApplicationLog>()
                .HasKey(x => x.LogId);

            modelBuilder.Entity<ApplicationLog>()
                .Property(x => x.Category)
                .HasColumnType("NVARCHAR(255)");

            modelBuilder.Entity<ApplicationLog>()
                .Property(x => x.Message)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();

            modelBuilder.Entity<ApplicationLog>()
                .Property(x => x.Exception)
                .HasColumnType("NVARCHAR(MAX)");
        }
        #endregion Methods..
    }
}
