using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Helpers;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Services
{
    public class CompanyService : BaseService, ICompanyService
    {
        public CompanyService(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ApiService> logger) : base(userContext, unitOfWork, mapper, logger)
        {
        }

        public async Task<List<CompanyModel>> GetCompaniesAsync()
        {
            var companies = await UnitOfWork.Query<Company>()
                .Select(c => new CompanyModel
                {
                    Id = c.Id,
                    Name = c.Name,
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

        public async Task<CompanyModel> AddCompanyAsync(CompanyInputModel input)
        {
            var company = Mapper.Map<Company>(input);
            company.CreatorId = UserContext.UserId;
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


        public async Task<CompanyModel> GetMyCompanyAsync()
        {
            Logger.LogInformation($"User {UserContext.UserId} is getting company data");

            var companyUsers = await UnitOfWork.Query<CompanyUser>(c => c.UserId == UserContext.UserId)
                .Include(o => o.Company).Include(o => o.User)
                .Select(c => new CompanyModel
                {
                    Id = c.User.Id,
                    Name = c.User.Name,
                    Users = c.User.CompanyUsers.Select(u => new UserModel
                    {
                        Id = u.Id,
                        Name = u.User.Name,

                    }).ToList()
                }).FirstOrDefaultAsync();

            return companyUsers;
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