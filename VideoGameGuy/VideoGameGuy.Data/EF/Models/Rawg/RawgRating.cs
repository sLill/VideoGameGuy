namespace VideoGameGuy.Data
{
    public class RawgRating : ModelBase
    {
        #region Properties..
        public Guid RawgRatingId { get; set; }

        public Guid GameId { get; set; }
        public RawgGame Game { get; set; }

        public int Score { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }
        #endregion Properties..
    }
}
