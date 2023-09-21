namespace VideoGameGuy.Data
{
    public class Rating : ModelBase
    {
        #region Properties..
        public Guid RatingId { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }

        public int Score { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }
        #endregion Properties..
    }
}
