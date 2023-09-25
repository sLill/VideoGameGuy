using Syncfusion.Licensing;

namespace VideoGameGuy.Data
{
    public class IgdbGame : ModelBase
    {
        #region Properties..
        public long IgdbGameId { get; set; }
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
        #endregion Properties..

        #region Constructors..
        public IgdbGame(IgdbApiGame game)
        {
            Initialize(game);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiGame game)
        {
            IgdbGameId = game.id;
            ParentSourceId = game.parent_game;
            Checksum = game.checksum;

            Name = game.name;

            AggregateRating = game.aggregated_rating;
            AggregateRating_Count = game.aggregated_rating_count;

            Rating = game.rating;
            Rating_Count = game.rating_count;

            TotalRating = game.total_rating;
            TotalRating_Count = game.total_rating_count;

            Category = game.category.ToString();
            CoverId = game.cover;
            Status = game.status.ToString();

            Storyline = game.name;
            Summary = game.name;

            ReleaseDate_Unix = game.first_release_date;
            Source_CreatedOn_Unix = game.created_at;
            Source_UpdatedOn_Unix = game.updated_at;
        }
        #endregion Methods..
    }
}
