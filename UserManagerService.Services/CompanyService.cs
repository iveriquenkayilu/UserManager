using AutoMapper;
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
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Helpers;
using UserManagerService.Shared.Interfaces.Helpers;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.Helpers;
using UserManagerService.Shared.Models.Search;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services
{
    public class CompanyService : BaseService, ICompanyService
    {
        private readonly IFileManagerHelper _fileManagerHelper;
        public CompanyService(IFileManagerHelper fileManagerHelper, IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ApiService> logger) : base(userContext, unitOfWork, mapper, logger)
        {
            _fileManagerHelper = fileManagerHelper;
        }

        public async Task<List<UserModel>> GetCompanyUsersAsync()
        {
            return await UnitOfWork.Query<CompanyUser>(u => u.CompanyId == UserContext.CompanyId).Include(o => o.User)
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
        }

        public async Task<List<CompanyWithUsersModel>> GetCompaniesWithUsers()
        {
            var companies = await UnitOfWork.Query<Company>()
                .Select(c => new CompanyWithUsersModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Description = c.Description,
                    UpdatedAt = c.UpdatedAt,
                    CreatedAt = c.CreatedAt,
                    Users = c.CompanyUsers.Select(u => new UserModel
                    {
                        Id = u.Id,
                        Name = u.User.Name,
                        Surname = u.User.Surname,
                        CreatedAt = u.User.CreatedAt,
                        Username = u.User.UserName,
                        Picture = u.User.Picture
                    }).ToList()
                }).ToListAsync();
            return companies;
        }

        public async Task<List<SearchResultModel>> SearchCompanies(string key)
        {
            var companies = await UnitOfWork.Query<Company>(c => c.Name.ToLower().Contains(key.ToLower()))
                .Select(c => new SearchResultModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Title = c.Type.ToString(),
                    Image = c.Logo
                }).ToListAsync();
            var companyIds = companies.Select(c => c.Id).ToList();

            Expression<Func<Company, bool>> predicate = companyIds.Count > 0 ? c => !companyIds.Contains(c.Id) : c => true;
            var companiesByDescription = await UnitOfWork.Query<Company>(c => c.Description.ToLower().Contains(key.ToLower()))
                .Where(predicate)
                .Select(c => new SearchResultModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Title = c.Type.ToString(),
                    Image = c.Logo
                }).ToListAsync();
            if (companiesByDescription.Count > 0)
                companies.AddRange(companiesByDescription);

            //companyIds = companies.Select(c => c.Id).ToList();
            //var companiesByType = await UnitOfWork.Query<Company>(c => c.Type.Contains(key.ToLower()))
            //   .Where(predicate)
            //    .Select(c => new SearchResultModel
            //    {
            //        Id = c.Id,
            //        Name = c.Name,
            //        Description = c.Description,
            //        Type = c.Type.ToString(),
            //        Image = c.Logo
            //    }).ToListAsync();
            //if (companiesByType.Count > 0)
            //    companies.AddRange(companiesByType);

            return companies;
        }

        public async Task<List<CompanyModel>> GetCompaniesAsync()
        {
            var companies = await UnitOfWork.Query<Company>()
                .Select(c => new CompanyModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Description = c.Description,
                    UpdatedAt = c.UpdatedAt,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();
            return companies;
        }

        public async Task<List<CompanyModel>> GetCreatedCompaniesAsync() // Or where you are the manager?
        {
            Logger.LogInformation($"User {UserContext.UserId} is getting companies that they created");

            var companies = await UnitOfWork.Query<Company>(c => c.CreatorId == UserContext.UserId)

                .Select(c => new CompanyModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Description = c.Description,
                    UpdatedAt = c.UpdatedAt,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();
            return companies;
        }

        public async Task<List<CompanyModel>> GetMyCompaniesAsync()
        {
            Logger.LogInformation($"User {UserContext.UserId} is getting their companies");

            var createdCompanies = await GetCreatedCompaniesAsync();
            var createdCompaniesIds = createdCompanies.Select(c => c.Id).ToList();
            var companies = await UnitOfWork.Query<CompanyUser>(c => c.UserId == UserContext.UserId)
                .Where(c => !createdCompaniesIds.Contains(c.CompanyId))
                .Include(c => c.Company)
                .Select(c => new CompanyModel
                {
                    Id = c.Company.Id,
                    Name = c.Company.Name,
                    Type = c.Company.Type,
                    Description = c.Company.Description,
                    UpdatedAt = c.Company.UpdatedAt,
                    CreatedAt = c.Company.CreatedAt
                }).ToListAsync();

            companies.AddRange(createdCompanies);
            return companies;
        }

        public async Task<List<CompanyShortModel>> GetUserCompaniesAsync(Guid userId)
        {
            Logger.LogInformation($"User {userId} is getting their companies");

            return await UnitOfWork.Query<CompanyUser>(c => c.UserId == userId)
                  .Include(o => o.Company)
                  .Select(c => new CompanyShortModel
                  {
                      Id = c.CompanyId,
                      Name = c.Company.Name
                  }).ToListAsync();
        }

        public async Task<CompanyModel> AddCompanyAsync(CompanyInputModel input)
        {
            var company = Mapper.Map<Company>(input);
            company.CreatorId = UserContext.UserId;

            // upload to fileService
            var files = await _fileManagerHelper.UploadFileAsync(new UploadSingleFileModel { AccessLevel = "Public", File = input.Logo });
            company.Logo = "url";
            await UnitOfWork.AddAsync(company);
            await UnitOfWork.SaveAsync();
            return Mapper.Map<CompanyModel>(company);
        }

        public async Task<CompanyModel> UpdateCompanyAsync(Guid id, CompanyInputModel input)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to update company {id}");

            // TODO validation

            var company = await UnitOfWork.Query<Company>(d => d.Id == id).FirstOrDefaultAsync();

            if (company is null)
                throw new CustomException($"Company {id} not found");

            //company.UpdatedBy = UserContext.UserId;
            company.UpdatedAt = DateTime.Now;
            company.Name = input.Name;

            UnitOfWork.Update(company);
            await UnitOfWork.SaveAsync();

            return Mapper.Map<CompanyModel>(company);
        }

        public async Task DeleteCompanyAsync(Guid id)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to delete company {id}");
            await UnitOfWork.SoftDeleteEntityAsync<Company>(id, UserContext.UserId);
        }

        public async Task<CompanyUserModel> AddUserAsync(CompanyUserInputModel input)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to add user {input.UserId} to company {input.CompanyId}");

            // TODO validation
            var companyUser = new CompanyUser { CompanyId = input.CompanyId, UserId = input.UserId };
            companyUser.CreatorId = UserContext.UserId;

            await UnitOfWork.AddAsync(companyUser);
            await UnitOfWork.SaveAsync();

            return Mapper.Map<CompanyUserModel>(companyUser);
        }

        public async Task DeleteUserAsync(CompanyUserInputModel input)
        {
            Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to add user {input.UserId} to company {input.CompanyId}");

            // TODO validation
            var companyUser = await UnitOfWork.Query<CompanyUser>(c => c.CompanyId == input.CompanyId && c.UserId == input.UserId)
                .FirstOrDefaultAsync();
            if (companyUser is null)
                throw new CustomException("User doesn't belong to company");

            UnitOfWork.Delete(companyUser);
            await UnitOfWork.SaveAsync();
        }
    }
}