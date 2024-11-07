using engineering_project.Models.Parameters;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class ResetPassword : IResetPassword
    {
        private readonly ILogger<ResetPassword> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        public ResetPassword(ILogger<ResetPassword> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public bool ChangePassword(ResetPasswordParameters parameters, string? token = null)
        {
            _logger.LogInformation($"Method {nameof(ChangePassword)} entered");
            try
            {
                HttpResponseMessage apiResponse;
                if (parameters.Code is null)
                {
                    var httpClient = _httpClientFactory.CreateClient("engineering-project");

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/PasswordReset/ChangePassword");
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    apiResponse = httpClient.SendAsync(requestMessage).Result;
                }
                else
                {
                    var httpClient = _httpClientFactory.CreateClient("engineering-project");
                    apiResponse = httpClient.PostAsJsonAsync("api/PasswordReset/ResetPassword", parameters).Result;
                }

                if (apiResponse.IsSuccessStatusCode)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in method {nameof(ChangePassword)}: {ex.Message}");
                throw;
            }
        }

        public async Task SendAskForResetCode(string mail)
        {
            _logger.LogInformation($"Method {nameof(SendAskForResetCode)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/PasswordReset/SendCode", mail);

                _logger.LogInformation($"Api responded with code {apiResponse.StatusCode}: {apiResponse.Content}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in method {nameof(SendAskForResetCode)}: {ex.Message}");
                throw;
            }
        }
    }
}
