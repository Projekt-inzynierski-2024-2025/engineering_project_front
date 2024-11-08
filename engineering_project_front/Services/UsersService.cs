using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<UsersService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public UsersService(ILogger<UsersService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<UsersResponse> GetUserFromToken(string token)
        {
            _logger.LogInformation($"Method {nameof(GetUserFromToken)} entered");
            try
            {
                UsersResponse? result = new();

                var httpClient = _httpClientFactory.CreateClient("engineering-project");

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/Login/GetUserFromToken");
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiResponse = await httpClient.SendAsync(requestMessage);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Status code was unsuccessful: {}", apiResponse.StatusCode);
                    return result;
                }

                var responseBody = await apiResponse.Content.ReadAsStreamAsync();

                result = await JsonSerializer.DeserializeAsync<UsersResponse>(responseBody, jsonSerializerOptions);

                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in method {nameof(GetUserFromToken)}: {ex.Message}");
                throw;
            }
        }
    }
}
