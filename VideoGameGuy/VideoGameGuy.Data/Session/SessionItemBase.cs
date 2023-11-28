namespace VideoGameGuy.Data
{
    public abstract class SessionItemBase
    {
        #region Properties..
        public Guid SessionItemId { get; set; } = Guid.NewGuid();
        #endregion Properties..
    }
}
