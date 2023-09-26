namespace VideoGameGuy.Data
{
    public interface IIgdbGameModesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGameMode> apiGameModes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
