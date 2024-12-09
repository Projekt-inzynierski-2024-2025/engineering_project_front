using engineering_project.Models.Parameters;

namespace engineering_project_front.Services.Interfaces
{
    public interface IResetPassword
    {
        Task<bool> ChangePassword(ResetPasswordParameters parameters, string? token = null);
        Task SendAskForResetCode(string mail);
    }
}
