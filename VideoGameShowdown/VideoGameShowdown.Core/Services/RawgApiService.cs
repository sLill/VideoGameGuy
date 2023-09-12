using Microsoft.Extensions.Options;
using VideoGameShowdown.Configuration;

namespace VideoGameShowdown.Core
{
    public class RawgApiService : IRawgApiService
    {
        #region Fields..
        private IOptionsSnapshot<RawgApiSettings> _settings;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public RawgApiService(IOptionsSnapshot<RawgApiSettings> settings)
        {
            _settings = settings;
        }
        #endregion Constructors..

        #region Methods..

        #endregion Methods..
    }
}
