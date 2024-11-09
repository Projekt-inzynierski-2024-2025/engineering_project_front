
﻿using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

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

    public async Task<OperationResponse<List<UsersResponse>>> GetUsersAsync()
    {
        _logger.LogInformation($"Method {nameof(GetUsersAsync)} entered");
        try
        {
            var httpClient = _httpClientFactory.CreateClient("engineering-project");
            var apiResponse = await httpClient.GetAsync("api/Users/GetUsers");

            if (!apiResponse.IsSuccessStatusCode)
            {
                var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                return new OperationResponse<List<UsersResponse>>
                {
                    Success = false,
                    Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                };
            }

            var users = await apiResponse.Content.ReadFromJsonAsync<List<UsersResponse>>(_serializerOptions);
            return new OperationResponse<List<UsersResponse>>
            {
                Success = true,
                Data = users,
                Message = $"Pobrano {users?.Count ?? 0} użytkowników."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching users from API.");
            return new OperationResponse<List<UsersResponse>>
            {
                Success = false,
                Message = "Wystąpił błąd podczas pobierania użytkowników."
            };
        }
    }

    public async Task<OperationResponse<UsersResponse>> GetUser(long ID)
    {
        _logger.LogInformation($"Method {nameof(GetUser)} entered");
        try
        {
            var httpClient = _httpClientFactory.CreateClient("engineering-project");
            var apiResponse = await httpClient.GetAsync($"api/Users/GetUser/{ID}");

            if (!apiResponse.IsSuccessStatusCode)
            {
                var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                return new OperationResponse<UsersResponse>
                {
                    Success = false,
                    Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                };
            }

            var user = await apiResponse.Content.ReadFromJsonAsync<UsersResponse>(_serializerOptions);
            return new OperationResponse<UsersResponse>
            {
                Success = true,
                Data = user,
                Message = "Użytkownik pobrany pomyślnie."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching user.");
            return new OperationResponse<UsersResponse>
            {
                Success = false,
                Message = "Wystąpił błąd podczas pobierania użytkownika."
            };
        }
    }

    public async Task<OperationResponse<bool>> AddUser(UserRequest user)
    {
        _logger.LogInformation($"Method {nameof(AddUser)} entered");
        try
        {
            var httpClient = _httpClientFactory.CreateClient("engineering-project");
            var apiResponse = await httpClient.PostAsJsonAsync("api/Users/addUser", user);

            if (!apiResponse.IsSuccessStatusCode)
            {
                var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                return new OperationResponse<bool>
                {
                    Success = false,
                    Message = $"Błąd {apiResponse.StatusCode}:{errorMessage}"
                };
            }

            return new OperationResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Użytkownik dodany pomyślnie."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding user.");
            return new OperationResponse<bool>
            {
                Success = false,
                Message = "Wystąpił błąd podczas dodawania użytkownika."
            };
        }
    }

    public async Task<OperationResponse<bool>> EditUser(UserRequest user)
    {
        _logger.LogInformation($"Method {nameof(EditUser)} entered");
        try
        {
            var httpClient = _httpClientFactory.CreateClient("engineering-project");
            var apiResponse = await httpClient.PutAsJsonAsync("api/Users/updateUser", user);

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
                Message = "Użytkownik edytowany pomyślnie."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while editing user.");
            return new OperationResponse<bool>
            {
                Success = false,
                Message = "Wystąpił błąd podczas edytowania użytkownika."
            };
        }
    }

    public async Task<OperationResponse<List<UsersResponse>>> GetManagers()
    {
        _logger.LogInformation($"Method {nameof(GetManagers)} entered");
        try
        {
            var httpClient = _httpClientFactory.CreateClient("engineering-project");
            var apiResponse = await httpClient.GetAsync("api/Users/GetManagers");

            if (!apiResponse.IsSuccessStatusCode)
            {
                var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                return new OperationResponse<List<UsersResponse>>
                {
                    Success = false,
                    Message = $"Błąd {apiResponse.StatusCode}: {errorMessage}"
                };
            }

            var managers = await apiResponse.Content.ReadFromJsonAsync<List<UsersResponse>>(_serializerOptions);
            return new OperationResponse<List<UsersResponse>>
            {
                Success = true,
                Data = managers,
                Message = $"Pobrano {managers?.Count ?? 0} kierowników."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching managers.");
            return new OperationResponse<List<UsersResponse>>
            {
                Success = false,
                Message = "Wystąpił błąd podczas pobierania kierowników."
            };
        }
    }

    public async Task<OperationResponse<bool>> DeleteUser(long ID)
    {
        _logger.LogInformation($"Method {nameof(DeleteUser)} entered");
        try
        {
            var httpClient = _httpClientFactory.CreateClient("engineering-project");
            var apiResponse = await httpClient.DeleteAsync($"api/Users/deleteUser/{ID}");

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
                Message = $"Użytkownik usunięty pomyślnie."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting user.");
            return new OperationResponse<bool>
            {
                Success = false,
                Message = "Wystąpił błąd podczas usuwania użytkownika."
            };

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

                result = await JsonSerializer.DeserializeAsync<UsersResponse>(responseBody, _serializerOptions);

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
