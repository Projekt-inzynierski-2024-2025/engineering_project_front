using engineering_project_front.Models;

namespace engineering_project_front.Services.Interfaces
{
    public interface ITeamsService
    {
        Task<List<TeamsResponse>> GetTeamsAsync();
        Task<TeamsResponse> GetTeam(long ID);
        Task<bool> AddTeam(TeamRequest team);
        Task<bool> EditTeam(TeamRequest team);
        Task<bool> DeleteTeam(long ID);

    }
}
