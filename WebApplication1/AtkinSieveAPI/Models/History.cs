namespace SieveApp.Models
{
    public class History
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RequestType { get; set; }
        public string RequestData { get; set; }
        public DateTime Timestamp { get; set; }
    }
}