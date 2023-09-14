namespace VideoGameCritic.Configuration
{
    public class RawgApiSettings
    {
        #region Properties..
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public int RequestTimeout { get; set; }
        public int RetryLimit { get; set; }
        public int Response_PageSize { get; set; }
        public int LocalCache_UpdateInterval_Days { get; set; }
        public string LocalCache_RelativePath { get; set; }
        public Dictionary<string, string> Endpoints { get; set; }
        #endregion Properties..
    }
}
