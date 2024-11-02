using engineering_project_front.Models;

namespace engineering_project_front.Services.Interfaces
{
    public interface IUsersService
    {
        Task<List<UsersResponse>> GetUsersAsync();
    }
}
