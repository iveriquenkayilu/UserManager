using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagerService.Entities;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Constants;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models.User;
using System.Threading.Tasks;

namespace UserManagerService.Services
{
    public class UserService : BaseService, IUserService
    {
        //private readonly IMapper _mapper;
        //private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ApiService> logger) : base(userContext, unitOfWork, mapper, logger)
        {
        }

        //public async Task<List<UserModel>> GetAsync()
        //{
        //    var users = await _userRepository.GetAsync();           
        //    return _mapper.Map<List<UserModel>>(users);
        //}

        //public async Task<UserModel> GetAsync(long id) // with roles
        //{
        //    var user = await UnitOfWork.Query<User>(u => u.Id == id).FirstOrDefaultAsync();

        //    return _mapper.Map<UserModel>(user);
        //}
        public async Task<User> GetEntityAsync(long id) => await UnitOfWork.GetAsync<User>(id);

        public async Task DeleteVisitorAsync(long id)
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
        public async Task<bool> VistiorExists(long id) => await UnitOfWork.AnyAsync<Visitor>(v => v.Id == id);

        //public async Task AddRoles(long currentUserId, long userId, List<string> roles)
        //{
        //    var currentUser = await _userRepository.GetAsync(currentUserId);
        //    if (currentUser.Id != await GetAdminId()) throw new NotImplementedException(); // only admin can do this or a specific coach
        //    var user =await _userRepository.GetAsync(userId);
        //    _userRepository.AddRoles(user, roles);  // if doesn't work, go to repository, don't use userManager but the tables, roles
        //}

        public async Task<long> GetAdminId()
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

        /// <summary>
        /// Adds the visitor asynchronously.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<long> AddVisitorAsync(VisitorModel input)
        {
            // TODO validation
            var visitor = Mapper.Map<Visitor>(input);
            await UnitOfWork.AddAsync<Visitor>(visitor);
            await UnitOfWork.SaveAsync();
            return visitor.Id;
        }
    }
}