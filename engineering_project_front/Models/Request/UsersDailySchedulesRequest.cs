namespace engineering_project_front.Models.Request
{
    public class UsersDailySchedulesRequest
    {
        public long ID { get; set; }
        public required DateTime TimeStart { get; set; }
        public required DateTime TimeEnd { get; set; }
        public long UserID { get; set; }
    }
}
