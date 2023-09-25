namespace VideoGameGuy.Data
{
    public class IgdbScreenshot : ModelBase
    {
        #region Properties..
        public long IgdbScreenshotId { get; set; }
        public Guid Checksum { get; set; }
        public long IgdbGameId { get; set; }

        public string? ImageId { get; set; }
        public string? Url { get; set; }

        public int? Height { get; set; }
        public int? Width { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbScreenshot(IgdbApiScreenshot screenshot)
        {
            Initialize(screenshot);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiScreenshot screenshot)
        {
            IgdbScreenshotId = screenshot.id;
            Checksum = screenshot.checksum;
            IgdbGameId = screenshot.game;

            ImageId = screenshot.image_id;
            Url = screenshot.url;

            Height = screenshot.height;
            Width = screenshot.width;
        }
        #endregion Methods..
    }
}
