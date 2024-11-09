using engineering_project_front.Models;

namespace engineering_project_front.Services.Interfaces
{
    public interface ITeamsService
    {
        Task<OperationResponse<List<TeamsResponse>>> GetTeamsAsync();
        Task<OperationResponse<TeamsResponse>> GetTeam(long ID);
        Task<OperationResponse<bool>> AddTeam(TeamRequest team);
        Task<OperationResponse<bool>> EditTeam(TeamRequest team);
        Task<OperationResponse<bool>> DeleteTeam(long ID);

    }
}
