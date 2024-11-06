using System;

namespace engineering_project_front.Models
{
    public class UsersResponse
    {
        public long ID { get; set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public string Email { get; set; }
        public  long TeamID { get; set; }
        public string TeamName { get; set; }
        public  int Role { get; set; }
    }
}
