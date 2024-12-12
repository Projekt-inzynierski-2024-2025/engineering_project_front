using System.ComponentModel.DataAnnotations;

namespace engineering_project_front.Models.Responses
{
    public class AvailabilitiesResponse
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public DateTime Date
        {
            get => TimeStart.Date;
            set => TimeStart = value.Date + TimeStart.TimeOfDay;
        }
        [Required(ErrorMessage = "Wymagany jest koniec dyspozycyjności.")]
        [Range(typeof(DateTime), $"0001-01-01 05:00:00", "9999-12-31 23:00:00", ParseLimitsInInvariantCulture = true, ErrorMessage = "Czas dyspozycyjności powinien znajdować się pomiędzy 05:00 a 23:00.")]
        public DateTime TimeStart { get; set; }
        [Required(ErrorMessage = "Wymagany jest koniec dyspozycyjności.")]
        [Range(typeof(DateTime), "0001-01-01 05:00:00", "9999-12-31 23:00:00", ParseLimitsInInvariantCulture = true, ErrorMessage = "Czas dyspozycyjności powinien znajdować się pomiędzy 05:00 a 23:00.")]
        public DateTime TimeEnd { get; set; }
        public int Type { get; set; }
        public string TypeName => Type switch { 0 => "Pełny dzień", 1 => "Od Do", _ => "Nieznany" };
        public int Status { get; set; }
        public string StatusName => Status switch { 0 => "Aktywny", 1 => "Zablokowany", _ => "Nieznany" };
        public bool IsReadonly => Date.Month == DateTime.Today.Month || DateTime.Today >= new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).AddDays(-7);
        public string CategoryColor { get; set; } = "#0000FF";
    }
}
