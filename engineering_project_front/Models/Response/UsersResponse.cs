using System;

namespace engineering_project_front.Models
{
    public class UsersResponse
    {
        public long ID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string TeamName { get; set; }
        public required string Role { get; set; }
    }
}
