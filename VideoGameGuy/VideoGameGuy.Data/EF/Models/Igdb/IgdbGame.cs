namespace VideoGameGuy.Data
{
    public class IgdbGame : ModelBase
    {
        #region Properties..
        public Guid IgdbGameId { get; set; }

        public long SourceId { get; set; }
        public int? ParentSourceId { get; set; }
        public Guid? Checksum { get; set; }
        public string? Name { get; set; }

        // Rating based on external critic scores
        public double? AggregateRating { get; set; }
        public int? AggregateRating_Count { get; set; }

        // Average IGDB user rating
        public double? Rating { get; set; }
        public int? Rating_Count { get; set; }

        // Average rating based on both IGDB user and external critic scores
        public double? TotalRating { get; set; }
        public int? TotalRating_Count { get; set; }

        public string? Category { get; set; }
        public long? CoverId { get; set; }
        public string? Status { get; set; }

        public string? Storyline { get; set; }
        public string? Summary { get; set; }

        public long? ReleaseDate_Unix { get; set; }
        public long? Source_CreatedOn_Unix { get; set; }
        public long? Source_UpdatedOn_Unix { get; set; }

        // Foreign Keys
        public List<IgdbPlatform>? Platforms { get; set; }
        public List<IgdbGameMode>? GameModes { get; set; }
        public List<IgdbMultiplayerMode>? MultiplayerModes { get; set; }
        public List<IgdbArtwork>? Artworks { get; set; }
        public List<IgdbScreenshot>? Screenshots { get; set; }
        public List<IgdbTheme>? Themes { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
