namespace VideoGameGuy.Data
{
    public class IgdbPlatform : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatformId { get; set; }

        public long SourceId { get; set; }
        public Guid Checksum { get; set; }
        public List<IgdbGame>? Games { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }

        public long? PlatformFamilyId {  get; set; }
        public long? PlatformLogoId {  get; set; }

        public long IgdbPlatformFamilyId { get; set; }
        public IgdbPlatformFamily IgdbPlatformFamily { get; set; }

        public long IgdbPlatformLogoId { get; set; }
        public IgdbPlatformLogo IgdbPlatformLogo { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..
    }
}
