using Microsoft.AspNetCore.Mvc;

namespace UserManagerService.Controllers
{
    public class CompaniesController : Controller
    {
        public IActionResult Index() => View();
    }
}
