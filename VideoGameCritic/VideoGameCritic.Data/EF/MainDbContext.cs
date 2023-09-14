using Microsoft.EntityFrameworkCore;

namespace VideoGameCritic.Data
{
    public class MainDbContext : DbContext
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public DbSet<SystemStatus> SystemStatus { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
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

            // ErrorLog
            modelBuilder.Entity<ErrorLog>()
                .HasKey(x => x.ErrorId);

            modelBuilder.Entity<ErrorLog>()
                .Property(x => x.Message)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();

            modelBuilder.Entity<ErrorLog>()
                .Property(x => x.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        }
        #endregion Methods..
    }
}
