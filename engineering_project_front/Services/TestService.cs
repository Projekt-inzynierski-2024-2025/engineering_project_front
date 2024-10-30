using engineering_project_front.Services.Interfaces;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class TestService : ITestService
    {
        private readonly ILogger<TestService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        public TestService(ILogger<TestService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task TestGet()
        {
            _logger.LogInformation($"Method {nameof(TestGet)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync("api/Users/GetUsers");

                if (apiResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return;
                }

                var token = await apiResponse.Content.ReadAsStringAsync();

                _logger.LogInformation(token);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
