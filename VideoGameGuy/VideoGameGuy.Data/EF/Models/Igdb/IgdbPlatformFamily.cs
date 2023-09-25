using System;

namespace VideoGameGuy.Data
{
    public class IgdbPlatformFamily : ModelBase
    {
        #region Properties..
        public Guid IgdbPlatformFamilyId { get; set; }
        public long SourceId { get; set; }
        public Guid Checksum { get; set; }

        public string? Name { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(IgdbApiPlatformFamily platformFamily)
        {
            SourceId = platformFamily.id;
            Checksum = platformFamily.checksum;

            Name = platformFamily.name;
        }
        #endregion Methods..
    }
}
