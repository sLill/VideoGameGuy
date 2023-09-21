namespace VideoGameGuy.Data
{
    public class ApplicationLog : ModelBase
    {
        #region Properties..
        public Guid LogId { get; set; }
        public string LogLevel { get; set; }
        public int EventId { get; set; }
        public string Category {  get; set; }
        public string Message { get; set; }
        public string Exception { get; set; } 
        #endregion Properties..
    }
}
