namespace TGProV4.Shared.Constants.Application;

public static class ApplicationConstants
{
    public static class Messages
    {
        public const string ValidationError = "One or more validation failures have occurred.";
        public const string InvalidCredentialInfo = "Email or password is incorrect.";
        public const string EmailUnconfirmed = "Email is not confirmed.";

        public const string LockedUser =
            "Your account has been locked! Please contact your website administrator for more information.";

        public const string TokenRevoked = "Token Has Been Revoked.";
        public const string TokenExpired = "Token Has Been Expired.";
        public const string InvalidToken = "Invalid Client Token.";
        public const string Unauthorized = "You are not authorized!";
        public const string Forbidden = "You are not authorized to access this resource!";
        public const string NotAllowToAddOrDeleteAdminRole = "you are not allowed to add or delete Administrator Role.";
        public const string InternalServerError = "An unhandled error has occurred.";
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Moderator = "Moderator";
        public const string Basic = "Basic";
    }
}
