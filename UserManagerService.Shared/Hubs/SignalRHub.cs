using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UserManagerService.Shared.Hubs
{
    /// <summary>
    /// Implements the real time hub.
    /// </summary>
    public class SignalRHub : Hub //, ISignalRHub  // can only work as Singleton? Test it as Scoped and Transcient
    {
        private readonly ILogger<SignalRHub> _logger;
        private readonly IServiceProvider _service;
        public SignalRHub(ILogger<SignalRHub> logger, IServiceProvider service)
        {
            _logger = logger;
            _service = service;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(Context.UserIdentifier))
                {
                    var id = long.Parse(Context.UserIdentifier);
                    //using (var scope = _service.CreateScope())
                    //{
                    //    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    //    var user = await unitOfWork.FirstOrDefaultAsync<User>(u => u.Id == id);

                    //        user.IsConnected = true;
                    //        unitOfWork.Update(user);
                    //        await unitOfWork.SaveAsync();
                    //        await Log($"Client {Context.User.Identity.Name} has connected");                                                               
                    //}
                }
                else
                    await Log($"{Context.ConnectionId} connected!");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            };

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var id = long.Parse(Context.UserIdentifier);
            //using (var scope = _service.CreateScope())
            //{
            //    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            //    var user = await unitOfWork.FirstOrDefaultAsync<User>(u => u.Id == id);
            //        user.IsConnected = false;
            //        unitOfWork.Update(user);
            //        await unitOfWork.SaveAsync();
            //        await Log($"Client {Context.User.Identity.Name} has disconnected");

            //}

            await base.OnDisconnectedAsync(exception);
        }
        public async Task Log(string input)
        {
            _logger.LogInformation(input);
            if (Clients != null)
                await Clients.All.SendAsync("AddLog", input);
        }

        public async Task MessageAsync(string toUser, string message)
        {
            string username = Context.User.Identity.Name ?? Context.ConnectionId;
            await Clients.All.SendAsync("Message", "Receive", username, message);
            //await Clients.Client(toUser).SendAsync("Message", "Receive", username , message);
        }
    }
}