using System.ComponentModel;

namespace VideoGameGuy.Data
{
    #region Enums..
    public enum IgdbPlatformCategory
    {
        console,
        arcade,
        platform,
        operating_system,
        portable_console,
        computer,
    }

    public enum IgdbCategory
    {
        main_game,
        dlc_addon,
        expansion,
        bundle,
        standalone_expansion,
        mod,
        episode,
        season,
        remake,
        remaster,
        expanded_game,
        port,
        fork,
        pack,
        update,
    }

    public enum IgdbStatus
    {
        main_game,
        dlc_addon,
        expansion,
        bundle,
        standalone_expansion,
        mod,
        episode,
        season,
        remake,
        remaster,
        expanded_game,
        port,
        fork,
        pack,
        update,
    }

    public enum IgdbScreenshotSize
    {
        //	90 x 128	Fit
        [DefaultValue("cover_small")]
        cover_small,

        // 569 x 320	Lfill, Center gravity
        [DefaultValue("screenshot_med")]
        screenshot_med,

        // 264 x 374	Fit
        [DefaultValue("cover_big")]
        cover_big,

        // 284 x 160	Fit
        [DefaultValue("logo_med")]
        logo_med,

        //	889 x 500	Lfill, Center gravity
        [DefaultValue("screenshot_big")]
        screenshot_big,

        //	1280 x 720	Lfill, Center gravity
        [DefaultValue("screenshot_huge")]
        screenshot_huge,

        // 90 x 90	Thumb, Center gravity
        [DefaultValue("thumb")]
        thumb,

        // 35 x 35	Thumb, Center gravity
        [DefaultValue("micro")]
        micro,

        // 1280 x 720	Fit, Center gravity
        [DefaultValue("720p")]
        i720p,

        // 1920 x 1080	Fit, Center gravity
        [DefaultValue("1080p")]
        i1080p
    }
    #endregion Enums..

    // https://api-docs.igdb.com/#game
    public class IgdbApiGame
    {
        #region Properties..
        public long id { get; set; }
        public double aggregated_rating { get; set; }
        public int aggregated_rating_count { get; set; }
        public List<IgdbApiArtwork> artworks { get; set; } = new List<IgdbApiArtwork>();
        public IgdbCategory category { get; set; }
        public Guid checksum { get; set; }
        public long cover { get; set; }
        public long created_at { get; set; }
        public long first_release_date { get; set; }
        public List<IgdbApiGameMode> game_modes { get; set; } = new List<IgdbApiGameMode>();
        public List<IgdbApiMultiplayerMode> multiplayer_modes { get; set; } = new List<IgdbApiMultiplayerMode>();
        public string name { get; set; }
        public string slug { get; set; }
        public int parent_game { get; set; }
        public List<IgdbApiPlatform> platforms { get; set; } = new List<IgdbApiPlatform>();
        public double rating { get; set; }
        public int rating_count { get; set; }
        public List<IgdbApiScreenshot> screenshots { get; set; } = new List<IgdbApiScreenshot>();
        public IgdbStatus status { get; set; }
        public string storyline { get; set; }
        public string summary { get; set; }
        public List<IgdbApiTheme> themes { get; set; } = new List<IgdbApiTheme>();
        public double total_rating { get; set; }
        public int total_rating_count { get; set; }
        public long updated_at { get; set; }
        #endregion Properties..
    }

    public class IgdbApiArtwork
    {
        #region Properties..
        public long id { get; set; }
        public Guid checksum { get; set; }
        public long game { get; set; }
        public int height { get; set; }
        public string image_id { get; set; }
        public string url { get; set; }
        public int width { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiGameMode
    {
        #region Properties..
        public long id { get; set; }
        public Guid checksum { get; set; }
        public long created_at { get; set; }
        public string name { get; set; }
        public long updated_at { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiMultiplayerMode
    {
        #region Properties..
        public long id { get; set; }
        public bool campaigncoop { get; set; }
        public Guid checksum { get; set; }
        public bool dropin { get; set; }
        public long game { get; set; }
        public bool lancoop { get; set; }
        public bool offlinecoop { get; set; }
        public bool onlinecoop { get; set; }
        public long platform { get; set; }
        public bool splitscreen { get; set; }
        public bool splitscreenonline { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiPlatform
    {
        #region Properties..
        public long id { get; set; }
        public IgdbPlatformCategory category { get; set; }
        public Guid checksum { get; set; }
        public long created_at { get; set; }
        public string name { get; set; }
        public long platform_family { get; set; }
        public long platform_logo { get; set; }
        public long updated_at { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiPlatformFamily
    {
        #region Properties..
        public long id { get; set; }
        public Guid checksum { get; set; }
        public string name { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiPlatformLogo
    {
        #region Properties..
        public long id { get; set; }
        public Guid checksum { get; set; }
        public int height { get; set; }
        public string image_id { get; set; }
        public string url { get; set; }
        public int width { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiScreenshot
    {
        #region Properties..
        public long id { get; set; }
        public Guid checksum { get; set; }
        public long game { get; set; }
        public int height { get; set; }
        public string image_id { get; set; }
        public string url { get; set; }
        public int width { get; set; } 
        #endregion Properties..
    }

    public class IgdbApiTheme
    {
        #region Properties..
        public long id { get; set; }
        public Guid checksum { get; set; }
        public long created_at { get; set; }
        public string name { get; set; }
        public long updated_at { get; set; }
        public string url { get; set; } 
        #endregion Properties..
    }
}
