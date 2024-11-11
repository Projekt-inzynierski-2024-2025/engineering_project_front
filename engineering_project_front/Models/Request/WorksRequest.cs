namespace engineering_project_front.Models.Request
{
    public class WorksRequest
    {
        public long UserID { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public DateTime TimeStart { get; set; } = DateTime.Now;
        public DateTime TimeEnd { get; set; } = new DateTime();
        public DateTime BreakStart { get; set; } = new DateTime();
        public DateTime BreakEnd { get; set; } = new DateTime();
        public int Status { get; set; }
    }
}