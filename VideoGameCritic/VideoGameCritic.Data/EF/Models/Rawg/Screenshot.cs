namespace VideoGameCritic.Data
{
    public class Screenshot
    {
        #region Properties..
        public Guid ScreenshotId { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }

        public string Source {  get; set; }

        public int? SourceId {  get; set; }

        public string Uri { get; set; }
        #endregion Properties..
    }
}
