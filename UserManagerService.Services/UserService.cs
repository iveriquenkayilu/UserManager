using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
using UserManagerService.Shared.Models.Helpers;
using UserManagerService.Shared.Models.Search;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ICompanyService _companyService;
        private readonly SimpleRoleService _simpleRoleService;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthHelper _authHelper;
        public UserService(ICompanyService companyService, SimpleRoleService simpleRoleService, IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger, IAuthHelper authHelper, SignInManager<User> signInManager) : base(userContext, unitOfWork, mapper, logger)
        {
            _companyService = companyService;
            _simpleRoleService = simpleRoleService;
            _authHelper = authHelper;
            _signInManager = signInManager;
        }

        public async Task<List<SearchResultModel>> SearchUsers(string key)
        {
            var users = await UnitOfWork.Query<User>(c => c.Name.ToLower().Contains(key.ToLower())
            || c.Surname.ToLower().Contains(key.ToLower()))
                .Select(c => new SearchResultModel
                {
                    Id = c.Id,
                    Name = c.Name + " " + c.Surname,
                    Image = c.Picture
                }).ToListAsync();
            var userIds = users.Select(c => c.Id).ToList();
            Expression<Func<User, bool>> predicate = userIds.Count > 0 ? c => !userIds.Contains(c.Id) : c => true;
            var usersByUsername = await UnitOfWork.Query<User>(c => c.UserName.ToLower().Contains(key.ToLower()))
                        .Where(predicate)
                        .Select(c => new SearchResultModel
                        {
                            Id = c.Id,
                            Name = c.Name + " " + c.Surname,
                            Image = c.Picture
                        }).ToListAsync();
            if (usersByUsername.Count > 0)
                users.AddRange(usersByUsername);

            return users;
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
            var user = await LoginAsync(Mapper.Map<LoginToCompanyInputModel>(input));
            return _authHelper.GetAccessToken(user.Id, user.Username, null, null);
        }

        public async Task<LoginWithRedirectOutputTokenModel> GetAuthTokenWithRedirectAsync(LoginToCompanyInputModel input, string redirectUrl)
        {
            var user = await LoginAsync(input);
            await CheckIfUserBelongsToCompanyAsync(user.Id, input.CompanyId ?? Guid.Empty);

            var token = new LoginWithRedirectTokenModel
            {
                SessionId = user.SessionId,
                ReturnUrl = redirectUrl
            };

            var userToken = new UserToken
            {
                Name = "RedirectToken",
                Value = JsonConvert.SerializeObject(token),
                ExpiredAt = DateTime.Now.AddMinutes(1),
                UserId = user.Id
            };
            await UnitOfWork.AddToCompanyAsync(userToken);
            await UnitOfWork.SaveAsync();

            return new LoginWithRedirectOutputTokenModel
            {
                TokenId = userToken.Id,
                UserId = userToken.UserId,
                CompanyId = input.CompanyId
            };
        }

        public async Task<AuthTokenModel> GetAuthTokenByTokenIdAsync(LoginWithRedirectOutputTokenModel input)
        {
            var token = await UnitOfWork.Query<UserToken>(u => u.Id == input.TokenId).FirstOrDefaultAsync();
            if (token is null)
                throw new CustomException("Failed to get token");

            var tokenValue = JsonConvert.DeserializeObject<LoginWithRedirectTokenModel>(token.Value);
            // maybe the origin of Request

            return await GetAuthTokenWithSessionIdAsync(new LoginInputWithSession
            {
                SessionId = tokenValue.SessionId,
                UserId = token.UserId,
                CompanyId = token.CompanyId
            });
        }

        public async Task<AuthTokenModel> GetAuthTokenWithSessionIdAsync(LoginInputWithSession input)
        {
            Logger.LogInformation($"User {input.UserId} is getting the token with session {input.SessionId}");
            if (input.UserId == Guid.Empty || input.SessionId == Guid.Empty)
                throw new CustomException(ResponseMessages.InvalidInput);

            var session = await UnitOfWork.Query<LoginSession>(l => l.Id == input.SessionId).FirstOrDefaultAsync();

            var visitor = _authHelper.GetVisitorInfo();
            if (session.CompanyId != input.CompanyId)
                throw new CustomException("Invalid session");
            if (session.Device != visitor.Device)
                throw new CustomException("Invalid session");
            if (session.IpAddress != visitor.AddressIp)
                throw new CustomException("Invalid session");

            var user = await _signInManager.UserManager.FindByIdAsync(input.UserId.ToString());

            if (input.CompanyId == null)
            {
                return _authHelper.CreateSecurityToken(user.Id, user.UserName, null, null);
            }
            var companyId = input.CompanyId ?? Guid.Empty;
            await CheckIfUserBelongsToCompanyAsync(user.Id, companyId);

            var roles = await _simpleRoleService.GetUserRolesAsync(user.Id, companyId);
            var company = await _companyService.GetCompanyAsync(companyId);
            var tokens = _authHelper.CreateSecurityToken(user.Id, user.UserName, roles.Select(r => r.ToUpper()).ToList(), company);
            tokens.SessionId = session.Id;
            return tokens;
        }

        private async Task CheckIfUserBelongsToCompanyAsync(Guid userId, Guid companyId)
        {
            if (companyId != Guid.Empty)
                if (!await UnitOfWork.AnyAsync<CompanyUser>(c => c.CompanyId == companyId && c.UserId == userId))
                    throw new CustomException("User does not belong to company");
        }
        public async Task<LoginOutputModel> GetAuthTokenAsync(LoginToCompanyInputModel input) // 1 or multiple companies
        {
            var user = await LoginAsync(input);

            var companies = await _companyService.GetUserCompaniesAsync(user.Id);

            if (companies.Count == 0)
            {
                if (input.CompanyId != null)
                {
                    Logger.LogInformation($"User {user.Id} does not belong to a company {input.CompanyId}");
                    await AddLoginSessionAndThrowException(user.Id, input.CompanyId, ResponseMessages.AuthenticationFailed);
                }

                return new LoginOutputModel
                {
                    AuthTokens = _authHelper.CreateSecurityToken(user.Id, user.Username, null, null)
                };
            }

            if (companies.Count == 1)
            {
                if (input.CompanyId != null && !companies.Any(c => c.Id == input.CompanyId))
                    await AddLoginSessionAndThrowException(user.Id, input.CompanyId, ResponseMessages.AuthenticationFailed);


                var roles = await _simpleRoleService.GetUserRolesAsync(user.Id, companies[0].Id);
                var tokens = _authHelper.CreateSecurityToken(user.Id, user.Username, roles.Select(r => r.ToUpper()).ToList(), companies.First());
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
                .Where(predicate).Include(l => l.Address)
                .Select(l => new LoginSessionModel
                {
                    CreatedAt = l.CreatedAt,
                    Device = l.Device,
                    IpAddress = l.IpAddress,
                    Location = l.Address.Description,
                    LatLng = new LatLng
                    {
                        Latitude = l.Address.Latitude,
                        Longitude = l.Address.Longitude
                    },
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

        private async Task<LoginOutputWithSession> LoginAsync(LoginToCompanyInputModel input) // Can use the other model that has companyId, and remove companyId as parm
        {
            Logger.LogInformation($"User {input.Username} {input.Email} is getting the token");
            if ((string.IsNullOrEmpty(input.Username) && string.IsNullOrEmpty(input.Email)) || string.IsNullOrEmpty(input.Password))
                throw new CustomException(ResponseMessages.InvalidInput);

            var user = string.IsNullOrEmpty(input.Email) ?
                await _signInManager.UserManager.FindByNameAsync(input.Username) :
                  await _signInManager.UserManager.FindByEmailAsync(input.Email);
            if (user is null)
            {
                await AddLoginSessionAndThrowException(Guid.Empty, input.CompanyId, ResponseMessages.WrongCredentials);
            }

            var signedIn = await _signInManager.PasswordSignInAsync(user, input.Password, true, false);

            var userOutput = Mapper.Map<LoginOutputWithSession>(user);
            if (signedIn.Succeeded)
            {
                await _signInManager.SignOutAsync(); // remove the cookie
                userOutput.SessionId = await AddLoginSessionAsync(user.Id, input.CompanyId ?? Guid.Empty, HttpStatusCode.OK);
            }
            else
            {
                await AddLoginSessionAndThrowException(user.Id, input.CompanyId, ResponseMessages.AuthenticationFailed);
            }
            return userOutput;
        }
        private async Task AddLoginSessionAndThrowException(Guid userId, Guid? companyId, string message)
        {
            await AddLoginSessionAsync(userId, companyId ?? Guid.Empty, HttpStatusCode.BadRequest);
            throw new CustomException(message);
        }

        private async Task<Guid> AddLoginSessionAsync(Guid userId, Guid companyId, HttpStatusCode status)
        {
            var visitor = _authHelper.GetVisitorInfo();
            var location = await _authHelper.GetIpAddressLocation(visitor.AddressIp);

            var address = new Address
            {
                Latitude = location.latitude,
                Longitude = location.longitude,
                Name = Names.LoginLocation,
                Description = location.Location,
                CreatorId = userId
            };

            Guid sessionId = new();
            var addressExist = await UnitOfWork.Query<Address>(a => a.Name == Names.LoginLocation && a.CreatorId == userId && a.Latitude == location.latitude && a.Longitude == location.longitude).FirstOrDefaultAsync();
            await UnitOfWork.ExecuteInTransactionAsync(async trans =>
            {
                if (addressExist == null)
                {
                    await UnitOfWork.AddAsync(address);
                    await UnitOfWork.SaveAsync();
                }
                else
                    address.Id = addressExist.Id;

                var session = new LoginSession
                {
                    Device = visitor.Device,
                    IpAddress = visitor.AddressIp,
                    Status = ((int)status).ToString(),
                    AddressId = address.Id,
                    CreatorId = userId,
                    CompanyId = companyId
                };
                await UnitOfWork.AddAsync(session);
                await UnitOfWork.SaveAsync();
                sessionId = session.Id;
            });

            return sessionId;
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