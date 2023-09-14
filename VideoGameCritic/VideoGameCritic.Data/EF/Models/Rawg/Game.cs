using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameCritic.Data
{
    public class Game
    {
        #region Properties..
        public Guid GameId { get; set; }

        public int? RawgId { get; set; }

        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? ImageUri { get; set; }

        public double? ReviewScore {  get; set; }

        public double? ReviewMaxScore { get; set; }
        
        [NotMapped]
        public double? ReviewScore_Percent { get; set; }

        public int? MetacriticScore { get; set; }

        public int? AveragePlaytime_Hours { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? ReviewCount { get; set; }

        // Foreign Key Relationships
        public PlayerbaseProgress? PlayerbaseProgress { get; set; }

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
            ReviewScore = rawgGame.rating;
            ReviewMaxScore = rawgGame.rating_top;
            MetacriticScore = rawgGame.metacritic;
            AveragePlaytime_Hours = rawgGame.playtime;
            UpdatedOn = rawgGame.updated;
            ReviewCount = rawgGame.reviews_count;
            PlayerbaseProgress = rawgGame.added_by_status != null
                   ? new PlayerbaseProgress() { OwnTheGame = rawgGame.added_by_status.owned, BeatTheGame = rawgGame.added_by_status.beaten } : null;
            Screenshots = rawgGame.short_screenshots.Select(x => new Screenshot() { Source = "RAWG", SourceId = x.id, Uri = x.image })?.ToList();
        }
        #endregion Constructors..

        #region Methods..
        public void UpdateFromRawgGame(RawgGame rawgGame)
        {
            RawgId = rawgGame.id;
            Name = rawgGame.name;
            ReleaseDate = rawgGame.released;
            ImageUri = rawgGame.background_image;
            ReviewScore = rawgGame.rating;
            ReviewMaxScore = rawgGame.rating_top;
            MetacriticScore = rawgGame.metacritic;
            AveragePlaytime_Hours = rawgGame.playtime;
            UpdatedOn = rawgGame.updated;
            ReviewCount = rawgGame.reviews_count;

            if (rawgGame.added_by_status != null)
            {
                PlayerbaseProgress = PlayerbaseProgress ?? new PlayerbaseProgress();
                PlayerbaseProgress.OwnTheGame = rawgGame.added_by_status.owned;
                PlayerbaseProgress.BeatTheGame = rawgGame.added_by_status.beaten;
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
        }
        #endregion Methods..
    }
}
