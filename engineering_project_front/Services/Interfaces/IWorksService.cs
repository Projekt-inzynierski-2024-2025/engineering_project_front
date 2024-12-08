using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;

namespace engineering_project_front.Services.Interfaces
{
    public interface IWorksService
    {
        Task<OperationResponse<bool>> StartWork(WorksRequest request);
        Task<OperationResponse<bool>> EndWork(WorksRequest request);
        Task<OperationResponse<bool>> StartBreak(WorksRequest request);
        Task<OperationResponse<bool>> EndBreak(WorksRequest request);
        Task<OperationResponse<WorksResponse>> GetWorkForDay(long userID, DateTime day);
        Task<OperationResponse<bool>> EditWork(WorksRequest request);
        Task<OperationResponse<IEnumerable<WorksResponse>>> GetWorkForMonth(long userID, DateTime month);
        Task<OperationResponse<bool>> RemoveWorkTime(WorksRequest work);
        Task<OperationResponse<bool>> ChangeWorkStatus(long userID, DateTime date);
    }
}
