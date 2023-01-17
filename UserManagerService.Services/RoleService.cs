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
using UserManagerService.Shared.Models.Roles;

namespace UserManagerService.Services
{
	public class RoleService : BaseService, IRoleService
	{
		public RoleService(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleService> logger) : base(userContext, unitOfWork, mapper, logger)
		{
		}

		public async Task<List<RoleModel>> GetRolesAsync()
			=> await UnitOfWork.QueryByCompanyId<Role>()
				.Select(c => new RoleModel
				{
					Id = c.Id,
					Name = c.Name,
					CreatedAt = c.CreatedAt,
					UpdatedAt = c.UpdatedAt
				}).ToListAsync();

		public async Task<RoleModel> AddRoleAsync(RoleInputModel input)
		{
			var company = Mapper.Map<Role>(input);
			company.CreatorId = UserContext.UserId;
			await UnitOfWork.AddAsync(company);
			await UnitOfWork.SaveAsync();
			return Mapper.Map<RoleModel>(company);
		}

		public async Task<RoleModel> UpdateRoleAsync(Guid id, RoleInputModel input)
		{
			Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to update role {id}");

			// TODO validation

			var company = await UnitOfWork.QueryByCompanyId<Role>(d => d.Id == id).FirstOrDefaultAsync();

			if (company is null)
				throw new CustomException($"Role {id} not found");

			//company.UpdatedBy = UserContext.UserId;
			company.UpdatedAt = DateTime.Now;
			company.Name = input.Name;

			UnitOfWork.Update(company);
			await UnitOfWork.SaveAsync();

			return Mapper.Map<RoleModel>(company);
		}

		public async Task DeleteRoleAsync(Guid id)
		{
			Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to delete company {id}");
			await UnitOfWork.SoftDeleteEntityAsync<Role>(id, UserContext.UserId);
		}

		public async Task<List<string>> GetUserRolesAsync(Guid userId, Guid companyId)
			=> await UnitOfWork.Query<UserRole>(c => c.UserId == userId && c.CompanyId == companyId).Select(u => u.Role.Name).ToListAsync();

		public async Task<List<RoleModel>> GetRolesByUserIdAsync(Guid userId)
		{
			Logger.LogInformation($"User {UserContext.UserId} is getting roles for user {userId}");

			var userRoles = await UnitOfWork.QueryByCompanyId<UserRole>(c => c.UserId == userId)
				.Include(o => o.Role).Include(o => o.User)
				.Select(c => new RoleModel
				{
					Id = c.Role.Id,
					Name = c.Role.Name,
					CreatedAt = c.Role.CreatedAt,
					UpdatedAt = c.Role.UpdatedAt
				}).ToListAsync();

			return userRoles;
		}

		public async Task<List<UserRoleModel>> AssignRolesToUserAsync(UserRoleInputModel input)
		{
			Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to assign roles {string.Join(",", input.RoleIds)} to user {input.UserId}");

			// TODO validation

			var userRoles = input.RoleIds.Select(id => new UserRole
			{
				UserId = input.UserId,
				RoleId = id,
				CreatorId = UserContext.UserId
			}).ToList();

			await UnitOfWork.AddRangeAsync(userRoles);
			await UnitOfWork.SaveAsync();

			return Mapper.Map<List<UserRoleModel>>(userRoles);
		}

		public async Task RemoveRolesFromUserAsync(UserRoleInputModel input)
		{
			Logger.LogWithUserInfo(UserContext.UserId, UserContext.Username, $"is trying to add user {input.UserId} to company {input.RoleIds}");

			// TODO validation
			var userRoles = await UnitOfWork.Query<UserRole>(c => input.RoleIds.Contains(c.RoleId) && c.UserId == input.UserId)
				.ToListAsync();
			if (userRoles is null || userRoles.Count == 0)
				throw new CustomException("Roles not assigned to user");

			foreach (var u in userRoles)
				UnitOfWork.Delete(u);
			await UnitOfWork.SaveAsync();
		}
	}
}