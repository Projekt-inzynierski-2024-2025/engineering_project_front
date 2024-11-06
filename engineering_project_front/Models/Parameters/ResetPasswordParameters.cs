namespace engineering_project.Models.Parameters
{
    public class ResetPasswordParameters
    {
        public required string Code { get; set; }
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
