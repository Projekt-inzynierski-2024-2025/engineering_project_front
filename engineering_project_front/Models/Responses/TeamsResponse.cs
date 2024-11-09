namespace engineering_project_front.Models.Responses
{
    public class TeamsResponse
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public long ManagerID { get; set; }
    }
}
