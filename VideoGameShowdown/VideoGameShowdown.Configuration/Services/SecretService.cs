using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;

namespace VideoGameShowdown.Configuration
{
    public class SecretService : ISecretService
    {
        #region Fields..
        private IOptions<SecretSettings> _settings;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public SecretService(IOptions<SecretSettings> settings)
        {
            _settings = settings;
        }
        #endregion Constructors..

        #region Methods..
        private TokenCredential GetAzureCredentials()
            => new AzurePowerShellCredential();

        public async Task<string> GetAzureSecretAsync(string key)
        {
            string secret = null;

            try
            {
                var azureCredentials = GetAzureCredentials();
                var secretClient = new SecretClient(new Uri(_settings.Value.Azure_KeyVaultBaseUrl), azureCredentials);

                var response = await secretClient.GetSecretAsync(key);
                secret = response.Value.Value;
            }
            catch (Exception ex) 
            {
                // TODO: Log
            }

            return secret;
        }

        public string GetAzureSecret(string key)
        {
            string secret = null;

            try
            {
                var azureCredentials = GetAzureCredentials();
                var secretClient = new SecretClient(new Uri(_settings.Value.Azure_KeyVaultBaseUrl), azureCredentials);
              
                var response = secretClient.GetSecret(key);
                secret = response.Value.Value;
            }
            catch (Exception ex)
            {
                // TODO: Log
            }

            return secret;
        }
        #endregion Methods..
    }
}