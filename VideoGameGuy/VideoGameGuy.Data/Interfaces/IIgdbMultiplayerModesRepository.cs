namespace VideoGameGuy.Data
{
    public interface IIgdbMultiplayerModesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiMultiplayerMode> apiMultiplayerModes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
