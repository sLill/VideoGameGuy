namespace VideoGameGuy.Common
{
    public static class StringExtensions
    {
        #region Methods..
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) 
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        #endregion Methods..
    }
}
