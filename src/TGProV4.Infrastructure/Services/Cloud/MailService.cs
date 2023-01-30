namespace TGProV4.Infrastructure.Services.Cloud;

public class MailService : IMailService
{
    private readonly MailConfiguration _mailConfiguration;
    private readonly ILogger<MailService> _logger;
    private readonly SmtpClient _smtpClient;

    public MailService(IOptions<MailConfiguration> options, ILogger<MailService> logger)
    {
        _mailConfiguration = options.Value;
        _logger = logger;
        _smtpClient = new SmtpClient();
    }

    public async Task Send(MailRequest request)
    {
        try
        {
            var mail = new MimeMessage {
                Sender = new MailboxAddress(_mailConfiguration.DisplayName, request.From ?? _mailConfiguration.From),
                Subject = request.Subject,
                Body = new BodyBuilder {
                    HtmlBody = request.Body
                }.ToMessageBody()
            };
            mail.To.Add(MailboxAddress.Parse(request.To));

            await _smtpClient.ConnectAsync(_mailConfiguration.Host, _mailConfiguration.Port,
                SecureSocketOptions.StartTls);
            await _smtpClient.AuthenticateAsync(_mailConfiguration.UserName, _mailConfiguration.Password);
            await _smtpClient.SendAsync(mail);
            await _smtpClient.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error has occurred: {message}", e.Message);
        }
    }
}
