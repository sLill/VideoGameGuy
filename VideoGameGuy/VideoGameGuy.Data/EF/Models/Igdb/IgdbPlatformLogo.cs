namespace VideoGameGuy.Data
{
    public class IgdbPlatformLogo : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatformLogoId { get; set; }
        public long SourceId { get; set; }
        public List<IgdbPlatform>? Platforms { get; set; }

        public Guid Checksum { get; set; }
        public string? ImageId { get; set; }
        public string Url { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }
        #endregion Properties..
    }
}
