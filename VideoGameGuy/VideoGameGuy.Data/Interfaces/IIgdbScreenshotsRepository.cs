namespace VideoGameGuy.Data
{
    public interface IIgdbScreenshotsRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiScreenshot> apiScreenshots, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
