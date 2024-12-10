using System.Net.Http.Json;
using System.Text.Json;
using Blazored.SessionStorage;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;

namespace engineering_project_front.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ILogger<ScheduleService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ISessionStorageService _sessionStorage;

        public ScheduleService(ILogger<ScheduleService> logger, IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _sessionStorage = sessionStorage;
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

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetHoursForEachDayInMonth/{year}/{month}/{teamID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<HoursForDayResponse>>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PostAsJsonAsync("api/DailySchedules/addDailySchedules", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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
        public async Task<OperationResponse<bool>> UpdateSchedule(DailySchedulesRequest request)
        {
            _logger.LogInformation("Updating schedule in API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PutAsJsonAsync("api/DailySchedules/UpdateDailySchedule", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = "Zaktualizowano harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas aktualizacji harmonogramu."
                };
            }
        }
        public async Task<OperationResponse<bool>> DeleteSchedule(long ID)
        {
            _logger.LogInformation("Deleting schedule from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.DeleteAsync($"api/DailySchedules/RemoveDailySchedule/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = "Usunięto harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas usuwania harmonogramu."
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

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetHoursForDayForTeam/{teamID}/{formattedDate}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<Double>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/DailySchedules/GetDailySchedule/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<DailySchedulesResponse>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetUsersDailySchedulesForDay/{teamID}/{formattedDate}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<UsersDailySchedulesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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

        public async Task<OperationResponse<bool>> AddUserSchedule(UsersDailySchedulesRequest request)
        {
            _logger.LogInformation("Adding user schedule to API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PostAsJsonAsync("api/UsersDailySchedules/AddUserDailySchedule", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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
                _logger.LogError(ex, "Error adding user schedule.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas dodawania harmonogramu."
                };
            }
        }

        public async Task<OperationResponse<bool>> UpdateUserSchedule(UsersDailySchedulesRequest request)
        {
            _logger.LogInformation("Updating user schedule in API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PutAsJsonAsync("api/UsersDailySchedules/UpdateUserDailySchedule", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = "Zaktualizowano harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user schedule.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas aktualizacji harmonogramu."
                };
            }
        }
        public async Task<OperationResponse<bool>> DeleteUserSchedule(long ID)
        {
            _logger.LogInformation("Deleting user schedule from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.DeleteAsync($"api/UsersDailySchedules/DeleteUserDailySchedule/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = "Usunięto harmonogram."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user schedule.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas usuwania harmonogramu."
                };
            }
        }

        public async Task<OperationResponse<List<HoursForUserForMonthResponse>>> GetUsersHoursForMonth(int year, int month, long teamID)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetUsersHoursForMonth/{year}/{month}/{teamID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<HoursForUserForMonthResponse>>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                var hours = await apiResponse.Content.ReadFromJsonAsync<List<HoursForUserForMonthResponse>>(_serializerOptions);
                return new OperationResponse<List<HoursForUserForMonthResponse>>
                {
                    Success = true,
                    Data = hours,
                    Message = $"Pobrano {hours?.Count} godzin."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching hours.");
                return new OperationResponse<List<HoursForUserForMonthResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania godzij"
                };
            }
        }


        public async Task<OperationResponse<List<UsersDailySchedulesResponse>>> GetUsersDailySchedulesForMonth(long userID, DateTime month)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var formattedDate = month.ToString("yyyy-MM-dd");
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/UsersDailySchedules/GetUsersDailySchedulesForMonth/{userID}/{formattedDate}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<UsersDailySchedulesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
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

        public async Task<OperationResponse<bool>> GetEditStatusMonthSchedule(long teamID, int year, int month)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.GetAsync($"api/DailySchedules/GetEditStatusMonthSchedule/{teamID}/{year}/{month}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                var status = await apiResponse.Content.ReadFromJsonAsync<bool>(_serializerOptions);
                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = status,
                    Message = $"Pobrano status."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching status.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania statusu."
                };
            }
        }

        public async Task<OperationResponse<bool>> ChangeEditStatusMonthSchedule(long teamID, int year, int month)
        {
            _logger.LogInformation("Fetching schedule from API.");
            try
            {

                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PutAsync($"api/DailySchedules/ChangeStatusMonthSchedule/{teamID}/{year}/{month}", null);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd: {errorMessage}"
                    };

                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = $"Zmieniono status."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing status.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zmiany statusu."
                };
            }
        }
    }
}
