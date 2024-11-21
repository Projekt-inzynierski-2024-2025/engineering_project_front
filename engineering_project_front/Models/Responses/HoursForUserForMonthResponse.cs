namespace engineering_project_front.Models.Responses
{
    public class HoursForUserForMonthResponse
    {
        public long userID { get; set; }
        public string userName { get; set; }
        public string userSurname { get; set; }
        public double workHoursForMonth { get; set; }


    }
}
