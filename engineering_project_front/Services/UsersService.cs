using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using System.ComponentModel;
using System.Net.Http.Json;
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

        public async Task<UsersResponse> GetUser(long ID)
        {
            _logger.LogInformation($"Method {nameof(GetUser)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Users/GetUser/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return new UsersResponse();
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UsersResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (user == null)
                {
                    _logger.LogWarning("Deserialized users list is null.");
                    return new UsersResponse();
                }

                _logger.LogInformation($"Retrieved {user} users from API.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from API.");
                return new UsersResponse();
            }
        }


        public async Task<bool> AddUser(UserRequest user)
        {
            _logger.LogInformation($"Method {nameof(AddUser)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/Users/addUser", user);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return false;
                }

                _logger.LogInformation($"OK");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from API.");
                return false;
            }
        }
        public async Task<bool> EditUser(UserRequest user)
        {           
            _logger.LogInformation($"Method {nameof(EditUser)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PutAsJsonAsync("api/Users/updateUser", user);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return false;
                }

                _logger.LogInformation($"OK");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from API.");
                return false;
            }
        }
        public async Task<List<UsersResponse>> GetMenegers()
        {
            _logger.LogInformation($"Method {nameof(GetMenegers)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync("api/Users/GetManagers");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return new List<UsersResponse>();
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var managers = JsonSerializer.Deserialize<List<UsersResponse>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (managers == null)
                {
                    _logger.LogWarning("Deserialized users list is null.");
                    return new List<UsersResponse>();
                }

                _logger.LogInformation($"Retrieved {managers.Count} users from API.");
                return managers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from API.");
                return new List<UsersResponse>();
            }
        }

        public async Task<bool> DeleteUser(long ID)
        {
            _logger.LogInformation($"Method {nameof(DeleteUser)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.DeleteAsync($"api/Users/deleteUser/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return false;
                }

                _logger.LogInformation($"User with ID {ID} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user from API.");
                return false;
            }
        }


    }
}
