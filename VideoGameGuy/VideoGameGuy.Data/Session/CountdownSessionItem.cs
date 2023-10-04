namespace VideoGameGuy.Data
{
    public class CountdownSessionItem : SessionItemBase
    {
        #region Properties..
        public TimeSpan TimeRemaining { get; set; } = TimeSpan.Zero;
        #endregion Properties..
    }
}
