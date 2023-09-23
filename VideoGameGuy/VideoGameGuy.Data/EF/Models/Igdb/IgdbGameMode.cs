namespace VideoGameGuy.Data
{
    public class IgdbGameMode : ModelBase
    {
        #region Properties..
        public Guid IgdbGameModeId { get; set; }

        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public List<IgdbGame>? Games { get; set; }

        public string Name { get; set; }

        public long Source_CreatedOn_Unix { get; set; }
        public long Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
