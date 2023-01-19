using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Constants;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models;
using UserManagerService.Shared.Models.Company;

namespace UserManagerService.Api.Controllers
{
    [Authorize]
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _companyService;
        public CompanyController(IUserContext userContext, ILogger<ServiceController> logger, ICompanyService companyService) : base(userContext, logger)
        {
            _companyService = companyService;
        }

        [Authorize(Roles = RoleConstants.ADMIN)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var companies = await _companyService.GetCompaniesAsync();
            return Ok(ResponseModel.Success(ResponseMessages.CompaniesFetched, companies));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CompanyInputModel input, [FromForm] List<IFormFile> Files)
        {
            var receivedFiles = HttpContext.Request.Form.Files.ToList();
            var model = await _companyService.AddCompanyAsync(input);
            return CustomResponse.Success("Company created successfully", model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CompanyInputModel input)
        {
            var model = await _companyService.UpdateCompanyAsync(id, input);
            return CustomResponse.Success("Company updated successfully", model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _companyService.DeleteCompanyAsync(id);
            return CustomResponse.Success("Company deleted successfully");
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] CompanyUserInputModel input)
        {
            var model = await _companyService.AddUserAsync(input);
            return CustomResponse.Success("User added to Company successfully", model);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser([FromBody] CompanyUserInputModel input)
        {
            await _companyService.DeleteUserAsync(input);
            return CustomResponse.Success("User deleted from company successfully");
        }
    }
}
