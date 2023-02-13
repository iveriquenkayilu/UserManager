using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Models;
using UserManagerService.Models.Home;

namespace UserManagerService.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }
    
        public IActionResult Index() => View();

        [AllowAnonymous]
        [HttpGet("/signIn")]
        [HttpGet("/auth")]
        public IActionResult Auth([FromQuery] AuthInputModel input)
        {
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
