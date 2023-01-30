namespace TGProV4.Shared.Constants.Application;

public static class ApplicationConstants
{
    public static class Secrets
    {
        public const string LoginProvider = "TGProV4.Identity";
        public const string DefaultPassword = "P@ssw0rd1";
    }

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
        public const string UploadImageError = "An error has occurred while trying to upload an image.";
        public const string HeaderError = "One or more required header values were missing.";
        public const string MailSent = "An email has been sent. Please check your mailbox and follow the instructions.";
        public const string InternalServerError = "An unhandled error has occurred.";
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Moderator = "Moderator";
        public const string Basic = "Basic";
    }

    public static class DefaultImages
    {
        public const string FemaleAvatar = "https://res.cloudinary.com/dglgzh0aj/image/upload/v1637290486/TGProV3/users/default/default_female_photo.jpg";
        public const string FemaleAvatarId = "TGProV3/users/default/default_female_photo";

        public const string MaleAvatar = "https://res.cloudinary.com/dglgzh0aj/image/upload/v1637290486/TGProV3/users/default/default_male_photo.jpg";
        public const string MaleAvatarId = "TGProV3/users/default/default_male_photo";

        public const string BrandImage = "https://res.cloudinary.com/dglgzh0aj/image/upload/v1637290486/TGProV3/brands/default/default_brand_photo.jpg";
        public const string BrandImageId = "TGProV3/brands/default/default_brand_photo";

        public const string ProductImage = "https://res.cloudinary.com/dglgzh0aj/image/upload/v1637290486/TGProV3/products/default/default_product_photo.jpg";
        public const string ProductImageId = "TGProV3/products/default/default_product_photo";
    }

    public static class Entities
    {
        public const string User = "users";
        public const string Brand = "brands";
        public const string Product = "products";
    }

    public static class TableSchemas
    {
        public const string Identity = "Identity";
        public const string Production = "Production";
    }

    public static class ClaimTypes
    {
        public const string Permission = "Permission";
    }

    public static class Routes
    {
        public const string ResetPassword = "api/account/reset-password/";
        public const string ConfirmEmail = "api/identity/confirm-email/";
    }
}
