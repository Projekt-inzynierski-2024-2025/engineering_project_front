namespace engineering_project_front.Models
{
    public class TreeData
    {
        public required string Id { get; set; }
        public string? Pid { get; set; }
        public required string Name { get; set; }
        public bool HasChild { get; set; }
        public bool Expanded { get; set; }
        public bool Selected { get; set; }
    }
}
