namespace VideoGameGuy.Data
{ 
    public class SessionData
    {
        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public DescriptionsSessionItem DescriptionsSessionItem { get; set; } = new DescriptionsSessionItem();
        public ReviewScoresSessionItem ReviewScoresSessionItem { get; set; } = new ReviewScoresSessionItem();
        public CountdownSessionItem CountdownSessionItem { get; set; } = new CountdownSessionItem();
        #endregion Properties..
    }
}
