using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VideoGameShowdown.Common
{
    public static class DbContextExtensions
    {
        #region Methods..
        public static IEnumerable<EntityEntry> GetPendingChanges(this DbContext value)
        {
            return value.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged);
        }
        #endregion Methods..
    }
}
