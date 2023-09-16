namespace VideoGameCritic.Data
{
    public class RawgGame
    {
        #region Properties..
        public int id { get; set; }
        public string name { get; set; }
        public DateTime? released { get; set; }
        public string background_image { get; set; }
        public double? rating { get; set; }
        public double? rating_top { get; set; }
        public List<rating>? ratings { get; set; }
        public int? ratings_count { get; set; }
        public added_by_status? added_by_status { get; set; }
        public int? metacritic { get; set; }
        public int? playtime { get; set; }
        public DateTime? updated { get; set; }
        public List<screenshot>? short_screenshots { get; set; } 
        #endregion Properties..
    }

    public class rating
    {
        #region Properties..
        public int id { get; set; }
        public string title { get; set; }
        public int count { get; set; }
        public double percent { get; set; }
        #endregion Properties..
    }

    public class added_by_status
    {
        #region Properties..
        public int yet { get; set; }
        public int owned { get; set; }
        public int beaten { get; set; }
        public int toplay { get; set; }
        public int dropped { get; set; }
        public int playing { get; set; }
        #endregion Properties..
    }

    public class screenshot
    {
        #region Properties..
        public int id { get; set; }
        public string image { get; set; }
        #endregion Properties..
    }
}
