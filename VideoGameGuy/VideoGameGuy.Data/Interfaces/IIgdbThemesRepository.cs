namespace VideoGameGuy.Data
{
    public interface IIgdbThemesRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiTheme> apiThemes, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
