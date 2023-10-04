namespace VideoGameGuy.Data
{ 
    public class SessionData
    {
        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();

        private Dictionary<Type, SessionItemBase> _sessionItems = new Dictionary<Type, SessionItemBase>();
        #endregion Properties..

        #region Methods..
        public T GetSessionItem<T>() where T : SessionItemBase, new()
        {
            _sessionItems.TryGetValue(typeof(T), out SessionItemBase sessionItem);

            if (sessionItem == null)
            {
                sessionItem = new T();
                _sessionItems[typeof(T)] = sessionItem;
            }

            return (T)sessionItem;
        }

        public void SetSessionItem<T>(T value) where T : SessionItemBase
        {
            _sessionItems[typeof(T)] = value;
        }
        #endregion Methods..
    }
}
