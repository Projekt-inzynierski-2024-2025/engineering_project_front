using engineering_project_front.Models.Responses;
using engineering_project_front.Models.Request;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace engineering_project_front.Services
{
    public class AvailabilitiesService : IAvailabilitiesService
    {
        private readonly ILogger<AvailabilitiesService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        public AvailabilitiesService(ILogger<AvailabilitiesService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<OperationResponse<bool>> CreateAvailabilities(AvailabilitiesRequest request)
        {
            _logger.LogInformation($"Method {nameof(CreateAvailabilities)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/Availabilities/CreateAvailability", request);

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
                    Message = "Pomyślnie zapisano dostępność do pracy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zapisywania dostępności pracy."
                };
            }
        }

        public async Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForDay(long userID, DateTime day)
        {
            _logger.LogInformation($"Method {nameof(GetAvailabilitiesForDay)} entered");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Availabilities/GetAvailabilitiesForDay/{userID}/{day.ToString("yyyy-MM-dd")}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var availabilities = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<AvailabilitiesResponse>>(_serializerOptions);

                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = true,
                    Data = availabilities,
                    Message = "Pomyślnie zedytowano twoją pracę"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting availabilities for month.");
                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zdobywania dyspozycji do pracy."
                };
            }
        }

        public async Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForDay(DateTime day)
        {
            _logger.LogInformation($"Method {nameof(GetAvailabilitiesForDay)} entered");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Availabilities/GetAvailabilitiesForMonth/{day.ToString("yyyy-MM-dd")}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var availabilities = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<AvailabilitiesResponse>>(_serializerOptions);

                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = true,
                    Data = availabilities,
                    Message = "Pomyślnie zedytowano twoją pracę"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting availabilities for month.");
                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zdobywania dyspozycji do pracy."
                };
            }
        }

        public async Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForMonth(long userID, DateTime month)
        {
            _logger.LogInformation($"Method {nameof(GetAvailabilitiesForMonth)} entered");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Availabilities/GetAvailabilitiesForMonth/{userID}/{month.ToString("yyyy-MM-dd")}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var availabilities = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<AvailabilitiesResponse>>(_serializerOptions);

                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = true,
                    Data = availabilities,
                    Message = "Pomyślnie zedytowano twoją pracę"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting availabilities for month.");
                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zdobywania dyspozycji do pracy."
                };
            }
        }

        public async Task<OperationResponse<IEnumerable<AvailabilitiesResponse>>> GetAvailabilitiesForMonth(DateTime month)
        {
            _logger.LogInformation($"Method {nameof(GetAvailabilitiesForMonth)} entered");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Availabilities/GetAvailabilitiesForMonth/{month.ToString("yyyy-MM-dd")}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var availabilities = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<AvailabilitiesResponse>>(_serializerOptions);

                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = true,
                    Data = availabilities,
                    Message = "Pomyślnie zedytowano twoją pracę"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting availabilities for month.");
                return new OperationResponse<IEnumerable<AvailabilitiesResponse>>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zdobywania dyspozycji do pracy."
                };
            }
        }

        public async Task<OperationResponse<bool>> RemoveAvailability(AvailabilitiesRequest availability)
        {
            _logger.LogInformation($"Method {nameof(RemoveAvailability)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                HttpRequestMessage request = new()
                {
                    Content = new StringContent(JsonSerializer.Serialize(availability), encoding:Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(httpClient.BaseAddress + "api/Availabilities/RemoveAvailability")
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
                    Message = "Pomyślnie usunięto dostępność do pracy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing availability.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas usuwania dostępności pracy."
                };
            }
        }

        public async Task<OperationResponse<bool>> UpdateAvailability(AvailabilitiesRequest request)
        {
            _logger.LogInformation($"Method {nameof(UpdateAvailability)} entered.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PutAsJsonAsync("api/Availabilities/UpdateAvailability", request);

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
                    Message = "Pomyślnie zapisano dostępność do pracy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas aktualizacji dostępności pracy."
                };
            }
        }
    }
}
