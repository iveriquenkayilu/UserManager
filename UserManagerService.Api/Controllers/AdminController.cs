using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagerService.Shared.Constants;

namespace UserManagerService.Api.Controllers
{
    [Authorize(Roles = Roles.ADMIN)]
    public class AdminController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }
    }
}
