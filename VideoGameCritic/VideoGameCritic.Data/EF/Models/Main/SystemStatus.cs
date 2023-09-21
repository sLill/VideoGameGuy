namespace VideoGameGuy.Data
{
    public class SystemStatus : ModelBase
    {
        #region Properties..
        public Guid SystemStatusId { get; set; }

        public DateTime? Rawg_UpdatedOnUtc { get; set; }
        public DateTime? Igdb_UpdatedOnUtc { get; set; }
        public DateTime? Application_StartedOnUtc { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..
    }
}
