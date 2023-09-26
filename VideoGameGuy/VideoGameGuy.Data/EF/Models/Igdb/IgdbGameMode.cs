namespace VideoGameGuy.Data
{
    public class IgdbGameMode : ModelBase
    {
        #region Properties..
        public Guid IgdbGameModeId { get; set; }
        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public string Name { get; set; }

        public long? Source_CreatedOn_Unix { get; set; }
        public long? Source_UpdatedOn_Unix { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(IgdbApiGameMode gameMode)
        {
            SourceId = gameMode.id;
            Checksum = gameMode.checksum;

            Name = gameMode.name;

            Source_CreatedOn_Unix = gameMode.created_at;
            Source_UpdatedOn_Unix = gameMode.updated_at;
        }
        #endregion Methods..
    }
}
