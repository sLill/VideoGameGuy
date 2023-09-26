namespace VideoGameGuy.Data
{
    public class IgdbPlatforms_PlatformFamilies : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatforms_PlatformFamiliesId { get; set; }
        public long Platforms_SourceId { get; set; }
        public long PlatformFamilies_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long platforms_SourceId, long platformFamilies_SourceId)
        {
            Platforms_SourceId = platforms_SourceId;
            PlatformFamilies_SourceId = platformFamilies_SourceId;
        }
        #endregion Methods..
    }
}
