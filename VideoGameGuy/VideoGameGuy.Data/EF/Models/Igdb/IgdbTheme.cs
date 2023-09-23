namespace VideoGameGuy.Data
{
    public class IgdbTheme : ModelBase
    {
        #region Properties..
        public Guid IgdbThemeId { get; set; }

        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public Guid GameId { get; set; }
        public IgdbGame Game { get; set; }

        public string? Name { get; set; }
        public string Url { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
