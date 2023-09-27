namespace VideoGameGuy.Data
{
    public interface IIgdbGameModesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGameMode> apiGameModes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
