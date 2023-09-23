namespace VideoGameGuy.Data
{
    public class IgdbPlatform : ModelBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public Guid IgdbPlatformId { get; set; }

        public List<IgdbGame>? Games { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
