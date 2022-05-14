using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Shared.Models.Company;

namespace UserManagerService.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyModel> AddCompanyAsync(CompanyInputModel input);
        Task<CompanyUserModel> AddUserAsync(CompanyUserInputModel input);
        Task DeleteCompanyAsync(long id);
        Task DeleteUserAsync(CompanyUserInputModel input);
        Task<List<CompanyModel>> GetCompaniesAsync();
        Task<CompanyModel> UpdateCompanyAsync(long id, CompanyInputModel input);
    }
}