using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace UserManagerService.Shared.Models.Helpers
{
    public class UploadFileInputModel
    {
        public Guid FolderId { get; set; }
        public string AccessLevel { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
