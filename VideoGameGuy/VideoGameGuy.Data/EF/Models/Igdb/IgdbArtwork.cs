namespace VideoGameGuy.Data
{
    public class IgdbArtwork : ModelBase
    {
        #region Properties..
        public Guid IgdbArtworkId { get; set; }

        public long SourceId { get; set; }
        public Guid Checksum { get; set; }
        
        public Guid GameId { get; set; }
        public IgdbGame Game { get; set; }

        public int? ImageId { get; set; }
        public string Url { get; set; }
        
        public int? Height { get; set; }
        public int? Width { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbArtwork(IgdbApiArtwork artwork)
        {
            SourceId = artwork.id;
            Checksum = artwork.checksum;

            ImageId = artwork.image_id;
            Url = artwork.url;
            
            Height = artwork.height;
            Width = artwork.width;
        }
        #endregion Constructors..
    }
}
