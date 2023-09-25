using System;

namespace VideoGameGuy.Data
{
    public class IgdbPlatformFamily : ModelBase
    {
        #region Properties..
        public long IgdbPlatformFamilyId { get; set; }
        public Guid Checksum { get; set; }

        public string? Name { get; set; }
        #endregion Properties..

        #region Constructors..
        public IgdbPlatformFamily(IgdbApiPlatformFamily platformFamily)
        {
            Initialize(platformFamily);
        }
        #endregion Constructors..

        #region Methods..
        public void Initialize(IgdbApiPlatformFamily platformFamily)
        {
            IgdbPlatformFamilyId = platformFamily.id;
            Checksum = platformFamily.checksum;

            Name = platformFamily.name;
        }
        #endregion Methods..
    }
}
