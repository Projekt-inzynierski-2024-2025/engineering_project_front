using Blazored.SessionStorage;
using engineering_project_front.Models.Parameters;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILogger<LoginService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ISessionStorageService _sessionStorage;
        private readonly IUsersService _usersService;

        public LoginService(ILogger<LoginService> logger, IHttpClientFactory httpClientFactory , IUsersService usersService, ISessionStorageService sessionStorage)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _usersService = usersService;
            _sessionStorage = sessionStorage;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<string> Login(LoginParameters login)
        {
            _logger.Log(LogLevel.Information, $"Method {nameof(Login)} entered");
            try
            {
                login.Password = HashPassword(login.Password!);

                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/Login", login);

                if (apiResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Status code was {apiResponse.StatusCode}: {apiResponse.Content}");
                    return string.Empty;

                }

                var token = await apiResponse.Content.ReadAsStringAsync();

                var usersResponse = await _usersService.GetUserFromToken(token);

                await _sessionStorage.SetItemAsStringAsync("token", token);

                await _sessionStorage.SetItemAsStringAsync("role", usersResponse.RoleName);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in method {nameof(Login)}: {ex.Message}");
                throw;
            }
        }

        private string HashPassword(string password)
        {
            _logger.Log(LogLevel.Information, $"Method {nameof(HashPassword)} entered");

            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hashBytes = System.Security.Cryptography.SHA256.HashData(inputBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
