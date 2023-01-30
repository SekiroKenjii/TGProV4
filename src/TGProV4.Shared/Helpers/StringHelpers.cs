namespace TGProV4.Shared.Helpers;

public static class StringHelpers
{
    public static class Message
    {
        public static string AlreadyTaken(string prop) { return $"{prop} is already taken."; }

        public static string NotFound(string entity) { return $"{entity} Not Found!"; }

        public static string MissedConfig(string entity) { return $"Missed config of {entity}"; }
    }

    public static class Mail
    {
        public static string ConfirmEmailBody(string url, string firstName, string lastName)
        {
            return $@"Hello {TransformName(firstName, lastName)},
Thank you for joining TGPro VietNam.

We’d like to confirm that your account was created successfully. To access TGPro click the link below.

{url}

If you experience any issues logging into your account, reach out to us at support@tgprovn.com.

Best,
The TGPro VietNam team
";
        }

        public static string ChangePasswordBody(string url, string firstName, string lastName)
        {
            return $@"Hi {TransformName(firstName, lastName)},
There was a request to change your password!

If you did not make this request then please ignore this email.

Otherwise, please click this link to change your password:

{url}
";
        }

        private static string TransformName(string firstName, string lastName)
        {
            return $"{firstName} {lastName}";
        }
    }
}
