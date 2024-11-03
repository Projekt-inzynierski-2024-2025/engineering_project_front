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

        public LoginService(ILogger<LoginService> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
                    return string.Empty;

                var token = await apiResponse.Content.ReadAsStringAsync();

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in method {nameof(Login)}: {ex.Message}");
                throw;
            }
        }

        public string HashPassword(string password)
        {
            _logger.Log(LogLevel.Information, $"Method {nameof(HashPassword)} entered");

            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hashBytes = System.Security.Cryptography.SHA256.HashData(inputBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
