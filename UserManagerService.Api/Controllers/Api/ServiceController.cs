using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models.Service;

namespace UserManagerService.Api.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IApiService _apiService;
        public ServiceController(ILogger<ServiceController> logger, IApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        [AllowAnonymous]
        [HttpPost("apiKey")]
        public async Task<IActionResult> GetApiKey([FromBody] ApiKeyRequestModel input)
        {
            var apiKey = await _apiService.GetApiKeyAsync(input);
            return Ok(apiKey);
        }
    }
}
