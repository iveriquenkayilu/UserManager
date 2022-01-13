using System.Collections.Generic;
using System.Threading.Tasks;
using RainyCorp.UserManagerService.Common.Models;
using RainyCorp.UserManagerService.Models.Message;

namespace RainyCorp.UserManagerService.Interfaces.Services
{
    public interface IMessageService
    {
        Task<List<MessageModel>> GetAsync();
        Task<MessageModel> GetAsync(long id);
        Task SendAsync(SendMessageModel form);
        Task<long> SendAsync(VisitorMessageModel input);
    }
}