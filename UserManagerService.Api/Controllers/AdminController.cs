using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagerService.Shared.Constants;

namespace UserManagerService.Api.Controllers
{
    [Authorize(Roles = RoleConstants.ADMIN)]
    public class AdminController : Controller
    {
        public IActionResult Users() => View();

        public IActionResult Roles() => View();
    }
}
