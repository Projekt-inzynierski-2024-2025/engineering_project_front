using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;

namespace engineering_project_front.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<OperationResponse<List<HoursForDayResponse>>> GetHoursForEachDayForMonthAsync(int year, int month, long teamID);
        Task<OperationResponse<bool>> AddSchedule(DailySchedulesRequest request);
        Task<OperationResponse<Double>> GetHoursForDayForTeam(long teamID, DateTime date);
        Task<OperationResponse<DailySchedulesResponse>> GetDailySchedule(long ID);
        Task<OperationResponse<List<UsersDailySchedulesResponse>>> GetUsersDailySchedules(long teamID, DateTime date);
    }
}
