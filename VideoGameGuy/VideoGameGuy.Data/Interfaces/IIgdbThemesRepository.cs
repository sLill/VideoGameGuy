namespace VideoGameGuy.Data
{
    public interface IIgdbThemesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiTheme> apiThemes, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiTheme> apiThemes);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
