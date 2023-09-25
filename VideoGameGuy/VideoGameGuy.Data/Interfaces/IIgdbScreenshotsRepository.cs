namespace VideoGameGuy.Data
{
    public interface IIgdbScreenshotsRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiScreenshot> apiScreenshots);
        #endregion Methods..
    }
}
