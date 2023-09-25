namespace VideoGameGuy.Data
{
    public interface IIgdbPlatformLogosRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos);
        #endregion Methods..
    }
}
