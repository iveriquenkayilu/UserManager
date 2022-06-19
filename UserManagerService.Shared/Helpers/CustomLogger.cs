using Microsoft.Extensions.Logging;
using System;

namespace UserManagerService.Shared.Helpers
{
    public static class CustomLogger
    {
        public static void LogWithUserInfo(this ILogger logger, Guid userId, string username, string message)
        {
            logger.LogInformation($"User {username}, id={userId} {message}");
        }
    }
}
