namespace VideoGameCritic.Configuration
{
    public class IgdbApiSettings
    {
        #region Properties..
        public string AuthUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int RequestTimeout { get; set; }
        public int RetryLimit { get; set; }
        public int LocalCache_UpdateInterval_Days { get; set; }
        public string LocalCache_RelativePath { get; set; }
        public Dictionary<string, string> Endpoints { get; set; }
        #endregion Properties..
    }
}
