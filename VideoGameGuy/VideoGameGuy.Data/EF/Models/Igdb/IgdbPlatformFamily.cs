namespace VideoGameGuy.Data
{
    public class IgdbPlatformFamily : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatformFamilyId { get; set; }
        public long SourceId { get; set; }
        public List<IgdbPlatform>? Platforms { get; set; }

        public Guid Checksum { get; set; }
        public string? Name { get; set; }
        #endregion Properties..
    }
}
