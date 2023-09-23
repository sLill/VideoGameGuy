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
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
