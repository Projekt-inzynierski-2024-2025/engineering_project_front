namespace engineering_project_front.Models.Request
{
    public class AvailabilitiesRequest
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
    }
}
