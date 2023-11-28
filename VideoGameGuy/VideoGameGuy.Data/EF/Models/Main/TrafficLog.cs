namespace VideoGameGuy.Data
{
    public class TrafficLog : ModelBase
    {
        #region Properties..
        public Guid TrafficLogId { get; set; }

        public string Ip { get; set; }
        public string? Referer { get; set; }
        public string? UserAgent { get; set; }
        #endregion Properties..
    }
}
