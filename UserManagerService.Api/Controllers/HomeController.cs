using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Models;

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
        [HttpGet]
        public IActionResult Login() => User.Identity.IsAuthenticated ? RedirectToAction(nameof(Index)) : View();

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        //public IActionResult Videos()
        //{
        //    var videos = new List<VideoModel> { new VideoModel { Url = "" } };
        //    var model = new VideosViewModel
        //    {
        //        Videos = videos
        //    };
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel input)
        //{
        //    if (input == null) return View();
        //    if (string.IsNullOrEmpty(input.Username)) return View();
        //    if (string.IsNullOrEmpty(input.Password)) return View();

        //    var result = await _signInManager.PasswordSignInAsync(input.Username, input.Password, true, false);
        //    if (result.Succeeded)
        //        return RedirectToAction(nameof(Index));
        //    else
        //        return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Home");
        }
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
