using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services.Interfaces
{
	public interface ICompanyService
	{
		Task<CompanyModel> AddCompanyAsync(CompanyInputModel input);
		Task<CompanyUserModel> AddUserAsync(CompanyUserInputModel input);
		Task DeleteCompanyAsync(Guid id);
		Task DeleteUserAsync(CompanyUserInputModel input);
		Task<List<CompanyModel>> GetCompaniesAsync();
        Task<List<UserModel>> GetCompanyUsersAsync();
        Task<List<CompanyModel>> GetCreatedCompaniesAsync();
        Task<List<CompanyModel>> GetMyCompaniesAsync();
        Task<List<CompanyShortModel>> GetUserCompaniesAsync(Guid userId);
        Task<CompanyModel> UpdateCompanyAsync(Guid id, CompanyInputModel input);
	}
}