using UserManagerService.Shared.Models.Service;
using System.Threading.Tasks;

namespace UserManagerService.Shared.Interfaces.Services
{
    public interface IApiService
    {
        Task<ServiceApiKeyModel> GetApiKeyAsync(ApiKeyRequestModel input);
    }
}