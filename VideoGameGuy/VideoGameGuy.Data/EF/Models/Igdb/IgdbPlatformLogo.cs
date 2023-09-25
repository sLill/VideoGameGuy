﻿namespace VideoGameGuy.Data
{
    public class IgdbPlatformLogo : ModelBase
    {
        #region Properties..
        public long IgdbPlatformLogoId { get; set; }
        public Guid Checksum { get; set; }

        public string? ImageId { get; set; }
        public string Url { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbPlatformLogo(IgdbApiPlatformLogo platformLogo)
        {
            Initialize(platformLogo);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiPlatformLogo platformLogo)
        {
            IgdbPlatformLogoId = platformLogo.id;
            Checksum = platformLogo.checksum;

            ImageId = platformLogo.image_id;
            Url = platformLogo.url;

            Height = platformLogo.height;
            Width = platformLogo.width;
        }
        #endregion Methods..
    }
}
