﻿namespace engineering_project_front.Models.Responses
{
    public class WorksResponse
    {
        public long UserID { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public DateTime BreakStart { get; set; }
        public DateTime BreakEnd { get; set; }
        public DateTime BreakTime
        {
            get => BreakEnd != DateTime.MinValue ? BreakEnd - BreakStart.TimeOfDay : DateTime.MinValue;
        }
        public DateTime WorkTime
        {
            get => TimeEnd != DateTime.MinValue ? TimeEnd - TimeStart.TimeOfDay - BreakTime.TimeOfDay : DateTime.MinValue;
        }
        public int Status { get; set; }
        public string StatusName => Status switch { 0 => "Aktywny", 1 => "Ukończony", _ => "Nieznany" };
    }
}