namespace VideoGameGuy.Data
{
    public interface IIgdbMultiplayerModesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiMultiplayerMode> apiMultiplayerModes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
