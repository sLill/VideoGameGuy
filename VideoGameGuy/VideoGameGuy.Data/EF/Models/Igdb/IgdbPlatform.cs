namespace VideoGameGuy.Data
{
    public class IgdbPlatform : ModelBase
    {
        #region Properties..
        public long IgdbPlatformId { get; set; }
        public Guid Checksum { get; set; }

        public long IgdbPlatformFamilyId { get; set; }
        public long IgdbPlatformLogoId { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbPlatform(IgdbApiPlatform platform)
        {
            Initialize(platform);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiPlatform platform)
        {
            IgdbPlatformId = platform.id;
            Checksum = platform.checksum;

            Name = platform.name;
            Category = platform.category.ToString();

            Source_CreatedOn_Unix = platform.created_at;
            Source_UpdatedOn_Unix = platform.updated_at;
        }
        #endregion Methods..
    }
}
