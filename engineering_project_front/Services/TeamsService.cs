using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace engineering_project_front.Services
{
    public class TeamsService : ITeamsService
    {


        private readonly ILogger<TeamsService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;
        public TeamsService(ILogger<TeamsService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        public async Task<List<TeamsResponse>> GetTeamsAsync()
        {
            _logger.LogInformation($"Method {nameof(GetTeamsAsync)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync("api/Teams/getTeams");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return new List<TeamsResponse>();
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var teams = JsonSerializer.Deserialize<List<TeamsResponse>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (teams == null)
                {
                    _logger.LogWarning("Deserialized teams list is null.");
                    return new List<TeamsResponse>();
                }

                _logger.LogInformation($"Retrieved {teams.Count} teams from API.");
                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching teams from API.");
                return new List<TeamsResponse>();
            }
        }


        public async Task<TeamsResponse> GetTeam(long ID)
        {
            _logger.LogInformation($"Method {nameof(GetTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Teams/getTeam/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return new TeamsResponse();
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var team = JsonSerializer.Deserialize<TeamsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (team == null)
                {
                    _logger.LogWarning("Deserialized team is null.");
                    return new TeamsResponse();
                }

                _logger.LogInformation($"Retrieved {team} team from API.");
                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching team from API.");
                return new TeamsResponse();
            }
        }

        public async Task<bool> AddTeam(TeamRequest team)
        {
            _logger.LogInformation($"Method {nameof(AddTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/Teams/addTeam", team);

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
        public async Task<bool> EditTeam(TeamRequest team)
        {
            _logger.LogInformation($"Method {nameof(EditTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PutAsJsonAsync("api/Teams/updateTeam", team);

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

        public async Task<bool> DeleteTeam(long ID)
        {
            _logger.LogInformation($"Method {nameof(DeleteTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.DeleteAsync($"api/Teams/delateTeam/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status code was not OK, it was {apiResponse.StatusCode}");
                    return false;
                }

                _logger.LogInformation($"Team with ID {ID} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting team from API.");
                return false;
            }
        }


    }
}
