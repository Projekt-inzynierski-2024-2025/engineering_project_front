using engineering_project_front.Models.Responses;

namespace engineering_project_front.Services.Interfaces
{
    public interface IAvailabilitiesService
    {
        Task<OperationResponse<AvailabilitiesResponse>> GetAvailabilitiesForMonth(long userID, DateTime month);
    }
}
