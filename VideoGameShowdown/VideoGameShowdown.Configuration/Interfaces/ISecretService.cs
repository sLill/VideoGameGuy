namespace VideoGameShowdown.Configuration
{
    public interface ISecretService
    {
        #region Methods..
        string GetAzureSecret(string key);
        UserSecretSettings GetUserSecretSettings();
        #endregion Methods..
    }
}
