namespace VideoGameGuy.Data
{
    public interface IIgdbScreenshotsRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiScreenshot> apiScreenshots, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiScreenshot> apiScreenshots);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
