using Microsoft.AspNetCore.Mvc;

namespace UserManagerService.Api.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
