
namespace engineering_project_front.Services.Interfaces
{
    public interface IValidateRole
    {
        Task<bool> IsAuthorized(params string[] authorizedRoles);
    }
}
