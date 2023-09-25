namespace VideoGameGuy.Data
{
    public class IgdbScreenshot : ModelBase
    {
        #region Properties..
        public Guid IgdbScreenshotId { get; set; }
        public long SourceId { get; set; }
        public long Game_SourceId { get; set; }
        public Guid Checksum { get; set; }

        public string? ImageId { get; set; }
        public string? Url { get; set; }

        public int? Height { get; set; }
        public int? Width { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(IgdbApiScreenshot screenshot)
        {
            SourceId = screenshot.id;
            Game_SourceId = screenshot.game;
            Checksum = screenshot.checksum;

            ImageId = screenshot.image_id;
            Url = screenshot.url;

            Height = screenshot.height;
            Width = screenshot.width;
        }
        #endregion Methods..
    }
}
