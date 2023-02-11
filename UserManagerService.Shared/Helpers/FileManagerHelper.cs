using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Extensions;
using UserManagerService.Shared.Interfaces.Helpers;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Models;
using UserManagerService.Shared.Models.Helpers;
using UserManagerService.Shared.Settings;

namespace UserManagerService.Shared.Helpers
{
    public class FileManagerHelper : IFileManagerHelper // TODO cache data
    {
        private readonly IHttpOrchestrator _httpOrchestrator;
        private readonly IUserContext _userContext;
        private readonly WebProtocolSettings _webProtocolSettings;
        private readonly ApiKeySettings _apiKeySettings;
        // get 
        private readonly ILogger<FileManagerHelper> _logger;
        public FileManagerHelper(IUserContext userContext, IHttpOrchestrator httpOrchestrator, IOptions<WebProtocolSettings> webProtocolSettings, IOptions<ApiKeySettings> apiKeySettings, ILogger<FileManagerHelper> logger)
        {
            _userContext = userContext;
            _httpOrchestrator = httpOrchestrator;
            _webProtocolSettings = webProtocolSettings.Value;
            _apiKeySettings = apiKeySettings.Value;
            _logger = logger;
        }

        public async Task<UploadedFileModel> UploadFileAsync(UploadSingleFileModel input)
        {
            var url = _webProtocolSettings.FileServiceUrl + "/api/v2/files";
            var header = new Dictionary<string, string> { { "Authorization", _userContext.JWTToken } };
            var file = new UploadedFileModel();
            try
            {
                var form = new Dictionary<string, string> { { "accessLevel", input.AccessLevel } };
                var result = await PostFileAsync(url, input.File, form);
                var data = JsonConvert.DeserializeObject<ResponseModel<UploadedFileModel>>(result);
                file = data.Data;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError("Timed out: " + ex.Message);
                throw new CustomException("Failed to upload file");
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured: " + e.Message, e);
                throw new CustomException("Failed to upload file");
            };

            return file;
        }

        //public async Task<List<UploadedFileModel>> UploadFileAsync(UploadFileInputModel input)
        //{
        //    var url = _webProtocolSettings.FileServiceUrl + "/api/v2/files";
        //    var header = new Dictionary<string, string> { { "Authorization", _userContext.JWTToken } };
        //    var users = new List<UploadedFileModel>();
        //    try
        //    {
        //        var payload = new { /*FolderId=null,*/ AccessLevel = input.AccessLevel };
        //        var result = await PostFileAsync(url, input.Files[0], payload);
        //        var data = JsonConvert.DeserializeObject<ResponseModel<List<UploadedFileModel>>>(result);
        //        users = data.Data;
        //    }
        //    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        //    {
        //        // Handle timeout.
        //        Console.WriteLine("Timed out: " + ex.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogInformation("Error occured: " + e.Message, e);
        //    };

        //    return users;
        //}

        private async Task<string> PostFileAsync(string url, IFormFile file, Dictionary<string, string> data = null)
        {
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                //Load the file and set the file's Content-Type header
                //fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                multipartFormContent.Add(await file.GetStreamContent(), "file", file.FileName);

                if (data is not null)
                {
                    foreach (var item in data)
                    {
                        multipartFormContent.Add(new StringContent(item.Value), item.Key);
                    }
                }
                //var payload = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");  // key
                //multipartFormContent.Add(payload);

                var httpClient = new HttpClient();
                //Send it
                var response = await httpClient.PostAsync(url, multipartFormContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> PostFilesAsync<T>(string url, UploadMultipleFilesModel files, T data)
        {
           
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                //Load the file and set the file's Content-Type header
                //fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                //Add the file

                //files.Files.ForEach(f =>
                //{
                //    var content = new MemoryStream();
                //    f.CopyToAsync(content).Wait();
                //    multipartFormContent.Add(new StreamContent(content), files.Key, f.FileName);
                //});

                //var bytes = new MemoryStream(await files.Files[0].GetBytes());
                //multipartFormContent.Add(new StreamContent(bytes),"Files","Test");
                //multipartFormContent.Add( await files.Files[0].GetStreamContent(), files.Key, files.Files[0].FileName);
                //multipartFormContent.Add(new StringContent("Public"),"AccessLevel");

                //var payload = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");  // key
                //multipartFormContent.Add(payload);

                var httpClient = new HttpClient();
                //Send it
                var response = await httpClient.PostAsync(url, multipartFormContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }



        //public static string FillUserNameById(Guid userId, List<UserProfileModel> userProfiles)
        //{
        //    var profile = userProfiles.Where(u => u.Id == userId).FirstOrDefault();
        //    if (profile is null)
        //        return $"User {userId}";
        //    else
        //        return GetProfileName(profile);
        //}

        //public static string GetProfileName(UserProfileModel profile)
        //  => $"{profile?.Name} {profile?.Surname}";
    }
}
