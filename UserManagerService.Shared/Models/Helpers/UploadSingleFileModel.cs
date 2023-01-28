using Microsoft.AspNetCore.Http;

namespace UserManagerService.Shared.Models.Helpers
{
    public class UploadSingleFileModel
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
