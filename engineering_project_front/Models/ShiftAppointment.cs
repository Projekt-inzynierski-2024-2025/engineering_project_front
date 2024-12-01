namespace engineering_project_front.Models
{
    public class ShiftAppointment
    {
        public long Id { get; set; }
        public string Subject { get; set; } = "Shift";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long EmployeeID { get; set; }
    }
}
