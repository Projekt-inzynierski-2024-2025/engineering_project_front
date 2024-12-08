using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;

namespace engineering_project_front.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<OperationResponse<List<HoursForDayResponse>>> GetHoursForEachDayForMonthAsync(int year, int month, long teamID);
        Task<OperationResponse<bool>> AddSchedule(DailySchedulesRequest request);
        Task<OperationResponse<bool>> UpdateSchedule(DailySchedulesRequest request);
        Task<OperationResponse<bool>> DeleteSchedule(long ID);
        Task<OperationResponse<Double>> GetHoursForDayForTeam(long teamID, DateTime date);
        Task<OperationResponse<DailySchedulesResponse>> GetDailySchedule(long ID);
        Task<OperationResponse<List<UsersDailySchedulesResponse>>> GetUsersDailySchedules(long teamID, DateTime date);
        Task<OperationResponse<bool>> AddUserSchedule(UsersDailySchedulesRequest request);
        Task<OperationResponse<bool>> UpdateUserSchedule(UsersDailySchedulesRequest request);
        Task<OperationResponse<bool>> DeleteUserSchedule(long ID);
        Task<OperationResponse<List<HoursForUserForMonthResponse>>> GetUsersHoursForMonth(int year, int month, long teamID);
        Task<OperationResponse<List<UsersDailySchedulesResponse>>> GetUsersDailySchedulesForMonth(long userID, DateTime month);
        Task<OperationResponse<bool>> GetEditStatusMonthSchedule(long teamID, int year, int month);
        Task<OperationResponse<bool>> ChangeEditStatusMonthSchedule(long teamID, int year, int month);
    }
}
