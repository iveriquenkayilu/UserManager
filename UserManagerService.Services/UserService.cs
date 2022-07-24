using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
using UserManagerService.Shared.Helpers;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthHelper _authHelper;
        public UserService(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ApiService> logger, IAuthHelper authHelper, SignInManager<User> signInManager) : base(userContext, unitOfWork, mapper, logger)
        {
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
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company.Name,
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

        public async Task<UserProfile> GetUserProfileAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomException("User id cannot be empty");

            var profile = await UnitOfWork.Query<User>(u => u.Id == id).Select(u => new UserProfile
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Username = u.UserName,
                Picture = u.Picture,
                Email = u.Email
            }).FirstOrDefaultAsync();

            var company = await UnitOfWork.Query<CompanyUser>(o => o.UserId == id)
                   .Include(o => o.Company).Select(o => (Company)o.Company).FirstOrDefaultAsync();

            if (company is not null)
            {
                profile.CompanyId = company.Id;
                profile.CompanyName = company.Name;
            }
            return profile;
        }

        public async Task<AuthTokenModel> GetAuthTokenAsync(LoginModel input)
        {
            Logger.LogInformation($"User {input.Username} {input.Email} is getting the token");
            if ((string.IsNullOrEmpty(input.Username) && string.IsNullOrEmpty(input.Email)) || string.IsNullOrEmpty(input.Password))
                throw new CustomException(ResponseMessages.InvalidInput);

            var tokens = new AuthTokenModel();
            var user = string.IsNullOrEmpty(input.Email) ?
                await _signInManager.UserManager.FindByNameAsync(input.Username) :
                  await _signInManager.UserManager.FindByEmailAsync(input.Email);
            if (user is null)
                throw new CustomException(ResponseMessages.WrongCredentials);

            var signedIn = await _signInManager.PasswordSignInAsync(user, input.Password, true, false);
            if (signedIn.Succeeded)
            {
                await _signInManager.SignOutAsync(); // remove the cookie
                var roles = (List<string>)await _signInManager.UserManager.GetRolesAsync(user);

                // Get default company
                var company = input.CompanyId is null ?
                    await UnitOfWork.Query<CompanyUser>(o => o.UserId == user.Id)
                    .Include(o => o.Company).Select(o => new CompanyShortModel
                    {
                        Id = o.Company.Id,
                        Name = o.Company.Name
                    }).FirstOrDefaultAsync()
                    : await UnitOfWork.Query<CompanyUser>(o => o.UserId == user.Id && o.CompanyId == input.CompanyId)
                    .Include(o => o.Company).Select(o => new CompanyShortModel
                    {
                        Id = o.Company.Id,
                        Name = o.Company.Name
                    }).SingleOrDefaultAsync();

                var visitor = _authHelper.GetVisitorInfo();

                var session = new LoginSession
                {
                    Device = visitor.Device,
                    IpAdress = visitor.AddressIp,
                    Status = "200"
                };
                await UnitOfWork.AddAsync(session);
                await UnitOfWork.SaveAsync();

                tokens = _authHelper.CreateSecurityToken(user.Id, user.UserName, roles.Select(r => r.ToUpper()).ToList(), company);
            }
            else
                throw new CustomException(ResponseMessages.AuthenticationFailed);

            return tokens;
        }

        public async Task DeleteVisitorAsync(Guid id)
        {
            var visitor = await UnitOfWork.GetAsync<Visitor>(id);
            if (visitor == null) return;
            UnitOfWork.Delete<Visitor>(visitor);  // test if needs to save
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
    }
}