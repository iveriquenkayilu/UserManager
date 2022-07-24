﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models.Service;
using System.Threading.Tasks;

namespace UserManagerService.Api.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IApiService _apiService;
        public ServiceController(IUserContext userContext, ILogger<ServiceController> logger, IApiService apiService) : base(userContext, logger)
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