using engineering_project_front.Models;

namespace engineering_project_front.Services.Interfaces
{
    public interface IUsersService
    {
        Task<List<UsersResponse>> GetUsersAsync();
        Task<UsersResponse> GetUser(long ID);
        Task<bool> AddUser(UserRequest user);
        Task<bool> EditUser(UserRequest user);
        Task<List<UsersResponse>> GetMenegers();
        Task<bool> DeleteUser(long ID);
    }
}
