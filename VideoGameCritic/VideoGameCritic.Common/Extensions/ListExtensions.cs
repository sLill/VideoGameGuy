namespace VideoGameGuy.Common
{
    public static class ListExtensions
    {
        #region Methods..
        public static List<T> TakeRandom<T>(this List<T> list, int numberOfElements)
        {
            Random random = new Random();
            return list.OrderBy(x => random.Next()).Take(numberOfElements).ToList();
        }
        #endregion Methods..
    }
}
