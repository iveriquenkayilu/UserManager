using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RainyCorp.UserManagerService.Shared.Interfaces.Services;

namespace RainyCorp.UserManagerService.Api.Controllers
{
    //[Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IUserContext _userContext;
        private readonly ILogger<BaseController> _logger;
        public BaseController(IUserContext userContext, ILogger<BaseController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }
    }
}