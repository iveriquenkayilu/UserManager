namespace RainyCorp.UserManagerService.Shared.Constants
{
    public class ResponseMessages
    {
        #region User

        public const string AuthenticationFailed = "AuthenticationFailed";
        public const string UserAuthenticated = "UserAuthenticated";
        public const string EmailExists = "EmailExists";
        public const string UserCreated = "UserCreated";
        public const string FailedToCreatUser = "FailedToCreatUser";
        public const string WrongCredentials = "FailedToCreatUser";
        public const string InvalidRefreshToken = "InvalidRefreshToken";
        public const string RefreshTokenFailed = "RefreshTokenFailed";
        public const string UserNotFound = "UserNotFound";
        public const string TokensRefreshed = "TokensRefreshed";

        #endregion

        #region Input

        public const string InvalidInput = "InvalidInput";

        #endregion
    }
}