namespace engineering_project_front.Models.Request
{
    public class DailySchedulesRequest
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public int HoursAmount { get; set; }
        public int Status { get; set; }
        public long TeamID { get; set; }
    }
}
