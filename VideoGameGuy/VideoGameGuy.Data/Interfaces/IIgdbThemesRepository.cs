namespace VideoGameGuy.Data
{
    public interface IIgdbThemesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiTheme> apiThemes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
