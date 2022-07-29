namespace UserManagerService.Shared.Constants
{
    public class ResponseMessages
    {
        #region User

        public const string AuthenticationFailed = "AuthenticationFailed";
        public const string UserAuthenticated = "UserAuthenticated";
        public const string EmailExists = "EmailExists";
        public const string UserCreated = "UserCreated";
        public const string FailedToCreatUser = "FailedToCreatUser";
        public const string WrongCredentials = "WrongCredentials";
        public const string InvalidRefreshToken = "InvalidRefreshToken";
        public const string RefreshTokenFailed = "RefreshTokenFailed";
        public const string UserNotFound = "UserNotFound";
        public const string TokensRefreshed = "TokensRefreshed";
        public const string UserProfilesFetched = "UserProfilesFetched";
        public const string Unauthorized = "Unauthorized";

        #endregion

        #region Company

        public const string CompaniesFetched = "CompaniesFetched";

        #endregion

        #region Roles

        public const string RolesFetched = "Roles fetched";
        public const string RoleCreated = "Role created successfully";
        public const string RoleUpdated = "Role updated successfully";
        public const string RoleDeleted = "Role deleted successfully";

        #endregion

        #region Input

        public const string InvalidInput = "InvalidInput";

        #endregion
    }
}