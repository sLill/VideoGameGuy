namespace VideoGameGuy.Common
{
    public static class HttpUtils
    {
        #region Methods..
        public static bool UrlExists(string url)
        {
            bool result = false;

            using (var client = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Head, url);
                    var response = client.Send(request);
                    return response.IsSuccessStatusCode;
                }
                catch (HttpRequestException) { }
            }

            return result;
        }
        #endregion Methods..
    }
}
