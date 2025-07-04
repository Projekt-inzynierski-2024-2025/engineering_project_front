﻿using Blazored.SessionStorage;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class WorksService : IWorksService
    {
        private readonly ILogger<WorksService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ISessionStorageService _sessionStorage;

        public WorksService(ILogger<WorksService> logger, IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _sessionStorage = sessionStorage;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<OperationResponse<bool>> EndBreak(WorksRequest request)
        {
            _logger.LogInformation($"Method {nameof(EndBreak)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PostAsJsonAsync("api/Works/EndBreak", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Pomyślnie zakończono przerwę. Możesz spokojnie wrócić do pracy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while ending break.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas kończenia przerwy."
                };
            }
        }

        public async Task<OperationResponse<bool>> EndWork(WorksRequest request)
        {
            _logger.LogInformation($"Method {nameof(EndWork)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PostAsJsonAsync("api/Works/EndWork", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Pomyślnie zakończono pracę. Do zobaczenia!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas kończenia pracy."
                };
            }
        }

        public async Task<OperationResponse<WorksResponse>> GetWorkForDay(long userID, DateTime day)
        {
            _logger.LogInformation($"Method {nameof(GetWorkForDay)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                string uri = $"api/Works/GetWorkTimeForDay/{userID}/{day.ToString("yyyy-MM-dd")}";
                var apiResponse = await httpClient.GetAsync(uri);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<WorksResponse>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var time = await apiResponse.Content.ReadFromJsonAsync<WorksResponse>(_serializerOptions);

                return new OperationResponse<WorksResponse>
                {
                    Success = true,
                    Data = time,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting work.");
                return new OperationResponse<WorksResponse>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas rozpoczynania przerwy."
                };
            }
        }

        public async Task<OperationResponse<bool>> StartBreak(WorksRequest request)
        {
            _logger.LogInformation($"Method {nameof(StartBreak)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PostAsJsonAsync("api/Works/StartBreak", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Pomyślnie rozpoczęto przerwę. Smacznej kawusi!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas rozpoczynania przerwy."
                };
            }
        }

        public async Task<OperationResponse<bool>> StartWork(WorksRequest request)
        {
            _logger.LogInformation($"Method {nameof(StartWork)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PostAsJsonAsync("api/Works/StartWork", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Pomyślnie rozpoczęto pracę. Miłej pracy!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas rozpoczynania pracy."
                };
            }
        }

        public async Task<OperationResponse<bool>> EditWork(WorksRequest request)
        {
            _logger.LogInformation($"Method {nameof(EditWork)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PutAsJsonAsync("api/Works/EditWorkTime", request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Pomyślnie zedytowano twoją pracę"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas edytowania pracy."
                };
            }
        }

        public async Task<OperationResponse<IEnumerable<WorksResponse>>> GetWorkForMonth(long userID, DateTime month)
        {
            _logger.LogInformation($"Method {nameof(GetWorkForMonth)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                string uri = $"api/Works/GetWorkTimeForMonth/{userID}/{month.ToString("yyyy-MM-dd")}";
                var apiResponse = await httpClient.GetAsync(uri);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<IEnumerable<WorksResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var work = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<WorksResponse>>(_serializerOptions);

                return new OperationResponse<IEnumerable<WorksResponse>>
                {
                    Success = true,
                    Data = work,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<IEnumerable<WorksResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania czasów pracy na miesiąc."
                };
            }
        }

        public async Task<OperationResponse<bool>> RemoveWorkTime(WorksRequest work)
        {
            _logger.LogInformation($"Method {nameof(RemoveWorkTime)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                HttpRequestMessage request = new()
                {
                    Content = new StringContent(JsonSerializer.Serialize(work), encoding: Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(httpClient.BaseAddress + "api/Works/RemoveWorkTime")
                };
                var apiResponse = await httpClient.SendAsync(request);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Message = "Pomyślnie usunięto godziny pracy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas usuwania pracy."
                };
            }
        }

        public async Task<OperationResponse<IEnumerable<WorksResponse>>> GetWorksForTeamForDay(DateTime day, long teamID)
        {
            _logger.LogInformation($"Method {nameof(GetWorksForTeamForDay)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                string uri = $"api/Works/GetWorksForTeamForDay/{day.ToString("yyyy-MM-dd")}/{teamID}";
                var apiResponse = await httpClient.GetAsync(uri);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<IEnumerable<WorksResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var work = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<WorksResponse>>(_serializerOptions);

                return new OperationResponse<IEnumerable<WorksResponse>>
                {
                    Success = true,
                    Data = work,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<IEnumerable<WorksResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania czasów pracy dla danego zespołu."
                };
            }
        }

        public async Task<OperationResponse<bool>> ChangeWorkStatus(long userID, DateTime date)
        {
            _logger.LogInformation($"Method {nameof(ChangeWorkStatus)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var token = await _sessionStorage.GetItemAsync<string>("token");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var apiResponse = await httpClient.PutAsync($"api/Works/ChangeWorkStatus/{userID}/{date.ToString("yyyy-MM-dd")}", null);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"{errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Pomyślnie zmieniono status pracy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zmiany statusu pracy."
                };
            }
        }

    }
}
