namespace VideoGameShowdown.Configuration
{
    public class RawgApiSettings
    {
        #region Properties..
        public string ApiKey { get; set; } 
        public string ApiUrl { get; set; }
        public int RequestTimeout { get; set; }
        public int PollingInterval_Minutes { get; set; }
        public string LocalCache_RelativePath { get; set; }
        public Dictionary<string, string> Endpoints { get; set; }
        #endregion Properties..
    }
}
