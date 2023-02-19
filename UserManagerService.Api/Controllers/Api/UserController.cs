using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Constants;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Models;
using UserManagerService.Shared.Models.Roles;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAuthHelper _auth;
        public UserController(ICompanyService companyService, SignInManager<User> signInManager, IAuthHelper auth, ILogger<UserController> logger, IUserService userService, IRoleService roleService) : base(logger)
        {
            _companyService = companyService;
            _signInManager = signInManager;
            _auth = auth;
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyUsers()
        {
            var users = await _companyService.GetCompanyUsersAsync();
            return Ok(users);
            // return CustomResponse.Success("Users fetched successfully", users);
        }

        [Authorize(Roles = RoleConstants.ADMIN)]
        [HttpGet("admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return CustomResponse.Success("Users fetched successfully", users);
        }

        [AllowAnonymous]
        [HttpPost("/api/v4/login")]
        public async Task<IActionResult> LoginWithSession([FromBody] LoginInputWithSession input)
        {
            var output = await _userService.GetAuthTokenWithSessionIdAsync(input);
            var result = (string.IsNullOrEmpty(output.AccessToken))
                    ? ResponseModel.Fail(ResponseMessages.AuthenticationFailed)
                    : ResponseModel.Success(ResponseMessages.UserAuthenticated, output);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("/api/v3/login")] // Get SessionId to redirect
        public async Task<IActionResult> LoginToRedirect([FromBody] LoginToCompanyInputModel input, string returnUrl)
        {
            var output = await _userService.GetAuthSessionToRedirectAsync(input, returnUrl);

            if (output.SessionId == Guid.Empty)
                return CustomResponse.Fail(ResponseMessages.AuthenticationFailed);
            else
                return CustomResponse.Success(ResponseMessages.UserAuthenticated, output);

        }

        [AllowAnonymous]
        [HttpPost("/api/v2/login")] // Company Login
        public async Task<IActionResult> Login([FromBody] LoginToCompanyInputModel input)
        {
            var output = await _userService.GetAuthTokenAsync(input);

            if (output.Companies is not null)
                return CustomResponse.Success(ResponseMessages.UserAuthenticated, new { output.Companies, output.SessionId });

            if (string.IsNullOrEmpty(output.AuthTokens.AccessToken) || string.IsNullOrEmpty(output.AuthTokens.RefreshToken))
                return CustomResponse.Fail(ResponseMessages.AuthenticationFailed);
            else
                return CustomResponse.Success(ResponseMessages.UserAuthenticated, output.AuthTokens);

        }

        [AllowAnonymous]
        [HttpPost("/api/v1/login")] // Simple
        public async Task<IActionResult> Login([FromBody] LoginInputModel input)
        {
            var output = await _userService.GetAuthTokenAsync(input);
            var result = (string.IsNullOrEmpty(output.AccessToken))
                    ? ResponseModel.Fail(ResponseMessages.AuthenticationFailed)
                    : ResponseModel.Success(ResponseMessages.UserAuthenticated, output);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("/api/refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenInput input)
        {
            var tokens = await _userService.RefreshTokenAsync(input);

            return tokens != null
                 ? CustomResponse.Success(ResponseMessages.TokensRefreshed, tokens)
                 : CustomResponse.Fail(ResponseMessages.RefreshTokenFailed);
        }

        [AllowAnonymous]
        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel input)
        {
            var model = await _userService.RegisterUserAsync(input);
            return CustomResponse.Success(ResponseMessages.UserCreated, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserInputModel input)
        {
            var model = await _userService.UpdateUserAsync(id, input);
            return CustomResponse.Success("User updated successfully", model);
        }

        [Authorize(Roles = RoleConstants.ADMIN)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return CustomResponse.Success("User deleted successfully");
        }

        [HttpGet("/api/me")]
        public async Task<IActionResult> Me() => Ok(await _userService.GetMyProfileAsync());

        [HttpGet("/api/sessions")]
        public async Task<IActionResult> LoginHistory([FromQuery] LoginSessionInputModel input)
        {
            var loginHistory = await _userService.GetLoginSessionsAsync(input);
            return CustomResponse.Success("Login history fetched successfully", loginHistory);
        }

        //[ApiKey]
        [AllowAnonymous]
        [HttpPost("profiles")]
        public async Task<IActionResult> Profiles([FromBody] GetUserProfilesModel input)
        {
            var profiles = await _userService.GetUserProfilesByIdsAsync(input.UserIds);
            return Ok(ResponseModel.Success(ResponseMessages.UserProfilesFetched, profiles));
        }

        [HttpPost("{UserId}/roles")]
        public async Task<IActionResult> AssignRoles([FromBody] UserRoleInputModel input)
        {
            var model = await _roleService.AssignRolesToUserAsync(input);
            return CustomResponse.Success("Roles assigned to user successfully", model);
        }

        [HttpDelete("{UserId}/roles")]
        public async Task<IActionResult> RemoveRoles([FromBody] UserRoleInputModel input)
        {
            await _roleService.RemoveRolesFromUserAsync(input);
            return CustomResponse.Success("Roles removed from user successfully");
        }
    }
}