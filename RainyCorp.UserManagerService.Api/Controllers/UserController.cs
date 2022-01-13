using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RainyCorp.UserManagerService.Entities;
using RainyCorp.UserManagerService.Shared.Constants;
using RainyCorp.UserManagerService.Shared.Interfaces.Services;
using RainyCorp.UserManagerService.Shared.Interfaces.Shared;
using RainyCorp.UserManagerService.Shared.Models;
using RainyCorp.UserManagerService.Shared.Models.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserContext _userContext;
        private readonly IAuth _auth;
        private readonly ILogger<UserController> _logger;
        public UserController(SignInManager<User> signInManager, IUserContext userContext, IAuth auth, ILogger<UserController> logger)
        {
            _signInManager = signInManager;
            _userContext = userContext;
            _auth = auth;
            _logger = logger;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok();
        }

        [HttpGet("/test")]
        public IActionResult Test() => Ok();

        [AllowAnonymous]
        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody] LoginModel input)
        {
            _logger.LogInformation($"User {_userContext.Username} is getting the token");
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
                return Ok(ResponseModel.Fail(ResponseMessages.InvalidInput));

            var tokens = new AuthTokenModel();
            var user = await _signInManager.UserManager.FindByNameAsync(input.Username);
            if (user is null)
                return Ok(ResponseModel.Fail(ResponseMessages.WrongCredentials));

            var signedIn = await _signInManager.PasswordSignInAsync(user, input.Password, true, false);
            if (signedIn.Succeeded)
            {
                await _signInManager.SignOutAsync(); // remove the cookie
                var roles = (List<string>)await _signInManager.UserManager.GetRolesAsync(user);

                tokens = _auth.CreateSecurityToken(user.Id, user.UserName, roles.Select(r => r.ToUpper()).ToList());
            }
            var result = (string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrEmpty(tokens.RefreshToken))
                    ? ResponseModel.Fail(ResponseMessages.AuthenticationFailed)
                    : ResponseModel.Success(ResponseMessages.UserAuthenticated, tokens);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel input)
        {
            _logger.LogInformation($"User with id: {_userContext.UserId} is creating a user with username {input.Username}");
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.Username))
            {
                _logger.LogInformation("Password or username is null or empty");
                return Ok(ResponseModel.Fail(ResponseMessages.WrongCredentials));
            }

            var user = await _signInManager.UserManager.FindByNameAsync(input.Username);
            if (user != null)
                return Ok(ResponseModel.Fail(ResponseMessages.EmailExists));
            user = new User
            {
                Email = input.Email,
                NormalizedEmail = input.Email.ToUpper(),
                UserName = input.Username,
                NormalizedUserName = input.Email.ToUpper(),
                AccessFailedCount = 0,
                EmailConfirmed = true,
                Name = input.Name,
                Surname = input.Surname,
                LockoutEnabled = false
            };

            var ir = await _signInManager.UserManager.CreateAsync(user, input.Password);

            if (ir.Succeeded)
            {
                //var createdUser = await _userRepository.GetUserByUsernameAsync(input.Username);
                _logger.LogInformation($"Created user `{input.Username}` successfully");

                return Ok(ResponseModel.Success(ResponseMessages.UserCreated));
            }
            else
            {
                string errors = "";
                ir.Errors.ToList().ForEach(e => { errors += $"{e.Code} {e.Description},"; });
                _logger.LogError($"Failed to create user: {errors}");
                return Ok(ResponseModel.Fail(ResponseMessages.FailedToCreatUser));
            }
        }
    }
}