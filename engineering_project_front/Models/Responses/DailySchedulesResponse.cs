namespace engineering_project_front.Models.Responses
{
    public class DailySchedulesResponse
    {
        public long ID { get; set; }
        public  DateTime Date { get; set; }
        public  int HoursAmount { get; set; }
        public  int Status { get; set; }
        public long TeamID { get; set; }
    }
}
