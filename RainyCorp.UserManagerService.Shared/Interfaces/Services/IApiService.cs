using RainyCorp.UserManagerService.Shared.Models.Service;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService.Shared.Interfaces.Services
{
    public interface IApiService
    {
        Task<ServiceApiKeyModel> GetApiKeyAsync(ApiKeyRequestModel input);
    }
}