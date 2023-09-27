namespace VideoGameGuy.Data
{
    public interface IIgdbPlatformLogosRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos);
        #endregion Methods..
    }
}
