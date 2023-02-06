using Microsoft.AspNetCore.Http;
using System;

namespace UserManagerService.Shared.Models.Helpers
{
    public class UploadSingleFileModel
    {
        public Guid FolderId { get; set; }
        public string AccessLevel { get; set; }
        public IFormFile File { get; set; }
    }
}
