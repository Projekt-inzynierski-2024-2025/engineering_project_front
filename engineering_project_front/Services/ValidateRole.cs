using Blazored.SessionStorage;
using engineering_project_front.Services.Interfaces;

namespace engineering_project_front.Services
{
    public class ValidateRole : IValidateRole
    {
        private readonly ISessionStorageService _sessionStorage;

        public ValidateRole(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task<bool> IsAuthorized(params string[] authorizedRoles)
        {
            string yourRole = await _sessionStorage.GetItemAsStringAsync("role");

            if (string.IsNullOrEmpty(yourRole))
                return false;
            
            var result = authorizedRoles.Any(r => r.Equals(yourRole));

            return result;
        }
    }
}
