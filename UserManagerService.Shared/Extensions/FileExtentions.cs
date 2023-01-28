using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UserManagerService.Shared.Extensions
{
    public static class FileExtentions
    {
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static async Task<StreamContent> GetStreamContent(this IFormFile formFile)
        {
            var content = new MemoryStream();
            await formFile.CopyToAsync(content);
            return new StreamContent(content);
        }
    }
}
