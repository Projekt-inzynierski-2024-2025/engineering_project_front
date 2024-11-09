using engineering_project_front.Models.Parameters;

namespace engineering_project_front.Services.Interfaces
{
    public interface ILoginService
    {
        Task<string> Login(LoginParameters loginParameter);
    }
}
