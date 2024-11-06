using engineering_project.Models.Parameters;

namespace engineering_project_front.Services.Interfaces
{
    public interface IResetPassword
    {
        bool ChangePassword(ResetPasswordParameters parameters);
        Task SendAskForResetCode(string mail);
    }
}
