using Microsoft.AspNetCore.Mvc;
using System;
using UserManagerService.Api.Models.Account;

namespace UserManagerService.Api.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Profile() => View(nameof(Index));

        [HttpGet("/Account/Edit/{userId}")]
        public IActionResult Edit(Guid userId) => View(new EditAccountModel { UserId = userId });
    }
}