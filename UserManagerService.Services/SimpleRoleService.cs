using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagerService.Repository;

namespace UserManagerService.Services
{
    public class SimpleRoleService
    {
        private readonly ApplicationDbContext _dbContext;
        public SimpleRoleService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId, Guid companyId)
            => await _dbContext.UserRoles.Where(r => r.UserId == userId && r.CompanyId == companyId)
                .Select(r => r.Role.Name.ToUpper()).ToListAsync();
    }
}
