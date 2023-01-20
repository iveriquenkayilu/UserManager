﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Constants;
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Helpers;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ICompanyService _companyService;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthHelper _authHelper;
        public UserService(ICompanyService companyService, IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger, IAuthHelper authHelper, SignInManager<User> signInManager) : base(userContext, unitOfWork, mapper, logger)
        {
            _companyService = companyService;
            _authHelper = authHelper;
            _signInManager = signInManager;
        }

        public async Task<List<UserProfile>> GetUserProfilesByIdsAsync(List<Guid> ids)
        {
            Logger.LogInformation("Getting user profiles");

            if (ids is null || ids.Count == 0)
                return new List<UserProfile>(); // TODO use a custom exception

            ids = ids.Distinct().ToList();

            var profiles = await UnitOfWork.Query<CompanyUser>(u => ids.Contains(u.UserId))
                .Include(o => o.Company).Include(o => o.User)
                .Select(u => new UserProfile
                {
                    Id = u.User.Id,
                    Name = u.User.Name,
                    Surname = u.User.Surname,
                    Username = u.User.UserName,
                    Picture = u.User.Picture
                }).ToListAsync();

            return profiles;
        }

        public async Task<List<UserModel>> GetUsersAsync()
            => await UnitOfWork.Query<User>()
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    CreatedAt = u.CreatedAt,
                    IsConnected = u.IsConnected,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    UpdatedAt = u.UpdatedAt,
                    Username = u.UserName
                }).ToListAsync();

        public async Task<UserModel> GetAsync(Guid id, UserInputModel input) // with roles
        {
            var user = await UnitOfWork.Query<User>(u => u.Id == id).FirstOrDefaultAsync();

            return Mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> GetAsync(Guid id) // with roles
        {
            var user = await UnitOfWork.Query<User>(u => u.Id == id).FirstOrDefaultAsync();

            return Mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> UpdateUserAsync(Guid id, UserInputModel input)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to update user {id}");

            // TODO validation

            if (id != UserContext.UserId)
            {
                Logger.LogError($"User {UserContext.UserId} cannot update user {id} info");
                throw new CustomException(ResponseMessages.Unauthorized);
            }

            var user = await UnitOfWork.Query<User>(d => d.Id == id).FirstOrDefaultAsync();

            if (user is null)
                throw new CustomException($"User {id} not found");

            user.CreatorId = UserContext.UserId;
            user.UpdatedAt = DateTime.Now;
            user.Name = input.Name;
            user.UserName = input.Username;
            user.Surname = input.Surname;
            user.Email = input.Email;


            UnitOfWork.Update(user);
            await UnitOfWork.SaveAsync();

            return Mapper.Map<UserModel>(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to delete user {id}");
            await UnitOfWork.SoftDeleteEntityAsync<User>(id);
        }
        public async Task<User> GetEntityAsync(Guid id) => await UnitOfWork.GetAsync<User>(id);

        public async Task<MyProfile> GetMyProfileAsync()
        {
            var profile = await GetUserProfileAsync(UserContext.UserId);
            var myProfile = Mapper.Map<MyProfile>(profile);
            myProfile.Companies = await _companyService.GetUserCompaniesAsync(UserContext.UserId);
            return myProfile;
        }

        public async Task<UserProfile> GetUserProfileAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomException("User id cannot be empty");

            return await UnitOfWork.Query<User>(u => u.Id == id).Select(u => new UserProfile
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Username = u.UserName,
                Picture = u.Picture,
                Email = u.Email
            }).FirstOrDefaultAsync();
        }

        public async Task<AccessTokenModel> GetAuthTokenAsync(LoginInputModel input) //zero company
        {
            var user = await LoginAsync(input, Guid.Empty);
            return _authHelper.GetAccessToken(user.Id, user.UserName, null, null);
        }

        public async Task<LoginOutputModel> GetAuthTokenAsync(LoginToCompanyInputModel input) // 1 or multiple companies
        {
            var user = await LoginAsync(Mapper.Map<LoginInputModel>(input), input.CompanyId ?? Guid.Empty);

            var companies = await _companyService.GetUserCompaniesAsync(user.Id);

            if (companies.Count == 0)
            {
                if (input.CompanyId != null)
                {
                    Logger.LogInformation($"User {user.Id} does not belong to a company {input.CompanyId}");
                    throw new CustomException(ResponseMessages.AuthenticationFailed);
                }

                return new LoginOutputModel
                {
                    AuthTokens = _authHelper.CreateSecurityToken(user.Id, user.UserName, null, null)
                };
            }


            if (companies.Count == 1)
            {
                if (input.CompanyId != null && !companies.Any(c => c.Id == input.CompanyId))
                    throw new CustomException(ResponseMessages.AuthenticationFailed);

                var roles = (List<string>)await _signInManager.UserManager.GetRolesAsync(user);
                var tokens = _authHelper.CreateSecurityToken(user.Id, user.UserName, roles.Select(r => r.ToUpper()).ToList(), companies.First());
                return new LoginOutputModel
                {
                    AuthTokens = tokens
                };
            }

            return new LoginOutputModel
            {
                Companies = companies
            };
        }

        public async Task DeleteVisitorAsync(Guid id)
        {
            var visitor = await UnitOfWork.GetAsync<Visitor>(id);
            if (visitor == null) return;
            UnitOfWork.Delete<Visitor>(visitor);  // test if needs to save
        }

        public async Task<List<LoginSessionModel>> GetLoginSessionsAsync(LoginSessionInputModel input)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to get their login history");

            Expression<Func<LoginSession, bool>> predicate = l =>
            (input.StartDate != null && input.EndDate != null) ?
                l.CreatedAt <= input.EndDate && l.CreatedAt >= input.StartDate : true;

            return await UnitOfWork.Query<LoginSession>(l => l.CreatorId == UserContext.UserId)
                .Where(predicate)
                .Select(l => new LoginSessionModel
                {
                    CreatedAt = l.CreatedAt,
                    Device = l.Device,
                    IpAddress = l.IpAddress,
                    Location = l.Location,
                    Status = l.Status,
                    Id = l.Id
                }).ToListAsync();
        }

        /// <summary>
        /// Checks if the visitor exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> VistiorExists(Guid id) => await UnitOfWork.AnyAsync<Visitor>(v => v.Id == id);

        //public async Task AddRoles(long currentUserId, long userId, List<string> roles)
        //{
        //    var currentUser = await _userRepository.GetAsync(currentUserId);
        //    if (currentUser.Id != await GetAdminId()) throw new NotImplementedException(); // only admin can do this or a specific coach
        //    var user =await _userRepository.GetAsync(userId);
        //    _userRepository.AddRoles(user, roles);  // if doesn't work, go to repository, don't use userManager but the tables, roles
        //}

        public async Task<Guid> GetAdminId()
        {
            var user = await UnitOfWork.Query<User>(u => u.Email == Admin.Email).FirstOrDefaultAsync();
            return user.Id;
        }

        //public async Task<UserModel> GetByPhoneNumber(string phoneNumber) => _mapper.Map<UserModel>( await _userRepository.GetByPhoneNumber(phoneNumber));

        //public async Task<UserModel> GetByUserName(string userName) => _mapper.Map<UserModel>(await _userRepository.GetByUsername(userName));
        //public async Task<IdentityResult> RegisterAsync(RegisterModel form)
        //{
        //    if (form == null) throw new NotImplementedException();
        //    if (form.Password == null) throw new NotImplementedException();


        //    var user = _mapper.Map<User>(form);
        //    user.UserName = form.Email;

        //    var password = form.Password;
        //    return await _userRepository.AddAsync(user,form.Password);
        //}


        public async Task<Guid> AddVisitorAsync(VisitorModel input)
        {
            // TODO validation
            var visitor = Mapper.Map<Visitor>(input);
            await UnitOfWork.AddAsync<Visitor>(visitor);
            await UnitOfWork.SaveAsync();
            return visitor.Id;
        }

        private async Task<User> LoginAsync(LoginInputModel input, Guid companyId) // Can use the other model that has companyId, and remove companyId as parm
        {
            Logger.LogInformation($"User {input.Username} {input.Email} is getting the token");
            if ((string.IsNullOrEmpty(input.Username) && string.IsNullOrEmpty(input.Email)) || string.IsNullOrEmpty(input.Password))
                throw new CustomException(ResponseMessages.InvalidInput);

            var user = string.IsNullOrEmpty(input.Email) ?
                await _signInManager.UserManager.FindByNameAsync(input.Username) :
                  await _signInManager.UserManager.FindByEmailAsync(input.Email);
            if (user is null)
                throw new CustomException(ResponseMessages.WrongCredentials);

            var signedIn = await _signInManager.PasswordSignInAsync(user, input.Password, true, false);
            if (signedIn.Succeeded)
            {
                await _signInManager.SignOutAsync(); // remove the cookie

                var visitor = _authHelper.GetVisitorInfo();
                var location = await _authHelper.GetIpAddressLocation(visitor.AddressIp);

                var session = new LoginSession
                {
                    Device = visitor.Device,
                    IpAddress = visitor.AddressIp,
                    Status = "200",
                    Location = location.Location,
                    CreatorId = user.Id,
                    CompanyId = companyId
                };
                await UnitOfWork.AddAsync(session);
                await UnitOfWork.SaveAsync();
            }
            else
                throw new CustomException(ResponseMessages.AuthenticationFailed);

            return user;
        }

        public async Task<UserProfile> RegisterUserAsync(RegisterModel input)
        {
            Logger.LogInformation($"User is creating a user with username {input.Username}");
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.Username))
            {
                Logger.LogInformation("Password or username is null or empty");
                throw new CustomException(ResponseMessages.WrongCredentials);
            }

            // TODO check if org exists

            var user = await _signInManager.UserManager.FindByNameAsync(input.Username);
            if (user != null)
                throw new CustomException(ResponseMessages.EmailExists);
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
                Logger.LogInformation($"Created user `{input.Username}` successfully");

                if (UserContext.IsUserAdmin() && input.CompanyId != Guid.Empty)
                {
                    var orgUser = new CompanyUser()
                    {
                        CompanyId = input.CompanyId == Guid.Empty ? UserContext.CompanyId : input.CompanyId,
                        UserId = user.Id,
                        CreatorId = UserContext.UserId,
                    };
                    await UnitOfWork.AddAsync(orgUser);
                    await UnitOfWork.SaveAsync();
                    input.CompanyId = orgUser.CompanyId;
                }

                return new UserProfile
                {
                    Id = user.Id,
                    Name = user.Name,
                    Username = user.UserName,
                    Surname = user.Surname,
                    Email = user.Email
                };
            }
            else
            {
                string errors = "";
                ir.Errors.ToList().ForEach(e => { errors += $"{e.Code} {e.Description},"; });
                Logger.LogError($"Failed to create user: {errors}");
                throw new CustomException(ResponseMessages.FailedToCreatUser);
            }
        }

        public async Task<AuthTokenModel> RefreshTokenAsync(RefreshTokenInput input)
        {
            Logger.LogInformation($"User is trying to refresh access token with refresh token : {input.RefreshToken}");

            var cachedToken = _authHelper.GetCachedRefreshTokenWithRequestIpValidation(input.RefreshToken);
            if (cachedToken is null)
                throw new CustomException(ResponseMessages.InvalidRefreshToken);

            if (!_authHelper.RevokeCachedRefreshToken(input.RefreshToken))
                throw new CustomException(ResponseMessages.RefreshTokenFailed);

            //var user = await _userRepository.GetUserByIdAsync();

            var user = await _signInManager.UserManager.FindByIdAsync(cachedToken.UserId.ToString());
            if (user is null)
                throw new CustomException(ResponseMessages.UserNotFound);

            var roles = (List<string>)await _signInManager.UserManager.GetRolesAsync(user); // TODO get roles with company always, Make role:companyId
            var company = await UnitOfWork.Query<CompanyUser>(o => o.UserId == user.Id && o.CompanyId == input.CompanyId)
                    .Include(o => o.Company).Select(o => new CompanyShortModel
                    {
                        Id = o.Company.Id,
                        Name = o.Company.Name
                    }).SingleOrDefaultAsync();

            if (company is null)
                throw new CustomException(ResponseMessages.RefreshTokenFailed);

            return _authHelper.CreateSecurityToken(user.Id, user.UserName, roles, company);
        }
    }
}