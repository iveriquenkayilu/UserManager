using Microsoft.AspNetCore.Http;
using UserManagerService.Entities.Datatypes;
using UserManagerService.Shared.Models.Helpers;

namespace UserManagerService.Shared.Models.Company
{
    public class CompanyInputModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CompanyTypeOption Type { get; set; }
        public IFormFile Logo { get; set; }
        // This is not working for now, don't waste time on it 
        // Upload from FE then send the url
        //public string Url { get; set; }
        public ShortLocationModel Location { get; set; }
    }
}
