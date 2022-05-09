using Microsoft.Extensions.Logging;

namespace UserManagerService.Shared.Helpers
{
    public static class CustomLogger
    {
        public static void LogWithUserInfo(this ILogger logger, long userId, string username, string message)
        {
            logger.LogInformation($"User {username}, id={userId} {message}");
        }
    }
}
