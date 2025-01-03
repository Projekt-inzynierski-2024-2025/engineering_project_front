namespace engineering_project_front.Models
{
    public class ShiftWork
    {
        public long ID { get; set; }
        public DateTime Date { get; set; } = new DateTime(1977, 1, 1);
        public TimeOnly TimeStartShift { get; set; } = new TimeOnly(0, 0);
        public TimeOnly TimeEndShift { get; set; } = new TimeOnly(0, 0);
        public TimeOnly TimeStartWork { get; set; } = new TimeOnly(0, 0);
        public TimeOnly TimeEndWork { get; set; } = new TimeOnly(0, 0);
        public TimeOnly TimeStartBreak { get; set; } = new TimeOnly(0, 0);
        public TimeOnly TimeEndBreak { get; set; } = new TimeOnly(0, 0);
    }
}
