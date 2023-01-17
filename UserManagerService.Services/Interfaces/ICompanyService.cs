using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Shared.Models.Company;

namespace UserManagerService.Services.Interfaces
{
	public interface ICompanyService
	{
		Task<CompanyModel> AddCompanyAsync(CompanyInputModel input);
		Task<CompanyUserModel> AddUserAsync(CompanyUserInputModel input);
		Task DeleteCompanyAsync(Guid id);
		Task DeleteUserAsync(CompanyUserInputModel input);
		Task<List<CompanyModel>> GetCompaniesAsync();
		Task<List<CompanyShortModel>> GetCompaniesAsync(Guid userId);
		Task<CompanyModel> UpdateCompanyAsync(Guid id, CompanyInputModel input);
	}
}