namespace VideoGameShowdown.Data
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

        public double? ReviewScore_Percent { get; set; }

        public int? MetacriticScore { get; set; }

        public int? AveragePlaytime { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? ReviewCount { get; set; }

        // Foreign Key Relationships
        public PlayerbaseProgress? PlayerbaseProgress { get; set; }

        public List<Screenshot>? Screenshots { get; set; }
        #endregion Properties..
    }
}
