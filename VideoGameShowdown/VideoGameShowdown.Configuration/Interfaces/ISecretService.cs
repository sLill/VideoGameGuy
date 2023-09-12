namespace VideoGameShowdown.Configuration
{
    public interface ISecretService
    {
        #region Methods..
        Task<string> GetAzureSecretAsync(string key);
        string GetAzureSecret(string key);
        #endregion Methods..
    }
}
