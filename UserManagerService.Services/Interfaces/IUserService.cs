using UserManagerService.Entities;
using UserManagerService.Shared.Models.User;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UserManagerService.Services.Interfaces
{
    public interface IUserService
    {
        //Task<List<UserModel>> GetAsync();
        //Task<UserModel> GetAsync(long id);
        ////Task<IdentityResult> RegisterAsync(RegisterModel form);
        //Task<UserModel> GetByPhoneNumber(string phoneNumber);
        //Task<UserModel> GetByUserName(string userName);
        Task<long> GetAdminId();
        //Task UpdateProfile(UserModel user, IFormFile image, bool ImageHasChanged);
        Task<User> GetEntityAsync(long id);

        /// <summary>
        /// Deletes the visitor asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteVisitorAsync(long id);

        /// <summary>
        /// Adds the visitor asynchronously.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<long> AddVisitorAsync(VisitorModel input);

        /// <summary>
        /// Checks if the visitor exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> VistiorExists(long id);
        Task<UserProfile> GetUserProfileAsync(long id);
        Task<List<UserProfile>> GetUserProfilesByIdsAsync(List<long> ids);
    }
}