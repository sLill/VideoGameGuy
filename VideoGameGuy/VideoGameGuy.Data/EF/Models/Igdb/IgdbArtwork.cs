namespace VideoGameGuy.Data
{
    public class IgdbArtwork : ModelBase
    {
        #region Properties..
        public long IgdbArtworkId { get; set; }
        public Guid Checksum { get; set; }
        public long IgdbGameId { get; set; }

        public int? ImageId { get; set; }
        public string Url { get; set; }
        
        public int? Height { get; set; }
        public int? Width { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbArtwork(IgdbApiArtwork artwork)
        {
            Initialize(artwork);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiArtwork artwork)
        {
            IgdbArtworkId = artwork.id;
            Checksum = artwork.checksum;

            IgdbGameId = artwork.game;

            ImageId = artwork.image_id;
            Url = artwork.url;

            Height = artwork.height;
            Width = artwork.width;
        }
        #endregion Methods..
    }
}
