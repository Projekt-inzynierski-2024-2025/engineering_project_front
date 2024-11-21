using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;


namespace engineering_project_front.Services.Interfaces
{
    public interface IUsersService
    {

        Task<OperationResponse<List<UsersResponse>>> GetUsersAsync();
        Task<OperationResponse<UsersResponse>> GetUser(long ID);
        Task<OperationResponse<bool>> AddUser(UserRequest user);
        Task<OperationResponse<bool>> EditUser(UserRequest user);
        Task<OperationResponse<List<UsersResponse>>> GetManagers();
        Task<OperationResponse<bool>> DeleteUser(long ID);
        Task<UsersResponse> GetUserFromToken(string token);
        Task<OperationResponse<List<UsersResponse>>> GetUserByTeam(long ID);

    }
}
