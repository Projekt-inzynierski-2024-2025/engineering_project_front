namespace engineering_project_front.Models.Responses
{
    public class UsersDailySchedulesResponse
    {
        public long ID { get; set; }
        public  DateTime TimeStart { get; set; }
        public  DateTime TimeEnd { get; set; }
        public long UserID { get; set; }

        public bool IsEditing { get; set; } = false;
    }
}
