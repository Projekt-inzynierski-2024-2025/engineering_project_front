namespace engineering_project_front.Models.Responses
{
    public class HoursForDayResponse
    {
        public long DailyScheduleID { get; set; }
        public DateTime Date { get; set; }
        public double? WorkHours { get; set; }
        public double ToDoHours { get; set; }
    }
}
