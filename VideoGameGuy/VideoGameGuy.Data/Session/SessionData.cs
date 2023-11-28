namespace VideoGameGuy.Data
{ 
    public class SessionData
    {
        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public ReviewScoresSessionItem ReviewScoresSessionItem { get; set; } = new ReviewScoresSessionItem();
        public DescriptionsSessionItem DescriptionsSessionItem { get; set; } = new DescriptionsSessionItem();
        public ScreenshotsSessionItem ScreenshotsSessionItem { get; set; } = new ScreenshotsSessionItem();

        public CountdownSessionItem DescriptionCountdownSessionItem { get; set; } = new CountdownSessionItem();
        public CountdownSessionItem ScreenshotCountdownSessionItem { get; set; } = new CountdownSessionItem();
        #endregion Properties..
    }
}
