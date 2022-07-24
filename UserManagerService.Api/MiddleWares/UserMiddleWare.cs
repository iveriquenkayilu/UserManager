using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Interfaces.Shared;

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
        public async Task InvokeAsync(HttpContext context, IUserService userService, IAuthHelper authHelper) //IRealTimeHub realTimeHub
        {
            //string visitorId = context.Request.Cookies["VisitorId"];

            //if (context.Session.GetString(".TigerStamina.Session") == null)
            //    context.Session.SetString(".TigerStamina.Session", Guid.NewGuid().ToString());// TODO make constant

            //if ((visitorId == null && !context.User.Identity.IsAuthenticated) || !await userService.VistiorExists((long.Parse(visitorId))))
            //{
            // a new visitor notification.

            var visitor = authHelper.GetVisitorInfo();

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
    }
}
