using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class UsersService : IUsersService
    {

        private readonly ILogger<UsersService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        public UsersService(ILogger<UsersService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        public async Task<List<UsersResponse>> GetUsersAsync()
        {
            _logger.LogInformation($"Method {nameof(GetUsersAsync)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync("api/Users/GetUsers");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return new List<UsersResponse>();
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<UsersResponse>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (users == null)
                {
                    _logger.LogWarning("Deserialized users list is null.");
                    return new List<UsersResponse>();
                }

                _logger.LogInformation($"Retrieved {users.Count} users from API.");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from API.");
                return new List<UsersResponse>(); 
            }
        }




    }
}
