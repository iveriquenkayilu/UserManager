using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Constants;
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Models;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.Roles;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAuthHelper _auth;
        public UserController(IUnitOfWork unitOfWork, SignInManager<User> signInManager, IUserContext userContext, IAuthHelper auth, ILogger<UserController> logger, IUserService userService, IRoleService roleService) : base(userContext, logger)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _auth = auth;
            _userService = userService;
            _roleService = roleService;
        }

        //TODO extract logic to the service
        [HttpGet]
        public async Task<IActionResult> GetCompanyUsers()
        {
            var users = await _unitOfWork.Query<CompanyUser>(u => u.CompanyId == _userContext.CompanyId).Include(o => o.User)
                .Select(u => new UserModel
                {
                    Id = u.User.Id,
                    CreatedAt = u.User.CreatedAt,
                    IsConnected = u.User.IsConnected,
                    Name = u.User.Name,
                    Surname = u.User.Surname,
                    UpdatedAt = u.User.UpdatedAt,
                    Username = u.User.UserName
                }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = RoleConstants.ADMIN)]
        [HttpGet("admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return CustomResponse.Success("Users fetched successfully", users);
        }

        [AllowAnonymous]
        [HttpPost("/api/v2/login")] // Company Login
        public async Task<IActionResult> Login([FromBody] LoginToCompanyInputModel input)
        {
            var output = await _userService.GetAuthTokenAsync(input);

            if (output.Companies is not null)
                return CustomResponse.Success(ResponseMessages.UserAuthenticated, output.Companies);

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
            _logger.LogInformation($"User is trying to refresh access token with refresh token : {input.RefreshToken}");

            var cachedToken = _auth.GetCachedRefreshTokenWithRequestIpValidation(input.RefreshToken);
            if (cachedToken is null)
                return Ok(ResponseModel.Fail(ResponseMessages.InvalidRefreshToken));

            if (!_auth.RevokeCachedRefreshToken(input.RefreshToken))
                return Ok(ResponseModel.Fail(ResponseMessages.RefreshTokenFailed));

            //var user = await _userRepository.GetUserByIdAsync();

            var user = await _signInManager.UserManager.FindByIdAsync(cachedToken.UserId.ToString());
            if (user is null)
                return Ok(ResponseModel.Fail(ResponseMessages.UserNotFound));

            var roles = (List<string>)await _signInManager.UserManager.GetRolesAsync(user); // TODO get roles with company always, Make role:companyId
            var company = await _unitOfWork.Query<CompanyUser>(o => o.UserId == user.Id && o.CompanyId == input.CompanyId)
                    .Include(o => o.Company).Select(o => new CompanyShortModel
                    {
                        Id = o.Company.Id,
                        Name = o.Company.Name
                    }).SingleOrDefaultAsync();

            if (company is null)
                throw new CustomException(ResponseMessages.RefreshTokenFailed);

            var tokens = _auth.CreateSecurityToken(user.Id, user.UserName, roles, company);

            var result = tokens != null
                ? ResponseModel.Success(ResponseMessages.TokensRefreshed, tokens)
                : ResponseModel.Fail(ResponseMessages.RefreshTokenFailed);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel input)
        {
            _logger.LogInformation($"User with id: {_userContext.UserId} is creating a user with username {input.Username}");
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.Username))
            {
                _logger.LogInformation("Password or username is null or empty");
                return Ok(ResponseModel.Fail(ResponseMessages.WrongCredentials));
            }

            // TODO check if org exists

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

                if (_userContext.IsUserAdmin() && input.CompanyId != Guid.Empty)
                {
                    var orgUser = new CompanyUser()
                    {
                        CompanyId = input.CompanyId == Guid.Empty ? _userContext.CompanyId : input.CompanyId,
                        UserId = user.Id,
                        CreatorId = _userContext.UserId,
                    };
                    await _unitOfWork.AddAsync(orgUser);
                    await _unitOfWork.SaveAsync();
                    input.CompanyId = orgUser.CompanyId;
                }

                var model = new UserProfile
                {
                    Id = user.Id,
                    Name = user.Name,
                    Username = user.UserName,
                    Surname = user.Surname,
                    Email = user.Email
                };
                return Ok(ResponseModel.Success(ResponseMessages.UserCreated, model));
            }
            else
            {
                string errors = "";
                ir.Errors.ToList().ForEach(e => { errors += $"{e.Code} {e.Description},"; });
                _logger.LogError($"Failed to create user: {errors}");
                return Ok(ResponseModel.Fail(ResponseMessages.FailedToCreatUser));
            }
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
        public async Task<IActionResult> Me() => Ok(await _userService.GetMyProfileAsync(_userContext.UserId));

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