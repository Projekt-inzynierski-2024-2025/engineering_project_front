using engineering_project_front.Models.Responses;
using engineering_project_front.Models.Request;

namespace engineering_project_front.Services.Interfaces
{
    public interface IAvailabilitiesService
    {
        Task<OperationResponse<bool>> CreateAvailabilities(AvailabilitiesRequest request);
        Task<OperationResponse<bool>> UpdateAvailability(AvailabilitiesRequest request);
        Task<OperationResponse<bool>> RemoveAvailability(AvailabilitiesRequest request);
        Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForDay(long userID, DateTime day);
        Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForDay(DateTime day);
        Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForMonth(long userID, DateTime month);
        Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForMonth(DateTime month);
        Task<OperationResponse<List<AvailabilitiesResponse>>> GetAvailabilitiesForTeam(DateTime day, long teamID);

    }
}
