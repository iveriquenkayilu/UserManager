using Microsoft.AspNetCore.Http;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using UAParser;

namespace UserManagerService.Api.MiddleWares
{
    public class UserMiddleWare
    {
        private readonly RequestDelegate _requestDelegate;
        public UserMiddleWare(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        /// <summary>
        /// Invokes the middle ware.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userService"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IUserService userService) //IRealTimeHub realTimeHub
        {
            //string visitorId = context.Request.Cookies["VisitorId"];

            //if (context.Session.GetString(".TigerStamina.Session") == null)
            //    context.Session.SetString(".TigerStamina.Session", Guid.NewGuid().ToString());// TODO make constant

            //if ((visitorId == null && !context.User.Identity.IsAuthenticated) || !await userService.VistiorExists((long.Parse(visitorId))))
            //{
            // a new visitor notification.

            try {

                var userAgent = context.Request.Headers["User-Agent"];

                var parser = Parser.GetDefault();
                var operatingSystem = parser.ParseOS(userAgent).ToString();
                var accessType = parser.ParseUserAgent(userAgent).ToString();
                var device = parser.ParseDevice(userAgent).ToString();
                var addressIp = GetIpAddress(context);

                var visitor = new VisitorModel
                {
                    AccessType = accessType,
                    AddressIp = addressIp,
                    Device = device,
                    OperatingSystem = operatingSystem,
                };
            }
            catch(Exception e)
            {

            };

                //var id = await userService.AddVisitorAsync(visitor);

                //context.Response.Cookies.Append("VisitorId", id.ToString(), new CookieOptions()
                //{
                //    Path = "/",
                //    HttpOnly = true,
                //    Secure = false,
                //});
                //Test maybe not await
                //await realTimeHub.SendNotificationAsync(new AddNotificationModel
                //{
                //    LinkId = id,
                //    To = NotificationOption.AdminAndCoaches,
                //    Type = NotificationType.NewVisitor,
                //    LinkParameter = id.ToString()
                //});
            //}
            await _requestDelegate(context);

        }

        private string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;

            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList
                    .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }

            return ipAddress.ToString();
        }
    }
}
