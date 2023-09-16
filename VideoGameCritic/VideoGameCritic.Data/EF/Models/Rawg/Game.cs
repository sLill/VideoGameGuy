namespace VideoGameCritic.Data
{
    public class Game : ModelBase
    {
        #region Properties..
        public Guid GameId { get; set; }

        public int? RawgId { get; set; }

        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? ImageUri { get; set; }

        public int? MetacriticScore { get; set; }

        public int? AverageUserScore { get; set; }

        public int? AveragePlaytime_Hours { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? RatingsCount { get; set; }

        // Foreign Key Relationships
        public PlayerbaseProgress? PlayerbaseProgress { get; set; }

        public List<Rating> Ratings { get; set; }

        public List<Screenshot>? Screenshots { get; set; }
        #endregion Properties..

        #region Constructors..
        public Game() { }

        public Game(RawgGame rawgGame)
        {
            RawgId = rawgGame.id;
            Name = rawgGame.name;
            ReleaseDate = rawgGame.released;
            ImageUri = rawgGame.background_image;
            MetacriticScore = rawgGame.metacritic;
            AveragePlaytime_Hours = rawgGame.playtime;
            UpdatedOn = rawgGame.updated;
            RatingsCount = rawgGame.ratings_count;
            PlayerbaseProgress = rawgGame.added_by_status != null
                   ? new PlayerbaseProgress() { OwnTheGame = rawgGame.added_by_status.owned, BeatTheGame = rawgGame.added_by_status.beaten } : null;

            Ratings = rawgGame.ratings.Select(x => new Rating() { Score = x.id, Description = x.title, Count = x.count })?.ToList();
            Screenshots = rawgGame.short_screenshots.Select(x => new Screenshot() { Source = "RAWG", SourceId = x.id, Uri = x.image })?.ToList();

            // Calculated
            AverageUserScore = GetAverageUserRating();
        }
        #endregion Constructors..

        #region Methods..
        private int? GetAverageUserRating()
        {
            int? score = null;

            if (RatingsCount != null && RatingsCount > 0 && (Ratings?.Any() ?? false))
            {
                double totalScore = Ratings.Sum(x => ((x.Score - 1) * 25.0) * x.Count);
                score = (int)(totalScore / RatingsCount);
            }

            return score;
        }

        public void UpdateFromRawgGame(RawgGame rawgGame)
        {
            RawgId = rawgGame.id;
            Name = rawgGame.name;
            ReleaseDate = rawgGame.released;
            ImageUri = rawgGame.background_image;
            MetacriticScore = rawgGame.metacritic;
            AveragePlaytime_Hours = rawgGame.playtime;
            UpdatedOn = rawgGame.updated;
            RatingsCount = rawgGame.ratings_count;

            if (rawgGame.added_by_status != null)
            {
                PlayerbaseProgress = PlayerbaseProgress ?? new PlayerbaseProgress();
                PlayerbaseProgress.OwnTheGame = rawgGame.added_by_status.owned;
                PlayerbaseProgress.BeatTheGame = rawgGame.added_by_status.beaten;
            }

            if (rawgGame.ratings != null)
            {
                Ratings = Ratings ?? new List<Rating>();
                rawgGame.ratings.ForEach(rawgRating =>
                {
                    var rating = Ratings.FirstOrDefault(x => x.Score == rawgRating.id);
                    if (rating == default)
                        Ratings.Add(new Rating() { Score = rawgRating.id, Description = rawgRating.title, Count = rawgRating.count });
                    else
                        rating.Count = rawgRating.count;
                });
            }

            if (rawgGame.short_screenshots != null)
            {
                Screenshots = Screenshots ?? new List<Screenshot>();
                rawgGame.short_screenshots.ForEach(rawgScreenshot =>
                {
                    var screenshot = Screenshots.FirstOrDefault(x => x.SourceId != null && x.Source == "RAWG" && x.SourceId == rawgScreenshot.id);
                    if (screenshot == default)
                        Screenshots.Add(new Screenshot() { Source = "RAWG", SourceId = rawgScreenshot.id, Uri = rawgScreenshot.image });
                    else
                        screenshot.Uri = rawgScreenshot.image;
                });
            }

            AverageUserScore = GetAverageUserRating();
        }
        #endregion Methods..
    }
}
