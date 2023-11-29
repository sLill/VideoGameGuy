namespace VideoGameGuy.Data
{
    public class RawgGame : ModelBase
    {
        #region Properties..
        public Guid RawgGameId { get; set; }

        public int? SourceId { get; set; }

        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? ImageUri { get; set; }

        public int? MetacriticScore { get; set; }

        public int? AverageUserScore { get; set; }

        public int? AveragePlaytime_Hours { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? RatingsCount { get; set; }

        // Foreign Key Relationships
        public RawgPlayerbaseProgress? PlayerbaseProgress { get; set; }

        public List<RawgRating> Ratings { get; set; }

        public List<RawgScreenshot>? Screenshots { get; set; }
        #endregion Properties..

        #region Constructors..
        public RawgGame() { }

        public RawgGame(RawgApiGame rawgApiGame)
        {
            SourceId = rawgApiGame.id;
            Name = rawgApiGame.name;
            ReleaseDate = rawgApiGame.released;
            ImageUri = rawgApiGame.background_image;
            MetacriticScore = rawgApiGame.metacritic;
            AveragePlaytime_Hours = rawgApiGame.playtime;
            UpdatedOn = rawgApiGame.updated;
            RatingsCount = rawgApiGame.ratings_count;
            PlayerbaseProgress = rawgApiGame.added_by_status != null
                   ? new RawgPlayerbaseProgress() { OwnTheGame = rawgApiGame.added_by_status.owned, BeatTheGame = rawgApiGame.added_by_status.beaten } : null;

            Ratings = rawgApiGame.ratings?.Select(x => new RawgRating() { Score = x.id, Description = x.title, Count = x.count })?.ToList();
            Screenshots = rawgApiGame.short_screenshots?.Select(x => new RawgScreenshot() { Source = "RAWG", SourceId = x.id, Uri = x.image })?.ToList();

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
                score += 5;
            }

            return score;
        }

        public int? GetAverageOverallRating()
        {
            int totalRating = 0;
            int ratingsCount = 0;

            if (MetacriticScore.HasValue)
            {
                totalRating += MetacriticScore.Value;
                ratingsCount++;
            }

            if (AverageUserScore.HasValue)
            { 
                totalRating += AverageUserScore.Value;
                ratingsCount++;
            }

            if (ratingsCount > 0)
                totalRating = totalRating / ratingsCount;

            return totalRating;
        }

        public void UpdateFromRawgGame(RawgApiGame rawgApiGame)
        {
            SourceId = rawgApiGame.id;
            Name = rawgApiGame.name;
            ReleaseDate = rawgApiGame.released;
            ImageUri = rawgApiGame.background_image;
            MetacriticScore = rawgApiGame.metacritic;
            AveragePlaytime_Hours = rawgApiGame.playtime;
            UpdatedOn = rawgApiGame.updated;
            RatingsCount = rawgApiGame.ratings_count;

            if (rawgApiGame.added_by_status != null)
            {
                PlayerbaseProgress = PlayerbaseProgress ?? new RawgPlayerbaseProgress();
                PlayerbaseProgress.OwnTheGame = rawgApiGame.added_by_status.owned;
                PlayerbaseProgress.BeatTheGame = rawgApiGame.added_by_status.beaten;
            }

            if (rawgApiGame.ratings != null)
            {
                Ratings = Ratings ?? new List<RawgRating>();
                rawgApiGame.ratings.ForEach(rawgRating =>
                {
                    var rating = Ratings.FirstOrDefault(x => x.Score == rawgRating.id);
                    if (rating == default)
                        Ratings.Add(new RawgRating() { Score = rawgRating.id, Description = rawgRating.title, Count = rawgRating.count });
                    else
                        rating.Count = rawgRating.count;
                });
            }

            if (rawgApiGame.short_screenshots != null)
            {
                Screenshots = Screenshots ?? new List<RawgScreenshot>();
                rawgApiGame.short_screenshots.ForEach(rawgScreenshot =>
                {
                    var screenshot = Screenshots.FirstOrDefault(x => x.SourceId != null && x.Source == "RAWG" && x.SourceId == rawgScreenshot.id);
                    if (screenshot == default)
                        Screenshots.Add(new RawgScreenshot() { Source = "RAWG", SourceId = rawgScreenshot.id, Uri = rawgScreenshot.image });
                    else
                        screenshot.Uri = rawgScreenshot.image;
                });
            }

            AverageUserScore = GetAverageUserRating();
        }
        #endregion Methods..
    }
}
