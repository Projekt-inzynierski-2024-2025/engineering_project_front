using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class WorksService : IWorksService
    {
        private readonly ILogger<WorksService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        public WorksService(ILogger<WorksService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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
                _logger.LogError(ex, "An error occurred while starting work.");
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
                _logger.LogError(ex, "An error occurred while starting work.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas edytowania pracy."
                };
            }
        }
    }
}
