namespace VideoGameGuy.Data
{
    public class IgdbScreenshot : ModelBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public Guid IgdbScreenshotId { get; set; }

        public Guid GameId { get; set; }
        public IgdbGame Game { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
