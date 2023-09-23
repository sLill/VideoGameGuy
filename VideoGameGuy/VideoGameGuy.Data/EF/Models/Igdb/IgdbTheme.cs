namespace VideoGameGuy.Data
{
    public class IgdbTheme : ModelBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public Guid IgdbThemeId { get; set; }

        public Guid GameId { get; set; }
        public IgdbGame Game { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
