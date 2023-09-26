namespace VideoGameGuy.Data
{
    public class IgdbPlatforms_PlatformLogos : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatforms_PlatformLogosId { get; set; }
        public long Platforms_SourceId { get; set; }
        public long PlatformLogos_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long platforms_SourceId, long platformLogos_SourceId)
        {
            Platforms_SourceId = platforms_SourceId;
            PlatformLogos_SourceId = platformLogos_SourceId;
        }
        #endregion Methods..
    }
}
