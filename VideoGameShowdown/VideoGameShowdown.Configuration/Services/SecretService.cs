using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;

namespace VideoGameShowdown.Configuration
{
    public class SecretService : ISecretService
    {
        #region Fields.. 
        private readonly ILogger<SecretService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public SecretService(ILogger<SecretService> logger,
                             IServiceProvider serviceProvider, 
                             IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }
        #endregion Constructors..

        #region Methods..
        private TokenCredential GetAzureCredentials()
            => new AzurePowerShellCredential();

        public string GetAzureSecret(string key)
        {
            string secret = null;
            
            try 
            { 
                var azureSecretSettings = _serviceProvider.GetService<IOptions<AzureSecretSettings>>();
                var azureCredentials = GetAzureCredentials();
                var secretClient = new SecretClient(new Uri(azureSecretSettings.Value.KeyVaultBaseUrl), azureCredentials);
              
                var response = secretClient.GetSecret(key);
                secret = response.Value.Value;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{ex.Message} - {ex.StackTrace}");
                throw;
            }

            return secret;
        }

        public UserSecretSettings GetUserSecretSettings()
        {
            UserSecretSettings userSecretSettings = null;

            try
            {
                userSecretSettings = _configuration.Get<UserSecretSettings>();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{ex.Message} - {ex.StackTrace}");
                throw;
            }

            return userSecretSettings;
        }
        #endregion Methods..
    }
}