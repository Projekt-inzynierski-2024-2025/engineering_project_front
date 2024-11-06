using engineering_project.Constants;

namespace engineering_project_front.Models.Responses
{
    public class UsersResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? TeamName { get; set; }
        public string? Manager { get; set; }
        public Enums.Role Role { get; set; }
    }
}
