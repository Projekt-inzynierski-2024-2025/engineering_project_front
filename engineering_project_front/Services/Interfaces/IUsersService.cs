using engineering_project_front.Models.Responses;

namespace engineering_project_front.Services.Interfaces
{
    public interface IUsersService
    {
        Task<UsersResponse> GetUserFromToken(string token);
    }
}
