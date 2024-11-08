namespace engineering_project.Models.Parameters
{
    public class ResetPasswordParameters
    {
        public string? Code { get; set; }
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
