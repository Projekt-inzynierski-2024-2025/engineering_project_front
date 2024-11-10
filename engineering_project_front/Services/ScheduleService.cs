using System.Net.Http.Json;
using System.Text.Json;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;

namespace engineering_project_front.Services
{
    public class ScheduleService: IScheduleService
    {
        private readonly ILogger<ScheduleService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        public ScheduleService(ILogger<ScheduleService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        public async Task<OperationResponse<List<HoursForDayResponse>>> GetHoursForEachDayForMonthAsync(int year, int month, long teamID)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetDailyScheduleUsersHoursAmount/{year}/{month}/{teamID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<HoursForDayResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };

                }

                var hours = await apiResponse.Content.ReadFromJsonAsync<List<HoursForDayResponse>>(_serializerOptions);
                return new OperationResponse<List<HoursForDayResponse>>
                {
                    Success = true,
                    Data = hours,
                    Message = $"Pobrano {hours?.Count} godzin."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching hours.");
                return new OperationResponse<List<HoursForDayResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania godzij"
                };
            }
        }



        public async Task<OperationResponse<bool>> AddSchedule(DailySchedulesRequest request)
        {
            _logger.LogInformation("Adding schedule to API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/DailySchedules/addDailySchedules", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };

                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = "Dodano nowy harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding schedule.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas dodawania harmonogramu."
                };
            }
        }

        public async Task<OperationResponse<Double>> GetHoursForDayForTeam(long teamID, DateTime date)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var formattedDate = date.ToString("yyyy-MM-dd");
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetDailyScheduleUsersHoursAmount/{teamID}/{formattedDate}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<Double>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };

                }

                var hours = await apiResponse.Content.ReadFromJsonAsync<Double>(_serializerOptions);
                return new OperationResponse<Double>
                {
                    Success = true,
                    Data = hours,
                    Message = $"Pobrano {hours} godzin."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching hours.");
                return new OperationResponse<Double>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania godzij"
                };
            }
        }

        public async Task<OperationResponse<DailySchedulesResponse>> GetDailySchedule(long ID)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/DailySchedules/GetDailySchedule/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<DailySchedulesResponse>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };

                }

                var schedule = await apiResponse.Content.ReadFromJsonAsync<DailySchedulesResponse>(_serializerOptions);
                return new OperationResponse<DailySchedulesResponse>
                {
                    Success = true,
                    Data = schedule,
                    Message = $"Pobrano harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule.");
                return new OperationResponse<DailySchedulesResponse>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania harmonogramu."
                };
            }
        }

        public async Task<OperationResponse<List<UsersDailySchedulesResponse>>> GetUsersDailySchedules(long teamID, DateTime date)
            {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var formattedDate = date.ToString("yyyy-MM-dd");
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetUsersDailySchedulesForDay/{teamID}/{formattedDate}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<UsersDailySchedulesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };

                }

                var schedules = await apiResponse.Content.ReadFromJsonAsync<List<UsersDailySchedulesResponse>>(_serializerOptions);
                return new OperationResponse<List<UsersDailySchedulesResponse>>
                {
                    Success = true,
                    Data = schedules,
                    Message = $"Pobrano harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule.");
                return new OperationResponse<List<UsersDailySchedulesResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania harmonogramu."
                };
            }
        }







    }
}
