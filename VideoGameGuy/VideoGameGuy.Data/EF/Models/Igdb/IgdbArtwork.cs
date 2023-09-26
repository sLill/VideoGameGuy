namespace VideoGameGuy.Data
{
    public class IgdbArtwork : ModelBase
    {
        #region Properties..
        public Guid IgdbArtworkId { get; set; }
        public long SourceId { get; set; }
        public long Games_SourceId { get; set; }
        public Guid Checksum { get; set; }

        public string? ImageId { get; set; }
        public string Url { get; set; }
        
        public int? Height { get; set; }
        public int? Width { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(IgdbApiArtwork artwork)
        {
            SourceId = artwork.id;
            Games_SourceId = artwork.game;
            Checksum = artwork.checksum;

            ImageId = artwork.image_id;
            Url = artwork.url;

            Height = artwork.height;
            Width = artwork.width;
        }
        #endregion Methods..
    }
}
