using Microsoft.EntityFrameworkCore;

namespace VideoGameCritic.Data
{
    public abstract class DbContextBase : DbContext
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        public override int SaveChanges()
        {
            var modelEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is ModelBase && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modelEntries)
            {
                ((ModelBase)entry.Entity).ModifiedOn = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                    ((ModelBase)entry.Entity).CreatedOn = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }
        #endregion Methods..
    }
}
