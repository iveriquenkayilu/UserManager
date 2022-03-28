using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RainyCorp.UserManagerService.Entities;
using RainyCorp.UserManagerService.Interfaces.Repositories;
using RainyCorp.UserManagerService.Shared.Interfaces.Services;
using RainyCorp.UserManagerService.Shared.Models.Service;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService.Services
{
    public class ApiService : BaseService, IApiService
    {
        public ApiService(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ApiService> logger) : base(userContext, unitOfWork, mapper, logger)
        {
        }

        public async Task<ServiceApiKeyModel> GetApiKeyAsync(ApiKeyRequestModel input)
        {
            var apiKey = await UnitOfWork.Query<ServiceApiKey>(s => s.KeyName.ToUpper() == input.KeyName.ToUpper()).FirstOrDefaultAsync();

            if (apiKey == null)
            {
                apiKey = new ServiceApiKey
                {
                    KeyName = input.KeyName,
                    Value = GenerateBase64String(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(20), // can come from the appsettings
                };
                await UnitOfWork.AddAsync(apiKey);
                await UnitOfWork.SaveAsync();
            }
            else
            {
                if (apiKey.ExpiresAt > DateTime.UtcNow)
                    throw new Exception(); // Not expired
                else
                {
                    apiKey.Value = GenerateBase64String();
                    apiKey.ExpiresAt = DateTime.UtcNow.AddMinutes(20); // can come from the appsettings
                    UnitOfWork.Update(apiKey);
                    await UnitOfWork.SaveAsync();
                }
            }

            return new ServiceApiKeyModel
            {
                KeyName = "Your service key name",
                Value = apiKey.Value,
                ExpiresAt = apiKey.ExpiresAt
            };
        }

        private string GenerateBase64String()  // TODO put in helper
        {
            string value;
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                value = Convert.ToBase64String(randomBytes);
            }
            return value;
        }
    }
}
