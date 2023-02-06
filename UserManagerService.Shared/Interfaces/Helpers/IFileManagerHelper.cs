using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Shared.Models.Helpers;

namespace UserManagerService.Shared.Interfaces.Helpers
{
    public interface IFileManagerHelper
    {
        //Task<List<UserProfileModel>> GetUserProfilesAsync(List<Guid> userIds);
        Task<List<UploadedFileModel>> UploadFileAsync(UploadSingleFileModel input);
    }
}
