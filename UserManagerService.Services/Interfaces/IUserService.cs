using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Shared.Models.Search;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services.Interfaces
{
	public interface IUserService
	{
		//Task<List<UserModel>> GetAsync();
		//Task<UserModel> GetAsync(long id);
		////Task<IdentityResult> RegisterAsync(RegisterModel form);
		//Task<UserModel> GetByPhoneNumber(string phoneNumber);
		//Task<UserModel> GetByUserName(string userName);
		Task<Guid> GetAdminId();
		//Task UpdateProfile(UserModel user, IFormFile image, bool ImageHasChanged);
		Task<User> GetEntityAsync(Guid id);
		Task DeleteVisitorAsync(Guid id);
		Task<Guid> AddVisitorAsync(VisitorModel input);
		Task<bool> VistiorExists(Guid id);
		Task<UserProfile> GetUserProfileAsync(Guid id);
		Task<List<UserProfile>> GetUserProfilesByIdsAsync(List<Guid> ids);
		Task DeleteUserAsync(Guid id);
		Task<UserModel> UpdateUserAsync(Guid id, UserInputModel input);
		Task<List<UserModel>> GetUsersAsync();
		Task<LoginOutputModel> GetAuthTokenAsync(LoginToCompanyInputModel input);
		Task<MyProfile> GetMyProfileAsync();
		Task<AccessTokenModel> GetAuthTokenAsync(LoginInputModel input);
		Task<List<LoginSessionModel>> GetLoginSessionsAsync(LoginSessionInputModel input);
		Task<UserProfile> RegisterUserAsync(RegisterModel input);
		Task<AuthTokenModel> RefreshTokenAsync(RefreshTokenInput input);
		Task<List<SearchResultModel>> SearchUsers(string key);
		Task<LoginWithRedirectOutputTokenModel> GetAuthSessionToRedirectAsync(LoginToCompanyInputModel input, string redirectUrl);
		Task<AuthTokenModel> GetAuthTokenWithSessionIdAsync(LoginInputWithSession input);
    }
}