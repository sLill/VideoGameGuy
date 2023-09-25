namespace VideoGameGuy.Data
{
    public class IgdbTheme : ModelBase
    {
        #region Properties..
        public long IgdbThemeId { get; set; }
        public Guid Checksum { get; set; }

        public string? Name { get; set; }
        public string Url { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbTheme(IgdbApiTheme theme)
        {
            Initialize(theme);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiTheme theme)
        {
            IgdbThemeId = theme.id;
            Checksum = theme.checksum;

            Name = theme.name;
            Url = theme.url;

            Source_CreatedOn_Unix = theme.created_at;
            Source_UpdatedOn_Unix = theme.updated_at;
        }
        #endregion Methods..
    }
}
