namespace VideoGameGuy.Data
{
    public interface IIgdbThemesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiTheme> apiThemes);
        #endregion Methods..
    }
}
