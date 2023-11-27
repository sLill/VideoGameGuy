namespace VideoGameGuy.Data
{
    public class Game : ModelBase
    {
        #region Properties..
        public Guid GameId { get; set; }

        public string ClientIp { get; set; }

        public Guid SessionId { get; set; }

        public GameType GameType { get; set; }

        public string? GameScore { get; set; }
        #endregion Properties..
    }
}
