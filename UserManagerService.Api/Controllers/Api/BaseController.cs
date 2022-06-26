using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagerService.Shared.Interfaces.Services;

namespace UserManagerService.Api.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IUserContext _userContext;
        protected readonly ILogger<BaseController> _logger;
        public BaseController(IUserContext userContext, ILogger<BaseController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }
    }
}