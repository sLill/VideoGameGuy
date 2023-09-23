namespace VideoGameGuy.Data
{
    public class RawgScreenshot : ModelBase
    {
        #region Properties..
        public Guid RawgScreenshotId { get; set; }

        public Guid GameId { get; set; }
        public RawgGame Game { get; set; }

        public string Source {  get; set; }

        public int? SourceId {  get; set; }

        public string Uri { get; set; }
        #endregion Properties..
    }
}
