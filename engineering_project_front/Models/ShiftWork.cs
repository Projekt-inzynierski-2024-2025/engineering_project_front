namespace engineering_project_front.Models
{
    public class ShiftWork
    { 
        public long ID { get; set; }
        public DateTime Date { get; set; } = new DateTime(1977, 1, 1, 0, 0, 0);
        public DateTime TimeStartShift { get; set; } = new DateTime(1977, 1, 1, 0, 0, 0);
        public DateTime TimeEndShift { get; set; } = new DateTime(1977, 1, 1, 0,0, 0);
        public DateTime TimeStartWork { get; set; } = new DateTime(1977, 1, 1, 0, 0, 0);
        public DateTime TimeEndWork { get; set; } = new DateTime(1977, 1, 1, 0, 0, 0);
        public DateTime TimeStartBreak { get; set; } = new DateTime(1977, 1, 1, 0, 0, 0);
        public DateTime TimeEndBreak { get; set; } = new DateTime(1977, 1, 1, 0, 0, 0);
    }
}
