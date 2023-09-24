namespace VideoGameGuy.Data
{
    public class IgdbMultiplayerMode : ModelBase
    {
        #region Properties..
        public Guid IgdbMultiplayerModeId { get; set; }
        
        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public List<IgdbGame>? Games { get; set; }

        public bool? CampaignCoop { get; set; }
        public bool? LanCoop { get; set; }
        public bool? OfflineCoop { get; set; }
        public bool? OnlineCoop { get; set; }
        public bool? SplitScreen { get; set; }
        public bool? SplitscreenOnline { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbMultiplayerMode(IgdbApiMultiplayerMode multiplayerMode)
        {
            SourceId = multiplayerMode.id;
            Checksum = multiplayerMode.checksum;

            CampaignCoop = multiplayerMode.campaigncoop;
            LanCoop = multiplayerMode.lancoop;
            OfflineCoop = multiplayerMode.offlinecoop;
            OnlineCoop = multiplayerMode.onlinecoop;
            SplitScreen = multiplayerMode.splitscreen;
            SplitscreenOnline = multiplayerMode.splitscreenonline;
        }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
