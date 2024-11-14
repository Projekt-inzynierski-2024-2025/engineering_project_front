using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class AvailabilitiesService :IAvailabilitiesService
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
        public async Task<OperationResponse<AvailabilitiesResponse>> GetAvailabilitiesForMonth(long userID, DateTime month)
        {
            _logger.LogInformation($"Method {nameof(GetAvailabilitiesForMonth)} entered");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Works/GetAvailabilitiesForMonth/{userID}/{month}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<AvailabilitiesResponse>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                    };
                }

                var availabilities = await apiResponse.Content.ReadFromJsonAsync<AvailabilitiesResponse>(_serializerOptions);

                return new OperationResponse<AvailabilitiesResponse>
                {
                    Success = true,
                    Data = availabilities,
                    Message = "Pomyślnie zedytowano twoją pracę"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting availabilities for month.");
                return new OperationResponse<AvailabilitiesResponse>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas zdobywania dyspozycji do pracy."
                };
            }
        }
    }
}
