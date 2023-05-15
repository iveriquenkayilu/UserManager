using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Models;
using UserManagerService.Models.Home;
using UserManagerService.Shared.Interfaces.Services;

namespace UserManagerService.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserContext _userContext;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, IUserContext userContext)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userContext = userContext;
        }

        public IActionResult Index() => View();

        [AllowAnonymous]
        [HttpGet("/signIn")]
        [HttpGet("/auth")]
        public IActionResult Auth([FromQuery] AuthInputModel input)
        {
            if (User.Identity.IsAuthenticated && _userContext.UserId == input.UserId && _userContext.CompanyId == input.CompanyId)
            {
                _logger.LogInformation($"User {_userContext.UserId} is already authenticated");

                if (Url.IsLocalUrl(input.ReturnUrl))
                {
                    _logger.LogInformation($"Redirecting to {input.ReturnUrl}");
                    return Redirect(input.ReturnUrl);
                }
                else
                {
                    _logger.LogCritical($"Url {input.ReturnUrl} is not a local url");
                    return BadRequest(); // return Error View
                }
            }
            return View(input);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => User.Identity.IsAuthenticated ? RedirectToAction(nameof(Index)) : View();

        [AllowAnonymous]
        [HttpGet("/login")]
        public IActionResult Login([FromQuery] LoginViewModel input) => View(input);

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Home");
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message });
        }
    }
}
