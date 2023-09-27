namespace VideoGameGuy.Data
{
    public interface IRepositoryBase
    {
        #region Methods..
        Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
