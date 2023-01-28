using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Models.Search;

namespace UserManagerService.Api.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        public SearchController(ILogger<ServiceController> logger, IUserService userService, ICompanyService companyService) : base(logger)
        {
            _userService = userService;
            _companyService = companyService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchQueryModel input)
        {
            if (string.IsNullOrEmpty(input.Key))
                return Ok(new ResultsModel());

            var results = new ResultsModel
            {
                Companies = input.Companies ? await _companyService.SearchCompanies(input.Key) : new(),
                Users = input.Users ? await _userService.SearchUsers(input.Key) : new()
            };
            return Ok(results);
        }
    }
}
