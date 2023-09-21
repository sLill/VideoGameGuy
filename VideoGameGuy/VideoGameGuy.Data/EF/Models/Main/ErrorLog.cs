namespace VideoGameGuy.Data
{
    public class ErrorLog : ModelBase
    {
        #region Properties..
        public Guid ErrorId { get; set; }
        public string LogLevel { get; set; }
        public int EventId { get; set; }
        public string Category {  get; set; }
        public string Message { get; set; }
        public string Exception { get; set; } 
        #endregion Properties..
    }
}
