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
        public string TypeName => Type switch { 0 => "Pełny dzień", 1 => "Od Do", _ => "Nieznany" };
        public int Status { get; set; }
        public string StatusName => Status switch { 0 => "Aktywny", 1 => "Zablokowany", _ => "Nieznany" };
    }
}
