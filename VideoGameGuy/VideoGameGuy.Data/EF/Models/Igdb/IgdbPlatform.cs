namespace VideoGameGuy.Data
{
    public class IgdbPlatform : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatformId { get; set; }
        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public long Source_PlatformFamilyId { get; set; }
        public long Source_PlatformLogoId { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(IgdbApiPlatform platform)
        {
            SourceId = platform.id;
            Checksum = platform.checksum;

            Source_PlatformFamilyId = platform.platform_family;
            Source_PlatformLogoId = platform.platform_logo;

            Name = platform.name;
            Category = platform.category.ToString();

            Source_CreatedOn_Unix = platform.created_at;
            Source_UpdatedOn_Unix = platform.updated_at;
        }
        #endregion Methods..
    }
}
