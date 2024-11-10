using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
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


        public async Task<OperationResponse<List<TeamsResponse>>> GetTeamsAsync()
        {
            _logger.LogInformation("Fetching teams from API.");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync("api/Teams/getTeams");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<List<TeamsResponse>>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };
                  
                }

                var teams = await apiResponse.Content.ReadFromJsonAsync<List<TeamsResponse>>(_serializerOptions);
                return new OperationResponse<List<TeamsResponse>>
                {
                    Success = true,
                    Data = teams,
                    Message = $"Pobrano {teams?.Count} zespołów."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams.");
                return new OperationResponse<List<TeamsResponse>>
                {
                    Success = false,
                    Message = "wystąpił błąd podczas pobierania zespołów"
                };
            }
        }


        public async Task<OperationResponse<TeamsResponse>> GetTeam(long ID)
        {
            _logger.LogInformation($"Method {nameof(GetTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Teams/getTeam/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<TeamsResponse>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };
                   
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var team = JsonSerializer.Deserialize<TeamsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (team == null)
                {
                    return new OperationResponse<TeamsResponse>
                    {
                        Success = false,
                        Message = "Brak zespołu"
                    };
                }

                return new OperationResponse<TeamsResponse>
                {
                    Success = true,
                    Data = team,
                    Message = "Udało się pobrać zespół"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching team from API.");
                return new OperationResponse<TeamsResponse>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania zepołu"
                };
            }
        }

        public async Task<OperationResponse<bool>> AddTeam(TeamRequest team)
        {
            _logger.LogInformation($"Method {nameof(AddTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PostAsJsonAsync("api/Teams/addTeam", team);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Udało się dodać zespoł"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding team.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił problem podczas dodawania zespołu"
                };
            }
        }
        public async Task<OperationResponse<bool>> EditTeam(TeamRequest team)
        {
            _logger.LogInformation($"Method {nameof(EditTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.PutAsJsonAsync("api/Teams/updateTeam", team);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Bład {apiResponse.StatusCode}: {errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Zaktualizowano zespoł"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing team.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wystąpił bład podczas edycji zespołu"
                };
            }
        }

        public async Task<OperationResponse<bool>> DeleteTeam(long ID)
        {
            _logger.LogInformation($"Method {nameof(DeleteTeam)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.DeleteAsync($"/api/Teams/deleteTeam/{ID}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<bool>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };
                }

                return new OperationResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = $"Udało się usunąć zespoł"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting team.");
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = "Wsytąpił błąd podczas usówania zespołu"
                };
            }
        }

        public async Task<OperationResponse<long>> GetTeamIDForManager(string managerEmail)
        {
            _logger.LogInformation($"Method {nameof(GetTeamIDForManager)} entered");
            try
            {
                var httpClient = _httpClientFactory.CreateClient("engineering-project");
                var apiResponse = await httpClient.GetAsync($"api/Teams/getTeamIDForManager/{managerEmail}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    return new OperationResponse<long>
                    {
                        Success = false,
                        Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                    };
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                var teamID = JsonSerializer.Deserialize<long>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new OperationResponse<long>
                {
                    Success = true,
                    Data = teamID,
                    Message = "Udało się pobrać ID zespołu"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching team ID for manager from API.");
                return new OperationResponse<long>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas pobierania ID zespołu"
                };
            }

        }
    }
}
