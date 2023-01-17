using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Shared.Models.Roles;

namespace UserManagerService.Services.Interfaces
{
	public interface IRoleService
	{
		Task<RoleModel> AddRoleAsync(RoleInputModel input);
		Task<List<UserRoleModel>> AssignRolesToUserAsync(UserRoleInputModel input);
		Task DeleteRoleAsync(Guid id);
		Task<List<RoleModel>> GetRolesAsync();
		Task<List<RoleModel>> GetRolesByUserIdAsync(Guid userId);
		Task<List<string>> GetUserRolesAsync(Guid userId, Guid companyId);
		Task RemoveRolesFromUserAsync(UserRoleInputModel input);
		Task<RoleModel> UpdateRoleAsync(Guid id, RoleInputModel input);
	}
}