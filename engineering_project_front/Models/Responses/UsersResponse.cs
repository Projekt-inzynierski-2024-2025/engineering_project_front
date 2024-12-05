namespace engineering_project_front.Models.Responses
{
    public class UsersResponse
    {
        public long ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public long TeamID { get; set; }
        public string? TeamName { get; set; }
        public string? Manager { get; set; }
        public int Role { get; set; }
        public string RoleName
        {
            get
            {
                return Role switch
                {
                    0 => "Administrator",
                    1 => "Kierownik",
                    2 => "Pracownik",
                    _ => "Unknown"
                };
            }
        }

        public bool IsAddingShift { get; set; } = false;
    }
}
