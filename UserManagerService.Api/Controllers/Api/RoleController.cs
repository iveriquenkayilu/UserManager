using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Constants;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models;
using UserManagerService.Shared.Models.Roles;

namespace UserManagerService.Api.Controllers
{
    [Authorize(Roles = RoleConstants.ADMIN)]
    [Route("api/roles")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        public RoleController(IUserContext userContext, ILogger<RoleController> logger, IRoleService roleService) : base(userContext, logger)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await _roleService.GetRolesAsync();
            return CustomResponse.Success(ResponseMessages.RolesFetched, roles);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RoleInputModel input)
        {
            var model = await _roleService.AddRoleAsync(input);
            return CustomResponse.Success(ResponseMessages.RoleCreated, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoleInputModel input)
        {
            var model = await _roleService.UpdateRoleAsync(id, input);
            return CustomResponse.Success(ResponseMessages.RoleUpdated, model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteRoleAsync(id);
            return CustomResponse.Success(ResponseMessages.RoleDeleted);
        }
    }
}
