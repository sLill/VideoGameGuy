namespace VideoGameGuy.Data
{
    public class IgdbPlatform : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatformId { get; set; }

        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public List<IgdbGame>? Games { get; set; }

        public Guid IgdbPlatformFamilyId { get; set; }
        public IgdbPlatformFamily IgdbPlatformFamily { get; set; }

        public Guid IgdbPlatformLogoId { get; set; }
        public IgdbPlatformLogo IgdbPlatformLogo { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbPlatform(IgdbApiPlatform platform)
        {
            SourceId = platform.id;
            Checksum = platform.checksum;

            Name = platform.name;
            Category = platform.category.ToString();

            Source_CreatedOn_Unix = platform.created_at;
            Source_UpdatedOn_Unix = platform.updated_at;
        }
        #endregion Constructors..
    }
}
