namespace VideoGameGuy.Data
{
    public interface IIgdbGameModesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGameMode> apiGameModes, bool suspendSaveChanges = false);

        Task<bool> SaveBulkChangesAsync();
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiGameMode> apiGameModes);
        #endregion Methods..
    }
}
