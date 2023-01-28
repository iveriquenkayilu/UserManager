using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace UserManagerService.Shared.Models.Helpers
{
    public class UploadMultipleFilesModel
    {
        public string Key { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
